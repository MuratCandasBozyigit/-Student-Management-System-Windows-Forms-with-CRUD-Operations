using ObisProjem;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace ObisProje
{
    static class Program
    {
        private static readonly string ConnectionString = "Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;";

        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                EnsureDatabaseExists(connection);
                EnsureTablesExist(connection);
                SeedInitialData(connection);
            }

            girisForm giris;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(giris = new girisForm());

            if (giris.DialogResult == DialogResult.OK)
            {
                Application.Run(new anaForm());
                MessageBox.Show("Başarıyla çalıştı.");
            }
        }

        private static void EnsureDatabaseExists(SqlConnection connection)
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM master.sys.databases WHERE name='dunyani1_obs'", connection))
            {
                int databaseExists = (int)command.ExecuteScalar();
                if (databaseExists == 0)
                {
                    using (SqlCommand createDbCommand = new SqlCommand("CREATE DATABASE dunyani1_obs", connection))
                    {
                        createDbCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void EnsureTablesExist(SqlConnection connection)
        {
            string[] tableCommands = new string[] {
                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='User_Table' AND xtype='U') " +
                "CREATE TABLE User_Table ([user] NVARCHAR(20) NOT NULL, [pass] NCHAR(16) NOT NULL)",

                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Student_Table' AND xtype='U') " +
                "CREATE TABLE Student_Table ([student_id] INT NOT NULL PRIMARY KEY IDENTITY, [student_firstname] NVARCHAR(30) NOT NULL, [student_lastname] NVARCHAR(40) NOT NULL, [student_birthdate] DATE NOT NULL, [student_gender] NCHAR(6) NOT NULL, [student_phone] NCHAR(10) NOT NULL, [student_address] NVARCHAR(300) NOT NULL, [student_picture] IMAGE NOT NULL)",

                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Course_Table' AND xtype='U') " +
                "CREATE TABLE Course_Table ([course_id] INT NOT NULL PRIMARY KEY IDENTITY, [course_label] NCHAR(50) NOT NULL, [hourse_number] INT NOT NULL, [course_desciption] NVARCHAR(100) NOT NULL)",

                "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Score_Table' AND xtype='U') " +
                "CREATE TABLE Score_Table ([student_id] INT NOT NULL, [score_course] NVARCHAR(50) NOT NULL, [score] INT  NULL, [score1] INT  NULL, [score_desc] NVARCHAR(50) NULL)"
            };

            foreach (var commandText in tableCommands)
            {
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedInitialData(SqlConnection connection)
        {
            // Add initial admin user
            using (SqlCommand userAdd = new SqlCommand("IF NOT EXISTS (SELECT * FROM User_Table WHERE [user] = @user) INSERT INTO User_Table ([user], [pass]) VALUES (@user, @pass)", connection))
            {
                userAdd.Parameters.AddWithValue("@user", "admin");
                userAdd.Parameters.AddWithValue("@pass", "admin");
                userAdd.ExecuteNonQuery();
            }

            // Sample data seeding (commented out for now)
            //studentAdd("Murat", "Bayraktar", "1995-01-01", "Male", "5343889147", "İstanbul/Avcılar", "Pictures\\Tarık Akan.jpg", connection);
            //courseAdd("Görsel Programlama", 6, "Elham Pashaei", connection);
            //scoreAdd(1, "Görsel Programlama", 80, 90, "OK", connection);
        }

        private static void studentAdd(string name, string lastname, string birthdate, string gender, string phone, string address, string url, SqlConnection connection)
        {
            byte[] images;
            using (FileStream stream = new FileStream(url, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader brs = new BinaryReader(stream))
                {
                    images = brs.ReadBytes((int)stream.Length);
                }
            }

            using (SqlCommand studentAdd = new SqlCommand(
                "INSERT INTO Student_Table (student_firstname, student_lastname, student_birthdate, student_gender, student_phone, student_address, student_picture) " +
                "VALUES (@name, @lastname, @birthdate, @gender, @phone, @address, @picture)", connection))
            {
                studentAdd.Parameters.AddWithValue("@name", name);
                studentAdd.Parameters.AddWithValue("@lastname", lastname);
                studentAdd.Parameters.AddWithValue("@birthdate", birthdate);
                studentAdd.Parameters.AddWithValue("@gender", gender);
                studentAdd.Parameters.AddWithValue("@phone", phone);
                studentAdd.Parameters.AddWithValue("@address", address);
                studentAdd.Parameters.AddWithValue("@picture", images);
                studentAdd.ExecuteNonQuery();
            }
        }

        private static void courseAdd(string label, int number, string desc, SqlConnection connection)
        {
            using (SqlCommand courseAdd = new SqlCommand(
                "INSERT INTO Course_Table (course_label, hourse_number, course_desciption) " +
                "VALUES (@label, @number, @desc)", connection))
            {
                courseAdd.Parameters.AddWithValue("@label", label);
                courseAdd.Parameters.AddWithValue("@number", number);
                courseAdd.Parameters.AddWithValue("@desc", desc);
                courseAdd.ExecuteNonQuery();
            }
        }

        private static void scoreAdd(int studentId, string courseLabel, int score, int score1, string description, SqlConnection connection)
        {
            using (SqlCommand scoreAdd = new SqlCommand(
                "INSERT INTO Score_Table (student_id, score_course, score, score1, score_desc) " +
                "VALUES (@studentId, @courseLabel, @score, @score1, @description)", connection))
            {
                scoreAdd.Parameters.AddWithValue("@studentId", studentId);
                scoreAdd.Parameters.AddWithValue("@courseLabel", courseLabel);
                scoreAdd.Parameters.AddWithValue("@score", score);
                scoreAdd.Parameters.AddWithValue("@score1", score1);
                scoreAdd.Parameters.AddWithValue("@description", description);
                scoreAdd.ExecuteNonQuery();
            }
        }

        // Notlar ve durum hesaplama
        private static void calculateStatus(int studentId, string courseLabel, SqlConnection connection)
        {
            string query = "SELECT (score + score1) / 2 AS average_score, " +
                           "CASE WHEN (score + score1) / 2 >= 50 THEN 'Geçti' ELSE 'Kaldı' END AS status " +
                           "FROM Score_Table WHERE student_id = @studentId AND score_course = @courseLabel";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@courseLabel", courseLabel);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string status = reader["status"].ToString();
                        MessageBox.Show("Durum: " + status);
                    }
                }
            }
        }
    }
}
