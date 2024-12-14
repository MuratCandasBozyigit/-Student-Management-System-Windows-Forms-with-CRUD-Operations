using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObisProjem.Score
{
    public partial class ManageScoresForm : Form
    {
        public ManageScoresForm()
        {
            InitializeComponent();
        }

        AvgScoreByCourse AvgScoreBy = new AvgScoreByCourse();
        private void button4_Click(object sender, EventArgs e)
        {
            AvgScoreBy.ShowDialog();
        }

        private void ManageScoresForm_Load(object sender, EventArgs e)
        {
          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Course_Table", sql);//database e veriler çekiliyor.
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                comboBox2.Items.Add(dataTable.Rows[i][1]);
            }
        }

        int rowindex;
        string button;

        private void button3_Click(object sender, EventArgs e)
        {
            if (button == "score")
            {
              SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

                SqlCommand sqlCommand = new SqlCommand("DELETE from Score_Table WHERE student_id=@student_id AND score_course=@score_course", sql);
                sql.Open();
                sqlCommand.Parameters.AddWithValue("@student_id", studentListData.Rows[rowindex].Cells[0].Value);
                sqlCommand.Parameters.AddWithValue("@score_course", studentListData.Rows[rowindex].Cells[4].Value);
                sqlCommand.ExecuteNonQuery();
                sql.Close();
                studentListData.Rows.RemoveAt(rowindex);
            }
            else
                MessageBox.Show("Not Bulunamadı");
        }

        private void AddScore_Click(object sender, EventArgs e)
        {
            // Boş alan kontrolü için liste
            List<string> emptyFields = new List<string>();

            if (string.IsNullOrWhiteSpace(comboBox2.Text))
                emptyFields.Add("Ders Adı");
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                emptyFields.Add("Öğrenci ID");
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                emptyFields.Add("Not 1");
            if (string.IsNullOrWhiteSpace(textBox3.Text))
                emptyFields.Add("Not 2");
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
                emptyFields.Add("Açıklama");

            // Eğer tüm alanlar boşsa işlemi durdur
            if (emptyFields.Count == 5)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Veritabanı bağlantısı
            SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            bool readerHasRows = false;

            string commandQuery = "SELECT score_course, student_id FROM Score_Table WHERE score_course = @score_course AND student_id = @student_id";
            using (SqlCommand cmd = new SqlCommand(commandQuery, sql))
            {
                sql.Open();
                cmd.Parameters.AddWithValue("@score_course", comboBox2.Text);
                cmd.Parameters.AddWithValue("@student_id", int.Parse(textBox2.Text));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    readerHasRows = (reader != null && reader.HasRows);
                }
                sql.Close();
            }

            if (readerHasRows)
            {
                MessageBox.Show("Ders Zaten Kayıtlı!!");
            }
            else
            {
                // Notları veritabanına ekle
                SqlCommand command = new SqlCommand("INSERT INTO Score_Table (student_id, score_course, score, score1, score_desc) VALUES (@student_id, @score_course, @score, @score1, @score_desc)", sql);
                sql.Open();
                command.Parameters.AddWithValue("@student_id", int.Parse(textBox2.Text));
                command.Parameters.AddWithValue("@score_course", comboBox2.Text);
                command.Parameters.AddWithValue("@score", string.IsNullOrWhiteSpace(textBox1.Text) ? (object)DBNull.Value : textBox1.Text);
                command.Parameters.AddWithValue("@score1", string.IsNullOrWhiteSpace(textBox3.Text) ? (object)DBNull.Value : textBox3.Text);
                command.Parameters.AddWithValue("@score_desc", string.IsNullOrWhiteSpace(richTextBox1.Text) ? (object)DBNull.Value : richTextBox1.Text);
                command.ExecuteNonQuery();
                sql.Close();

                // Uyarı mesajı: Hangi alanlar boş bırakıldı?
                if (emptyFields.Count > 0)
                {
                    string emptyFieldMessage = string.Join(", ", emptyFields);
                    MessageBox.Show($"Bazı alanlar boş bırakıldı: {emptyFieldMessage}. Ancak bilgiler başarıyla eklendi!", "Bilgilendirme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Yeni Not Eklendi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Ortalama notu hesapla ve güncelle
                UpdateAverageScore(int.Parse(textBox2.Text), comboBox2.Text);
            }

            // Formu temizle
            textBox2.Clear();
            textBox1.Clear();
            textBox3.Clear();
            comboBox2.Text = "";
            richTextBox1.Clear();
        }

        private void UpdateAverageScore(int studentId, string scoreCourse)
        {
          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            // Notları al
            string query = "SELECT score, score1 FROM Score_Table WHERE student_id = @student_id AND score_course = @score_course";
            using (SqlCommand cmd = new SqlCommand(query, sql))
            {
                sql.Open();
                cmd.Parameters.AddWithValue("@student_id", studentId);
                cmd.Parameters.AddWithValue("@score_course", scoreCourse);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Verilerin tipi kontrol edilerek alınır
                        float score = 0;
                        float score1 = 0;

                        if (!reader.IsDBNull(0))
                        {
                            score = reader.GetFloat(0);
                        }

                        if (!reader.IsDBNull(1))
                        {
                            score1 = reader.GetFloat(1);
                        }

                        // Ortalama hesapla
                        float averageScore = (score + score1) / 2;

                        // Ortalama notu güncelle
                        string updateQuery = "UPDATE Score_Table SET average_score = @average_score WHERE student_id = @student_id AND score_course = @score_course";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, sql))
                        {
                            updateCmd.Parameters.AddWithValue("@average_score", averageScore);
                            updateCmd.Parameters.AddWithValue("@student_id", studentId);
                            updateCmd.Parameters.AddWithValue("@score_course", scoreCourse);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
                sql.Close();
            }
        }


        private void studentListData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBox2.Text = studentListData.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
            catch
            {
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button = "score";
          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            SqlDataAdapter da = new SqlDataAdapter("SELECT Student_Table.student_id , Student_Table.student_firstname , Student_Table.student_lastname , Course_Table.course_id , Score_Table.score_course ,Score_Table.score1, Score_Table.score FROM Student_Table,Course_Table,Score_Table where Student_Table.student_id=Score_Table.student_id And Score_Table.score_course=Course_Table.course_label", sql);//database e veriler çekiliyor.
            DataTable dataTable = new DataTable();
            sql.Open();
            da.Fill(dataTable);
            sql.Close();
            studentListData.DataSource = dataTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button = "student";
          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            SqlDataAdapter da = new SqlDataAdapter("SELECT student_id , student_firstname , student_lastname , student_birthdate FROM Student_Table", sql);//database e veriler çekiliyor.
          

            DataTable dataTable = new DataTable();
            sql.Open();
            da.Fill(dataTable);
            sql.Close();
            studentListData.DataSource = dataTable;
        }

        private void studentListData_Click(object sender, EventArgs e)
        {
            rowsSelect();
        }

        private void rowShow(int i)
        {
            textBox2.Text = studentListData.Rows[i].Cells[0].Value.ToString();
        }

        private void rowsSelect()
        {
            for (int i = 0; i < studentListData.Rows.Count; i++)
            {
                if (studentListData.Rows[i].Selected == true)
                    rowShow(i);

            }
        }

        private void studentListData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            rowindex = e.RowIndex;
            textBox2.Text = studentListData.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void HoursNumber_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
