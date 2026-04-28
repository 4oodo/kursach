-- ==========================================
-- Полный скрипт создания БД EnergyManagement
-- ==========================================

-- 1. Создание базы данных
CREATE DATABASE EnergyManagement ON PRIMARY ( 
	NAME = N'EnergyManagement', 
	FILENAME = N'E:\Microsoft sql\MSSQL17.MSSQLSERVER\MSSQL\DATA\EnergyManagement.mdf', 
	SIZE = 10MB, 
	FILEGROWTH = 10% 
) LOG ON ( 
	NAME = N'EnergyManagement_log', 
	FILENAME = N'E:\Microsoft sql\MSSQL17.MSSQLSERVER\MSSQL\DATA\EnergyManagement_log.ldf', 
	SIZE = 5MB, 
	FILEGROWTH = 10% 
);
GO

USE EnergyManagement;
GO

-- 2. Создание таблиц
CREATE TABLE dbo.[User] ( 
	UserID INT PRIMARY KEY IDENTITY(1,1), 
	Username NVARCHAR(100) NOT NULL UNIQUE, 
	Password NVARCHAR(255) NOT NULL,
	Email NVARCHAR(255),
	FullName NVARCHAR(255),
	IsAdmin BIT NOT NULL DEFAULT 0
);

CREATE TABLE dbo.Building ( 
	BuildingID INT PRIMARY KEY IDENTITY(1,1), 
	BuildingName NVARCHAR(255) NOT NULL, 
	Address NVARCHAR(500) NOT NULL 
);

CREATE TABLE dbo.RoomCategory ( 
	RoomCategoryID INT PRIMARY KEY IDENTITY(1,1), 
	CategoryName NVARCHAR(100) NOT NULL UNIQUE 
);

CREATE TABLE dbo.TimePeriod ( 
	TimePeriodID INT PRIMARY KEY IDENTITY(1,1), 
	PeriodName NVARCHAR(100) NOT NULL UNIQUE 
);

CREATE TABLE dbo.Room ( 
	RoomID INT PRIMARY KEY IDENTITY(1,1), 
	BuildingID INT NOT NULL, 
	Floor INT NOT NULL, 
	RoomCategoryID INT NOT NULL, 
	FOREIGN KEY (BuildingID) REFERENCES dbo.Building(BuildingID), 
	FOREIGN KEY (RoomCategoryID) REFERENCES dbo.RoomCategory(RoomCategoryID) 
);

CREATE TABLE dbo.EnergyConsumption ( 
	ConsumptionID INT PRIMARY KEY IDENTITY(1,1), 
	RoomID INT NOT NULL, 
	[Date] DATE NOT NULL, 
	TimePeriodID INT NOT NULL, 
	EnergyValue DECIMAL(10, 2) NOT NULL CHECK (EnergyValue >= 0),
	FOREIGN KEY (RoomID) REFERENCES dbo.Room(RoomID), 
	FOREIGN KEY (TimePeriodID) REFERENCES dbo.TimePeriod(TimePeriodID) 
);

-- 3. Создание индексов для оптимизации производительности
CREATE INDEX IX_Room_BuildingID ON dbo.Room(BuildingID);
CREATE INDEX IX_Room_RoomCategoryID ON dbo.Room(RoomCategoryID);
CREATE INDEX IX_EnergyConsumption_RoomID ON dbo.EnergyConsumption(RoomID);
CREATE INDEX IX_EnergyConsumption_TimePeriodID ON dbo.EnergyConsumption(TimePeriodID);
CREATE INDEX IX_EnergyConsumption_Date ON dbo.EnergyConsumption([Date]);

-- 4. Добавление тестовых данных
-- Добавление пользователей
INSERT INTO dbo.[User] (Username, Password, Email, FullName, IsAdmin) VALUES 
('admin', 'admin123', 'admin@energy.com', 'Администратор', 1),
('user1', 'user123', 'user1@energy.com', 'Иван Петров', 0),
('user2', 'user123', 'user2@energy.com', 'Мария Сидорова', 0);

-- Добавление зданий
INSERT INTO dbo.Building (BuildingName, Address) VALUES 
('Главное здание', 'ул. Ленина, 10'),
('Корпус А', 'ул. Ленина, 12'),
('Корпус Б', 'ул. Советская, 5');

-- Добавление категорий помещений
INSERT INTO dbo.RoomCategory (CategoryName) VALUES 
('Офис'),
('Коридор'),
('Склад'),
('Кафе'),
('Серверная'),
('Лестничная клетка');

-- Добавление периодов времени
INSERT INTO dbo.TimePeriod (PeriodName) VALUES 
('Ночь (00:00-06:00)'),
('Утро (06:00-12:00)'),
('День (12:00-18:00)'),
('Вечер (18:00-00:00)');

-- Добавление помещений
INSERT INTO dbo.Room (BuildingID, Floor, RoomCategoryID) VALUES 
-- Главное здание
(1, 1, 1), (1, 1, 2), (1, 1, 3),
(1, 2, 1), (1, 2, 1), (1, 2, 5),
(1, 3, 4), (1, 3, 6),

-- Корпус А
(2, 1, 1), (2, 1, 2), (2, 1, 4),
(2, 2, 1), (2, 2, 3),

-- Корпус Б
(3, 1, 1), (3, 1, 6),
(3, 2, 1), (3, 2, 2);

-- Добавление данных энергопотребления
DECLARE @RoomCount INT;
DECLARE @i INT = 1;
DECLARE @Date DATE = '2024-01-01';
DECLARE @RoomID INT;
DECLARE @TimePeriodID INT;

SET @RoomCount = (SELECT COUNT(*) FROM dbo.Room);

WHILE @i <= 30
BEGIN
	SET @RoomID = (@i % @RoomCount) + 1;
	SET @TimePeriodID = ((@i - 1) % 4) + 1;

	INSERT INTO dbo.EnergyConsumption (RoomID, [Date], TimePeriodID, EnergyValue)
	VALUES (@RoomID, DATEADD(DAY, (@i - 1) / 4, @Date), @TimePeriodID, 
			15.0 + RAND() * 50.0);

	SET @i = @i + 1;
END

GO

-- 5. Создание представлений для отчетов
CREATE VIEW vw_EnergyReport AS
SELECT 
	b.BuildingID,
	b.BuildingName,
	r.RoomID,
	r.Floor,
	rc.CategoryName,
	tp.PeriodName,
	ec.[Date],
	ec.EnergyValue
FROM dbo.EnergyConsumption ec
JOIN dbo.Room r ON ec.RoomID = r.RoomID
JOIN dbo.Building b ON r.BuildingID = b.BuildingID
JOIN dbo.RoomCategory rc ON r.RoomCategoryID = rc.RoomCategoryID
JOIN dbo.TimePeriod tp ON ec.TimePeriodID = tp.TimePeriodID;

GO

-- Вывод информации
PRINT '======================================';
PRINT 'База данных EnergyManagement создана!';
PRINT '======================================';
PRINT 'Таблицы:';
PRINT '- Building (Здания)';
PRINT '- Room (Помещения)';
PRINT '- RoomCategory (Категории помещений)';
PRINT '- TimePeriod (Периоды времени)';
PRINT '- EnergyConsumption (Энергопотребление)';
PRINT '';
PRINT 'Тестовые данные добавлены!';
