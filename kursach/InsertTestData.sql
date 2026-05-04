-- Вставка тестовых данных в БД EnergyManagement

-- Проверка и добавление ролей
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE RoleName = 'User')
	INSERT INTO dbo.Role (RoleName) VALUES ('User');
IF NOT EXISTS (SELECT 1 FROM dbo.Role WHERE RoleName = 'Admin')
	INSERT INTO dbo.Role (RoleName) VALUES ('Admin');

-- Проверка и добавление ключей администратора
IF NOT EXISTS (SELECT 1 FROM dbo.AdminKey WHERE AdminKeyValue = 'ADMIN_KEY_2024_001')
	INSERT INTO dbo.AdminKey (AdminKeyValue, IsActive) VALUES ('ADMIN_KEY_2024_001', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.AdminKey WHERE AdminKeyValue = 'ADMIN_KEY_2024_002')
	INSERT INTO dbo.AdminKey (AdminKeyValue, IsActive) VALUES ('ADMIN_KEY_2024_002', 1);

-- Получить правильные RoleID
DECLARE @UserRoleID INT;
DECLARE @AdminRoleID INT;

SELECT @UserRoleID = RoleID FROM dbo.Role WHERE RoleName = 'User';
SELECT @AdminRoleID = RoleID FROM dbo.Role WHERE RoleName = 'Admin';

-- Добавление пользователей
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'admin')
	INSERT INTO dbo.[User] (Username, PasswordHash, RoleID) 
	VALUES ('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', @AdminRoleID);

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'user1')
	INSERT INTO dbo.[User] (Username, PasswordHash, RoleID) 
	VALUES ('user1', '04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb', @UserRoleID);

IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = 'user2')
	INSERT INTO dbo.[User] (Username, PasswordHash, RoleID) 
	VALUES ('user2', '04f8996da763b7a969b1028ee3007569eaf3a635486ddab211d512c85b9df8fb', @UserRoleID);

-- Вывод информации о загруженных данных
SELECT '✓ Роли:' AS [Information];
SELECT RoleID, RoleName FROM dbo.Role;

SELECT '' AS [Separator];
SELECT '✓ Ключи администратора:' AS [Information];
SELECT KeyID, AdminKeyValue, CASE WHEN IsActive = 1 THEN 'Активен' ELSE 'Неактивен' END AS Status FROM dbo.AdminKey;

SELECT '' AS [Separator];
SELECT '✓ Пользователи:' AS [Information];
SELECT u.UserID, u.Username, r.RoleName FROM dbo.[User] u
JOIN dbo.Role r ON u.RoleID = r.RoleID;
