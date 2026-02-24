using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace ThreadGeneratorApp
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;

        public DatabaseService()
        {
            // Update this connection string to match your SQL Server configuration
            _connectionString = "Server=localhost\\SQLEXPRESS;Database=ThreadGeneratorDB;Trusted_Connection=true;TrustServerCertificate=True;";
            _masterConnectionString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=true;TrustServerCertificate=True;";
        }

        public async Task InitializeDatabaseAsync()
        {
            using (var masterConnection = new SqlConnection(_masterConnectionString))
            {
                await masterConnection.OpenAsync();

                // Create database if it doesn't exist
                string createDbQuery = @"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ThreadGeneratorDB')
                        BEGIN
                            CREATE DATABASE ThreadGeneratorDB;
                        END";

                using (var command = new SqlCommand(createDbQuery, masterConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }

            // Connect to the specific database and create table
            using (var appConnection = new SqlConnection(_connectionString))
            {
                await appConnection.OpenAsync();

                string createTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ThreadResults' AND xtype='U')
                        BEGIN
                            CREATE TABLE ThreadResults (
                                ID INT IDENTITY(1,1) PRIMARY KEY,
                                ThreadID INT NOT NULL,
                                [Time] DATETIME NOT NULL,
                                Data NVARCHAR(100) NOT NULL
                            );
                        END";

                using (var command = new SqlCommand(createTableQuery, appConnection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task SaveResultAsync(ThreadResult result)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    string insertQuery = @"
                        INSERT INTO ThreadResults (ThreadID, [Time], Data)
                        VALUES (@ThreadID, @Time, @Data)";

                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ThreadID", result.ThreadId);
                        command.Parameters.AddWithValue("@Time", result.Timestamp);
                        command.Parameters.AddWithValue("@Data", result.GeneratedString);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid stopping threads
                Console.WriteLine($"Database save error: {ex.Message}");
            }
        }
    }
}
