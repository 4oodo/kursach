using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using kursach.Models;

namespace kursach.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            // SQL Server 2019 - используем правильное имя экземпляра
            // Если MSSQL17 не работает, используем просто localhost или (local)
            _connectionString = @"Data Source=localhost;Initial Catalog=EnergyManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

            // Добавляем обработчик для проверки SSL-сертификата (для надежности)
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Доверяем всем сертификатам (включая самоподписанные)
            // ВНИМАНИЕ: Это небезопасно для production! Используйте только для разработки.
            return true;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        #region Building CRUD

        public ObservableCollection<Building> GetAllBuildings()
        {
            var buildings = new ObservableCollection<Building>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT BuildingID, BuildingName, Address FROM dbo.Building";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            buildings.Add(new Building
                            {
                                BuildingID = (int)reader["BuildingID"],
                                BuildingName = reader["BuildingName"].ToString(),
                                Address = reader["Address"].ToString()
                            });
                        }
                    }
                }
            }
            return buildings;
        }

        public void AddBuilding(Building building)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.Building (BuildingName, Address) VALUES (@name, @address)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", building.BuildingName);
                    cmd.Parameters.AddWithValue("@address", building.Address);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateBuilding(Building building)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE dbo.Building SET BuildingName = @name, Address = @address WHERE BuildingID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", building.BuildingID);
                    cmd.Parameters.AddWithValue("@name", building.BuildingName);
                    cmd.Parameters.AddWithValue("@address", building.Address);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBuilding(int buildingID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.Building WHERE BuildingID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", buildingID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region RoomCategory CRUD

        public ObservableCollection<RoomCategory> GetAllRoomCategories()
        {
            var categories = new ObservableCollection<RoomCategory>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT RoomCategoryID, CategoryName FROM dbo.RoomCategory";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new RoomCategory
                            {
                                RoomCategoryID = (int)reader["RoomCategoryID"],
                                CategoryName = reader["CategoryName"].ToString()
                            });
                        }
                    }
                }
            }
            return categories;
        }

        public void AddRoomCategory(RoomCategory category)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.RoomCategory (CategoryName) VALUES (@name)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", category.CategoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRoomCategory(RoomCategory category)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE dbo.RoomCategory SET CategoryName = @name WHERE RoomCategoryID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", category.RoomCategoryID);
                    cmd.Parameters.AddWithValue("@name", category.CategoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRoomCategory(int categoryID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.RoomCategory WHERE RoomCategoryID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", categoryID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region TimePeriod CRUD

        public ObservableCollection<TimePeriod> GetAllTimePeriods()
        {
            var periods = new ObservableCollection<TimePeriod>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT TimePeriodID, PeriodName FROM dbo.TimePeriod";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            periods.Add(new TimePeriod
                            {
                                TimePeriodID = (int)reader["TimePeriodID"],
                                PeriodName = reader["PeriodName"].ToString()
                            });
                        }
                    }
                }
            }
            return periods;
        }

        public void AddTimePeriod(TimePeriod period)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.TimePeriod (PeriodName) VALUES (@name)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", period.PeriodName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTimePeriod(TimePeriod period)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE dbo.TimePeriod SET PeriodName = @name WHERE TimePeriodID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", period.TimePeriodID);
                    cmd.Parameters.AddWithValue("@name", period.PeriodName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTimePeriod(int periodID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.TimePeriod WHERE TimePeriodID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", periodID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Room CRUD

        public ObservableCollection<Room> GetAllRooms()
        {
            var rooms = new ObservableCollection<Room>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT r.RoomID, r.BuildingID, r.Floor, r.RoomCategoryID, 
                               b.BuildingName, rc.CategoryName
                        FROM dbo.Room r
                        INNER JOIN dbo.Building b ON r.BuildingID = b.BuildingID
                        INNER JOIN dbo.RoomCategory rc ON r.RoomCategoryID = rc.RoomCategoryID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rooms.Add(new Room
                            {
                                RoomID = (int)reader["RoomID"],
                                BuildingID = (int)reader["BuildingID"],
                                Floor = (int)reader["Floor"],
                                RoomCategoryID = (int)reader["RoomCategoryID"],
                                BuildingName = reader["BuildingName"].ToString(),
                                CategoryName = reader["CategoryName"].ToString()
                            });
                        }
                    }
                }
            }
            return rooms;
        }

        public ObservableCollection<Room> GetRoomsByBuilding(int buildingID)
        {
            var rooms = new ObservableCollection<Room>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT r.RoomID, r.BuildingID, r.Floor, r.RoomCategoryID, 
                               b.BuildingName, rc.CategoryName
                        FROM dbo.Room r
                        INNER JOIN dbo.Building b ON r.BuildingID = b.BuildingID
                        INNER JOIN dbo.RoomCategory rc ON r.RoomCategoryID = rc.RoomCategoryID
                        WHERE r.BuildingID = @buildingID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buildingID", buildingID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rooms.Add(new Room
                            {
                                RoomID = (int)reader["RoomID"],
                                BuildingID = (int)reader["BuildingID"],
                                Floor = (int)reader["Floor"],
                                RoomCategoryID = (int)reader["RoomCategoryID"],
                                BuildingName = reader["BuildingName"].ToString(),
                                CategoryName = reader["CategoryName"].ToString()
                            });
                        }
                    }
                }
            }
            return rooms;
        }

        public void AddRoom(Room room)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.Room (BuildingID, Floor, RoomCategoryID) VALUES (@buildingId, @floor, @categoryId)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@buildingId", room.BuildingID);
                    cmd.Parameters.AddWithValue("@floor", room.Floor);
                    cmd.Parameters.AddWithValue("@categoryId", room.RoomCategoryID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRoom(Room room)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE dbo.Room SET BuildingID = @buildingId, Floor = @floor, RoomCategoryID = @categoryId WHERE RoomID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", room.RoomID);
                    cmd.Parameters.AddWithValue("@buildingId", room.BuildingID);
                    cmd.Parameters.AddWithValue("@floor", room.Floor);
                    cmd.Parameters.AddWithValue("@categoryId", room.RoomCategoryID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRoom(int roomID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.Room WHERE RoomID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", roomID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region EnergyConsumption CRUD

        public ObservableCollection<EnergyConsumption> GetAllEnergyConsumptions()
        {
            var consumptions = new ObservableCollection<EnergyConsumption>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT ec.ConsumptionID, ec.RoomID, ec.[Date], ec.TimePeriodID, ec.EnergyValue,
                               CONCAT(b.BuildingName, ' - Этаж ', r.Floor) as RoomInfo, tp.PeriodName
                        FROM dbo.EnergyConsumption ec
                        INNER JOIN dbo.Room r ON ec.RoomID = r.RoomID
                        INNER JOIN dbo.Building b ON r.BuildingID = b.BuildingID
                        INNER JOIN dbo.TimePeriod tp ON ec.TimePeriodID = tp.TimePeriodID
                        ORDER BY ec.[Date] DESC";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            consumptions.Add(new EnergyConsumption
                            {
                                ConsumptionID = (int)reader["ConsumptionID"],
                                RoomID = (int)reader["RoomID"],
                                Date = (DateTime)reader["Date"],
                                TimePeriodID = (int)reader["TimePeriodID"],
                                EnergyValue = (decimal)reader["EnergyValue"],
                                RoomInfo = reader["RoomInfo"].ToString(),
                                TimePeriodName = reader["PeriodName"].ToString()
                            });
                        }
                    }
                }
            }
            return consumptions;
        }

        public void AddEnergyConsumption(EnergyConsumption consumption)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "INSERT INTO dbo.EnergyConsumption (RoomID, [Date], TimePeriodID, EnergyValue) VALUES (@roomId, @date, @timePeriodId, @energy)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@roomId", consumption.RoomID);
                    cmd.Parameters.AddWithValue("@date", consumption.Date);
                    cmd.Parameters.AddWithValue("@timePeriodId", consumption.TimePeriodID);
                    cmd.Parameters.AddWithValue("@energy", consumption.EnergyValue);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateEnergyConsumption(EnergyConsumption consumption)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE dbo.EnergyConsumption SET RoomID = @roomId, [Date] = @date, TimePeriodID = @timePeriodId, EnergyValue = @energy WHERE ConsumptionID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", consumption.ConsumptionID);
                    cmd.Parameters.AddWithValue("@roomId", consumption.RoomID);
                    cmd.Parameters.AddWithValue("@date", consumption.Date);
                    cmd.Parameters.AddWithValue("@timePeriodId", consumption.TimePeriodID);
                    cmd.Parameters.AddWithValue("@energy", consumption.EnergyValue);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteEnergyConsumption(int consumptionID)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "DELETE FROM dbo.EnergyConsumption WHERE ConsumptionID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", consumptionID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region SQL Query Execution

        public DataTable ExecuteCustomQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.CommandTimeout = 60;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении запроса: {ex.Message}");
            }
            return dt;
        }

        #endregion

        #region Reports

        public DataTable GetEnergyConsumptionReport(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT 
                           b.BuildingName,
                           r.Floor,
                           rc.CategoryName,
                           tp.PeriodName,
                           ec.[Date],
                           ec.EnergyValue,
                           SUM(ec.EnergyValue) OVER (PARTITION BY b.BuildingID, r.RoomID, tp.TimePeriodID ORDER BY ec.[Date]) as CumulativeEnergy
                    FROM dbo.EnergyConsumption ec
                    INNER JOIN dbo.Room r ON ec.RoomID = r.RoomID
                    INNER JOIN dbo.Building b ON r.BuildingID = b.BuildingID
                    INNER JOIN dbo.RoomCategory rc ON r.RoomCategoryID = rc.RoomCategoryID
                    INNER JOIN dbo.TimePeriod tp ON ec.TimePeriodID = tp.TimePeriodID
                    WHERE ec.[Date] BETWEEN @startDate AND @endDate
                    ORDER BY b.BuildingName, r.Floor, ec.[Date]";

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        public DataTable GetBuildingStatistics()
        {
            string query = @"SELECT 
                           b.BuildingID,
                           b.BuildingName,
                           COUNT(DISTINCT r.RoomID) as RoomCount,
                           SUM(ec.EnergyValue) as TotalEnergy,
                           AVG(ec.EnergyValue) as AverageEnergy,
                           MAX(ec.EnergyValue) as MaxEnergy,
                           MIN(ec.EnergyValue) as MinEnergy
                    FROM dbo.Building b
                    LEFT JOIN dbo.Room r ON b.BuildingID = r.BuildingID
                    LEFT JOIN dbo.EnergyConsumption ec ON r.RoomID = ec.RoomID
                    GROUP BY b.BuildingID, b.BuildingName
                    ORDER BY TotalEnergy DESC";

            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        #endregion

        #region User Management

        public User AuthenticateUser(string username, string passwordHash)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = @"SELECT u.UserID, u.Username, u.PasswordHash, u.RoleID, r.RoleName 
                               FROM dbo.[User] u
                               INNER JOIN dbo.Role r ON u.RoleID = r.RoleID
                               WHERE u.Username = @username AND u.PasswordHash = @passwordHash";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = (int)reader["UserID"],
                                Username = reader["Username"].ToString(),
                                PasswordHash = reader["PasswordHash"].ToString(),
                                RoleID = (int)reader["RoleID"],
                                RoleName = reader["RoleName"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool CreateUser(string username, string passwordHash, int roleID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO dbo.[User] (Username, PasswordHash, RoleID) VALUES (@username, @passwordHash, @roleID)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@roleID", roleID);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error in CreateUser: {sqlEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateUser: {ex.Message}");
                return false;
            }
        }

        public bool UserExists(string username)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM dbo.[User] WHERE Username = @username";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        #endregion

        #region Role Management

        public ObservableCollection<Role> GetAllRoles()
        {
            var roles = new ObservableCollection<Role>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT RoleID, RoleName FROM dbo.Role";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Role
                            {
                                RoleID = (int)reader["RoleID"],
                                RoleName = reader["RoleName"].ToString()
                            });
                        }
                    }
                }
            }
            return roles;
        }

        public int GetRoleIDByName(string roleName)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT RoleID FROM dbo.Role WHERE RoleName = @roleName";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@roleName", roleName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? (int)result : 0;
                }
            }
        }

        #endregion

        #region AdminKey Management

        public bool ValidateAdminKey(string keyValue)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM dbo.AdminKey WHERE AdminKeyValue = @keyValue AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyValue", keyValue);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public ObservableCollection<AdminKey> GetAllAdminKeys()
        {
            var keys = new ObservableCollection<AdminKey>();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "SELECT KeyID, AdminKeyValue, IsActive FROM dbo.AdminKey";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object isActiveValue = reader["IsActive"];
                                bool isActive = false;

                                if (isActiveValue != DBNull.Value)
                                {
                                    if (isActiveValue is bool)
                                        isActive = (bool)isActiveValue;
                                    else if (isActiveValue is int)
                                        isActive = (int)isActiveValue != 0;
                                    else if (isActiveValue is byte)
                                        isActive = (byte)isActiveValue != 0;
                                    else
                                        isActive = Convert.ToBoolean(isActiveValue);
                                }

                                var key = new AdminKey
                                {
                                    KeyID = (int)reader["KeyID"],
                                    AdminKeyValue = reader["AdminKeyValue"] != DBNull.Value ? reader["AdminKeyValue"].ToString() : string.Empty,
                                    IsActive = isActive
                                };
                                keys.Add(key);
                                System.Diagnostics.Debug.WriteLine($"Added key: ID={key.KeyID}, Value={key.AdminKeyValue}, Active={key.IsActive}");
                            }
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine($"GetAllAdminKeys: Total keys loaded = {keys.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading admin keys: {ex.Message}\n{ex.StackTrace}");
            }
            return keys;
        }

        public bool AddAdminKey(string keyValue)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO dbo.AdminKey (AdminKeyValue, IsActive) VALUES (@keyValue, 1)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyValue", keyValue);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeactivateAdminKey(int keyID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE dbo.AdminKey SET IsActive = 0 WHERE KeyID = @keyID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyID", keyID);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAdminKey(int keyID)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM dbo.AdminKey WHERE KeyID = @keyID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyID", keyID);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
