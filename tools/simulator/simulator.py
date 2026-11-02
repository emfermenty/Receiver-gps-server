import argparse
import datetime
import math
import random
import socket
import struct
import threading
import time

PACKET_LENGTH = 34

# Коды событий (байт 3 геопакета). Названия по кодам знает ReceiverApp.
EVENT_PERIODIC = 0
EVENT_TURN = 1
EVENT_STOP = 2
EVENT_START_MOVING = 3
EVENT_LOW_BATTERY = 4
EVENT_SOS = 5

LOW_BATTERY_MV = 3750

DEVICES = [
    {"imei": "862549040911052", "manufacturer": "SimTrack", "model": "ST-100", "firmware": "1.4.2",
     "lat": 56.8345, "lon": 60.6123},
    {"imei": "356307042441013", "manufacturer": "SimTrack", "model": "ST-200", "firmware": "2.0.1",
     "lat": 56.8389, "lon": 60.5974},
    {"imei": "864893030561234", "manufacturer": "GeoSim", "model": "GS-1", "firmware": "0.9.7",
     "lat": 56.8261, "lon": 60.6302},
]


def create_handshake_packet(device: dict) -> bytes:
    """
    Handshake: 4 байта префикс + 15 байт IMEI + 1 байт длины + "производитель;модель;прошивка" (UTF-8)
    """
    prefix = b"\x00\x00\x00\x01"
    imei_bytes = device["imei"].encode("utf-8").ljust(15, b"0")[:15]
    info = f'{device["manufacturer"]};{device["model"]};{device["firmware"]}'.encode("utf-8")
    return prefix + imei_bytes + bytes([len(info)]) + info


def create_geo_packet(event_code, lat, lon, speed, course, height, gps_sats, glonass_sats, v_battery):
    packet = bytearray()
    packet.extend(b'\x01\x02')

    # 2: длина пакета
    packet.append(PACKET_LENGTH)
    # 3: код события
    packet.append(event_code)
    # 4..8: пустые/неиспользуемые байты
    packet.extend(b'\x00' * 5)

    # 9..12 latitude
    packet.extend(struct.pack("<f", lat))
    # 13..16 longitude
    packet.extend(struct.pack("<f", lon))
    # 17..18 course*10
    packet.extend(struct.pack("<H", int(course * 10)))
    # 19..20 speed*10
    packet.extend(struct.pack("<H", int(speed * 10)))
    # 21..22 height
    packet.extend(struct.pack("<H", height))
    # 23..24 пусто
    packet.extend(b'\x00\x00')
    # 25: спутники, младший полубайт GPS, старший ГЛОНАСС
    packet.append((gps_sats & 0x0F) | ((glonass_sats & 0x0F) << 4))
    # 26..29 timestamp
    unix_time = int(datetime.datetime.now(datetime.timezone.utc).timestamp())
    packet.extend(struct.pack("<I", unix_time))
    # 30..31 пусто
    packet.extend(b'\x00\x00')
    # 32..33 battery
    packet.extend(struct.pack("<H", v_battery))

    return bytes(packet)


class DeviceState:
    """Состояние одного трекера: двигается по курсу, иногда поворачивает и останавливается."""

    def __init__(self, device: dict):
        self.lat = device["lat"]
        self.lon = device["lon"]
        self.speed = random.randint(20, 60)
        self.course = random.uniform(0, 360)
        self.height = random.randint(200, 250)
        self.battery = random.randint(3900, 4150)

    def step(self, interval: float) -> int:
        prev_speed, prev_course, prev_battery = self.speed, self.course, self.battery

        if self.speed == 0:
            if random.random() < 0.3:
                self.speed = random.randint(20, 60)
        elif random.random() < 0.1:
            self.speed = 0
        else:
            self.speed = min(90, max(5, self.speed + random.randint(-5, 5)))

        if self.speed > 0:
            if random.random() < 0.2:
                self.course += random.choice((-1, 1)) * random.uniform(45, 90)
            else:
                self.course += random.uniform(-5, 5)
            self.course %= 360

        # смещение в км за интервал -> градусы
        distance_km = self.speed * interval / 3600
        rad = math.radians(self.course)
        self.lat += distance_km * math.cos(rad) / 111.32
        self.lon += distance_km * math.sin(rad) / (111.32 * math.cos(math.radians(self.lat)))

        self.height = min(300, max(150, self.height + random.randint(-3, 3)))

        self.battery -= random.randint(0, 3)
        if self.battery < 3600:
            self.battery = 4150  # поставили на зарядку

        turn = abs((self.course - prev_course + 180) % 360 - 180)
        if random.random() < 0.01:
            return EVENT_SOS
        if prev_battery >= LOW_BATTERY_MV > self.battery:
            return EVENT_LOW_BATTERY
        if prev_speed > 0 and self.speed == 0:
            return EVENT_STOP
        if prev_speed == 0 and self.speed > 0:
            return EVENT_START_MOVING
        if turn > 30:
            return EVENT_TURN
        return EVENT_PERIODIC


def run_device(device: dict, host: str, port: int, interval: float):
    state = DeviceState(device)
    imei = device["imei"]

    while True:
        try:
            with socket.create_connection((host, port)) as client:
                print(f"[{imei}] Подключено к {host}:{port}")
                client.sendall(create_handshake_packet(device))

                while True:
                    event_code = state.step(interval)
                    packet = create_geo_packet(event_code, state.lat, state.lon, state.speed, state.course,
                                               state.height, random.randint(4, 9), random.randint(2, 6),
                                               state.battery)
                    client.sendall(packet)
                    print(f"[{imei}] событие={event_code} lat={state.lat:.6f} lon={state.lon:.6f} "
                          f"speed={state.speed} course={state.course:.1f} battery={state.battery}")
                    time.sleep(interval)
        except OSError as ex:
            print(f"[{imei}] Нет соединения ({ex}), повтор через 5 с")
            time.sleep(5)


def main():
    parser = argparse.ArgumentParser(description="Имитатор GPS-трекеров")
    parser.add_argument("--host", default="127.0.0.1")
    parser.add_argument("--port", type=int, default=40433)
    parser.add_argument("--devices", type=int, default=len(DEVICES), help="сколько устройств запустить")
    parser.add_argument("--interval", type=float, default=3, help="период отправки, секунды")
    args = parser.parse_args()

    threads = [threading.Thread(target=run_device, args=(device, args.host, args.port, args.interval), daemon=True)
               for device in DEVICES[:args.devices]]
    for thread in threads:
        thread.start()

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Остановлено")


if __name__ == "__main__":
    main()
