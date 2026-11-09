# Лабораторная работа №3

Путь данных: Python-симулятор → ReceiverApp → SQLite (`raw_points`) → построчный gRPC с mTLS → mapsence → PostgreSQL/PostGIS.
Экспорт и импорт написаны на C#. Дополнительные баллы за разные языки не заявляются.

## Сертификаты

Из корня проекта в PowerShell (IP — адрес машины импорта):

```powershell
powershell -ExecutionPolicy Bypass -File tools/certs/generate-certs.ps1 -Addresses 192.168.100.3
```

Linux: `bash tools/certs/generate-certs.sh 192.168.100.3`.
Пароль PFX по умолчанию `receiver123`; для изменения используйте `-ClientCertPassword` в PowerShell или переменную `CLIENT_CERT_PASSWORD` в Bash и такое же значение в `.env`.
Скрипт перевыпускает сертификаты сервисов; после этого перезапустите контейнеры.

## Локальный запуск

Скопируйте `.env.example` в `.env`. Оставьте `TRANSPORT=grpc` и `GRPC_ADDRESS=https://mapsence:5001`.

```powershell
docker compose --profile export --profile import up -d --build postgis mapsence receiver simulator
```

Kafka для лабораторной не требуется. Swagger: http://localhost:5001/swagger. Внешний порт gRPC: 5002.

## Два физических устройства

Сертификаты выпустите один раз с IP/DNS машины импорта в SAN.
На машину импорта скопируйте `ca.crt`, `mapsence.crt`, `mapsence.key` в `certs/`.
На машину экспорта — `ca.crt`, `receiver-client.pfx`. Приватный ключ CA передавать не нужно.

На машине импорта в `.env` задайте `TRANSPORT=grpc` и запустите:

```powershell
docker compose --profile import up -d --build
```

На машине экспорта задайте `TRANSPORT=grpc`, `GRPC_ADDRESS=https://192.168.100.3:5002` (свой адрес) и пароль PFX:

```powershell
docker compose --profile export up -d --build receiver simulator
```

Разрешите входящие TCP-подключения на порт 5002 машины импорта. Имя/IP в адресе должно совпадать с SAN сертификата.

## Проверка перед сдачей

```powershell
docker compose logs --tail 30 receiver mapsence
docker compose exec postgis psql -U postgres -d mapsence -c "SELECT count(*) FROM zxc.geo_data;"
docker compose exec postgis psql -U postgres -d mapsence -c "SELECT tablename FROM pg_tables WHERE schemaname='zxc';"
```

В логах receiver должны появляться `[gRPC] Отправлена строка`, а число точек в PostgreSQL — расти.
При остановке mapsence строки остаются в SQLite и передаются после восстановления связи.
Для сдачи экспорт и импорт должны работать на двух физических устройствах; два контейнера на одном компьютере подходят только для локальной проверки.

Сохраняйте том `receiver-data` вместе с БД импорта: защита от дублей использует номер строки SQLite. Если сбросить только SQLite, сохранив PostgreSQL, номера будут использованы повторно и совпавшие строки будут пропущены. Для чистой демонстрации используйте новую пару БД либо сохраняйте оба существующих тома.
