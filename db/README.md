# Курсовая работа по базам данных

Тема: разработка базы данных и приложения для учета поставок ноутбуков в салон компьютерной техники.

Студентка: Токарева. Первая буква фамилии: Т, вариант по методичке: 11.

## Состав работы

- `sql/01_schema.sql` - создание базы `coursework_db` и таблиц MySQL.
- `sql/00_setup_user.sql` - создание пользователя приложения и выдача прав.
- `sql/02_seed.sql` - тестовые данные для проверки запросов и приложения.
- `sql/03_queries.sql` - ответы SQL на все вопросы варианта 11.
- `src/LaptopSupplyApp/` - простое приложение C# WinForms + MySQL.
- `docs/schema.md` - описание ER-схемы и реляционной модели.
- `docs/coursework-report.docx` - пояснительная записка после генерации.
- `docs/source-materials/` - Markdown-версии исходных файлов преподавателя и бриф для ревьюера.

## Локальная база

Используется MySQL 8.4:

- сервер: `127.0.0.1`
- порт: `3306`
- база: `coursework_db`
- пользователь приложения: `course_user`
- пароль: `course_password`

Первичная настройка пользователя выполняется под `root` после создания базы:

```powershell
mysql -uroot -p --default-character-set=utf8mb4 -e "SOURCE C:/Projects/Alyona-session/db/sql/01_schema.sql; SOURCE C:/Projects/Alyona-session/db/sql/00_setup_user.sql;"
```

Развернуть тестовые данные:

```powershell
mysql -ucourse_user -pcourse_password --default-character-set=utf8mb4 -e "SOURCE C:/Projects/Alyona-session/db/sql/02_seed.sql;"
```

Проверить запросы:

```powershell
mysql -ucourse_user -pcourse_password --default-character-set=utf8mb4 -e "SOURCE C:/Projects/Alyona-session/db/sql/03_queries.sql;"
```

## Приложение

Сборка:

```powershell
dotnet build C:/Projects/Alyona-session/db/src/LaptopSupplyApp/LaptopSupplyApp.csproj
```

Запуск:

```powershell
dotnet run --project C:/Projects/Alyona-session/db/src/LaptopSupplyApp/LaptopSupplyApp.csproj
```

Приложение подключается к MySQL, показывает ноутбуки, поставщиков и поставки, выполняет учебные SELECT-запросы и позволяет добавить или удалить поставку.
Также в интерфейсе есть отдельные команды варианта: изменить ОЗУ `Rover1000` и удалить поставщика `SComp`.
