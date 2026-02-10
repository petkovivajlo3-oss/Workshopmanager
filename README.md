# WorkshopManager (примерен проект)

Това е **готов .NET 8 Web API** проект за управление на автосервиз (клиенти, автомобили и сервизни поръчки).
Направен е в стил *слоеве* (Domain / Application / Infrastructure / Api), за да е четим и разширяем.

## Какво има вътре
- Customers (CRUD)
- Vehicles (CRUD, филтър по customerId)
- Service Orders (CRUD + добавяне на редове/части)
- EF Core + SQLite (`workshop.db`)
- Swagger UI на `/swagger`
- Seed данни при първо стартиране (включително пример с Golf 6)

## Стартиране
1) Изисквания: .NET SDK 8+
2) В папката на решението:

```bash
dotnet restore
dotnet run --project src/WorkshopManager.Api
```

Отвори:
- Swagger: http://localhost:5180/swagger

## Идеи за развитие
- Аутентикация (JWT)
- Отделна таблица за части/склад
- Планиране по механик + календар
- Валидации (FluentValidation)
- Front-end (React/Angular) или Razor Pages

> Забележка: Това НЕ е копи-пейст на чужд repo – структурата и моделите са преработени и разширени.
