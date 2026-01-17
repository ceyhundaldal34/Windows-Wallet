
using System;
using System.Data.SQLite; // SQLite 
using System.IO;

namespace Windows_Wallet__Final_Ödevi_
{
    public class DatabaseHelper
    {
        // Veritabanı dosyamızın adı
        private static string dbFile = "cuzdan.db";
        // Bağlantı cümlesi (Connection String)
        private static string connectionString = $"Data Source={dbFile};Version=3;";

        // Bu metot veritabanı dosyasını ve tabloları oluşturur
        public static void InitializeDatabase()
        {
            // Eğer dosya yoksa, SQLite otomatik oluşturur ama biz kontrol edelim
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // 1. KULLANICILAR TABLOSU (Users)
                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL,
                        Email TEXT,
                        Balance REAL DEFAULT 0,
                        CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                // 2. İŞLEMLER TABLOSU (Transactions)
                // Hangi kullanıcı ne kadar para yatırdı/çekti?
                string createTransactionsTable = @"
                    CREATE TABLE IF NOT EXISTS Transactions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        Amount REAL NOT NULL,
                        Description TEXT,
                        TransactionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    );";

                // Komutları çalıştır
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createUsersTable;
                    command.ExecuteNonQuery();

                    command.CommandText = createTransactionsTable;
                    command.ExecuteNonQuery();
                }
            }
        }

        // Diğer sayfalardan veritabanına bağlanmak istediğimizde bu metodu çağıracağız
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}