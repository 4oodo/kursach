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
            // Используем хардкодированную строку подключения вместо ConfigurationManager
            // Для .NET Framework 4.7.2 - используем минимальный набор параметров
            _connectionString = "Data Source=DESKTOP-N513RVN;Initial Catalog=EnergyManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False";

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
    }
}
