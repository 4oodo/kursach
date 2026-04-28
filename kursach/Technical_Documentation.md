# Техническое описание системы управления энергопотреблением

## Архитектура приложения

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                │
│              (MainWindow.xaml / C#)                 │
└─────────────────────────────────────────────────────┘
						  ↕
┌─────────────────────────────────────────────────────┐
│                   Business Logic Layer               │
│              (MainWindow.xaml.cs)                   │
└─────────────────────────────────────────────────────┘
						  ↕
┌─────────────────────────────────────────────────────┐
│                    Data Access Layer                 │
│              (DatabaseService.cs)                   │
└─────────────────────────────────────────────────────┘
						  ↕
┌─────────────────────────────────────────────────────┐
│                   SQL Server Database                │
│                 (EnergyManagement DB)                │
└─────────────────────────────────────────────────────┘
```

## Слои приложения

### 1. Presentation Layer (UI)
- **Файлы**: `MainWindow.xaml`, `MainWindow.xaml.cs`, `App.xaml`
- **Технология**: WPF (Windows Presentation Foundation)
- **Язык**: XAML для UI, C# для логики
- **Фреймворк**: .NET Framework 4.7.2

**Компоненты**:
- 7 TabItems для различных функций
- DataGrid для отображения данных
- ComboBox для выбора связанных данных
- DatePicker для выбора дат
- TextBox для ввода текста

### 2. Business Logic Layer
- **Файл**: `MainWindow.xaml.cs`
- **Ответственность**: Обработка событий пользователя, валидация данных
- **Основные методы**:
  - `RefreshXxxBtn_Click()` - обновление данных
  - `AddXxxBtn_Click()` - добавление новых записей
  - `UpdateXxxBtn_Click()` - обновление существующих записей
  - `DeleteXxxBtn_Click()` - удаление записей
  - Методы для обработки отчетов и SQL запросов

### 3. Data Access Layer
- **Файл**: `Services/DatabaseService.cs`
- **Паттерн**: Repository Pattern
- **Основные методы**:
  - `GetAll<Entity>()` - получение всех записей
  - `Add<Entity>()` - добавление записи
  - `Update<Entity>()` - обновление записи
  - `Delete<Entity>()` - удаление записи
  - `ExecuteCustomQuery()` - выполнение SQL запросов
  - `GetEnergyConsumptionReport()` - получение отчета
  - `GetBuildingStatistics()` - получение статистики

## Модели данных

```csharp
class Building
{
	int BuildingID
	string BuildingName
	string Address
}

class Room
{
	int RoomID
	int BuildingID
	int Floor
	int RoomCategoryID
	string BuildingName (для отображения)
	string CategoryName (для отображения)
}

class RoomCategory
{
	int RoomCategoryID
	string CategoryName
}

class TimePeriod
{
	int TimePeriodID
	string PeriodName
}

class EnergyConsumption
{
	int ConsumptionID
	int RoomID
	DateTime Date
	int TimePeriodID
	decimal EnergyValue
	string RoomInfo (для отображения)
	string TimePeriodName (для отображения)
}
```

## Схема базы данных

### Таблица Building
```sql
BuildingID (int, PK, Identity)
BuildingName (nvarchar(255), NOT NULL)
Address (nvarchar(500), NOT NULL)
```

### Таблица RoomCategory
```sql
RoomCategoryID (int, PK, Identity)
CategoryName (nvarchar(100), NOT NULL, UNIQUE)
```

### Таблица TimePeriod
```sql
TimePeriodID (int, PK, Identity)
PeriodName (nvarchar(100), NOT NULL, UNIQUE)
```

### Таблица Room
```sql
RoomID (int, PK, Identity)
BuildingID (int, NOT NULL, FK -> Building)
Floor (int, NOT NULL)
RoomCategoryID (int, NOT NULL, FK -> RoomCategory)
```

### Таблица EnergyConsumption
```sql
ConsumptionID (int, PK, Identity)
RoomID (int, NOT NULL, FK -> Room)
Date (date, NOT NULL)
TimePeriodID (int, NOT NULL, FK -> TimePeriod)
EnergyValue (decimal(10,2), NOT NULL, CHECK >= 0)
```

## Индексы

```sql
IX_Room_BuildingID ON Room(BuildingID)
IX_Room_RoomCategoryID ON Room(RoomCategoryID)
IX_EnergyConsumption_RoomID ON EnergyConsumption(RoomID)
IX_EnergyConsumption_TimePeriodID ON EnergyConsumption(TimePeriodID)
IX_EnergyConsumption_Date ON EnergyConsumption([Date])
```

## Отношения между таблицами

```
Building
	↓ (1:N)
Room ← (M:1) → RoomCategory
	↓ (1:N)
EnergyConsumption ← (M:1) → TimePeriod
```

## Строка подключения

```
Data Source=DESKTOP-N513RVN;
Initial Catalog=EnergyManagement;
Integrated Security=True;
Connect Timeout=30;
Encrypt=True;
Trust Server Certificate=True;
Application Intent=ReadWrite;
Multi Subnet Failover=False;
Command Timeout=30
```

## Конфигурация

**Файл**: `App.config`

```xml
<connectionStrings>
	<add name="EnergyManagementDB" 
		 connectionString="..." 
		 providerName="System.Data.SqlClient" />
</connectionStrings>
```

## Процесс CRUD операций

### Create (Добавление)
1. Пользователь заполняет форму
2. Нажимает кнопку "Добавить"
3. Валидация данных в коде позади
4. Вызов `DatabaseService.Add<Entity>()`
5. Выполнение INSERT запроса
6. Обновление UI таблицы

### Read (Чтение)
1. При инициализации окна вызывается `LoadInitialData()`
2. Вызов `DatabaseService.GetAll<Entity>()`
3. Выполнение SELECT запроса
4. ObservableCollection возвращается и привязывается к DataGrid
5. Данные отображаются автоматически

### Update (Обновление)
1. Пользователь дважды кликает на строку в DataGrid
2. Данные заполняют текстовые поля
3. Пользователь изменяет данные
4. Нажимает "Обновить"
5. Вызов `DatabaseService.Update<Entity>()`
6. Выполнение UPDATE запроса
7. Таблица обновляется

### Delete (Удаление)
1. Пользователь выбирает строку
2. Нажимает "Удалить"
3. Появляется диалог подтверждения
4. Вызов `DatabaseService.Delete<Entity>()`
5. Выполнение DELETE запроса
6. Таблица обновляется

## Обработка ошибок

Все операции обернуты в try-catch блоки:

```csharp
try
{
	// Выполнение операции
}
catch (SqlException ex)
{
	MessageBox.Show($"Ошибка БД: {ex.Message}");
}
catch (InvalidOperationException ex)
{
	MessageBox.Show($"Ошибка операции: {ex.Message}");
}
catch (Exception ex)
{
	MessageBox.Show($"Неожиданная ошибка: {ex.Message}");
}
```

## Производительность

### Оптимизации
1. **Индексирование**: Все FK и часто используемые столбцы индексированы
2. **Кэширование**: ObservableCollection используется для уменьшения количества запросов
3. **Параметризованные запросы**: Защита от SQL injection
4. **Connection pooling**: Автоматическое управление соединениями

### Возможные улучшения
1. Внедрение Entity Framework для упрощения запросов
2. Асинхронные операции для больших объемов данных
3. Кэширование часто запрашиваемых данных
4. Пагинация для больших таблиц

## Безопасность

### Реализованные меры
1. **Параметризованные запросы**: Все SQL запросы используют параметры
2. **Integrated Security**: Используется Windows Authentication
3. **Шифрование соединения**: SSL Encrypt=True

### Рекомендации
1. Ограничить доступ к SQL консоли
2. Регулярно делать резервные копии БД
3. Использовать парольную защиту на уровне СУБД
4. Периодически пересматривать права доступа

## Тестирование

### Единичные тесты (рекомендуется добавить)
- Тесты DatabaseService методов
- Тесты валидации данных
- Тесты SQL запросов

### Интеграционные тесты
- Полный цикл CRUD операций
- Корректность отчетов
- Обработка ошибок

### Пользовательское тестирование
- Проверка UI интуитивности
- Производительность с большими объемами данных
- Поведение при потере соединения с БД

## Развертывание

### Требования
- Windows 7 SP1 или новее
- .NET Framework 4.7.2
- SQL Server 2012 или новее
- 100 МБ свободного места на диске

### Процесс
1. Скопировать файлы приложения
2. Выполнить Database_Setup.sql на целевом SQL Server
3. Обновить строку подключения в App.config
4. Запустить приложение

## Документация

- `README.md` - Общее описание проекта
- `Administrator_Guide.md` - Руководство пользователя
- `Technical_Documentation.md` - Техническое описание (этот файл)
- `Database_Setup.sql` - SQL скрипт инициализации БД
- `InitializeDatabase.sql` - Дополнительные тестовые данные

## История версий

**v1.0 (2024)**
- Начальная версия
- Реализованы все основные CRUD операции
- Добавлена SQL консоль
- Реализованы базовые отчеты

## Автор

Разработано для учебного проекта (курсач)

## Лицензия

Внутреннее использование
