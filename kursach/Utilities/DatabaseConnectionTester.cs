using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace kursach.Utilities
{
    /// <summary>
    /// Вспомогательный класс для проверки подключения к базе данных
    /// </summary>
    public class DatabaseConnectionTester
    {
        private readonly string _connectionString;

        public DatabaseConnectionTester(string connectionString)
        {
            _connectionString = connectionString;
            // Добавляем обработчик для проверки SSL-сертификата
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Доверяем всем сертификатам (включая самоподписанные)
            // ВНИМАНИЕ: Это небезопасно для production! Используйте только для разработки.
            return true;
        }

        /// <summary>
        /// Проверяет подключение к базе данных
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Подключение к БД успешно!");

                    // Проверяем наличие таблиц
                    SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'dbo' AND table_name = 'Building'",
                        conn);

                    int result = (int)cmd.ExecuteScalar();
                    if (result > 0)
                    {
                        Console.WriteLine("Таблица Building найдена!");
                    }
                    else
                    {
                        Console.WriteLine("Таблица Building не найдена! Запустите Database_Setup.sql");
                        return false;
                    }

                    return true;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
                return false;
            }
        }
    }
}
