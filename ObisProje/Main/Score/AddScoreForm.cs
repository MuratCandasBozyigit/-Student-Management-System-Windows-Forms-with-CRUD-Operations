using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ObisProjem.Score
{
    public partial class AddScoreForm : Form
    {
        public AddScoreForm()
        {
            InitializeComponent();
        }

        // Form'un yüklenmesinde ComboBox'ı doldurun
        private void AddScoreForm_Load(object sender, EventArgs e)
        {
            // Dersleri ComboBox2'ye ekle
            comboBox2.Items.Clear();
          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            // Course_Table'dan verileri çek
            DataTable dataTable = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Course_Table", sql);
            sql.Open();
            da.Fill(dataTable);
            sql.Close();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                comboBox2.Items.Add(dataTable.Rows[i][1]); // Ders ismini ComboBox'a ekle
            }

            // Öğrencileri DataGridView'e ekle
            DataTable dataTable1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT student_id, student_firstname, student_lastname FROM Student_Table", sql);
            sql.Open();
            da1.Fill(dataTable1);
            sql.Close();
            studentListData.DataSource = dataTable1;

            // Sınav türlerini ComboBox1'e ekle
            comboBox1.Items.Clear();
            comboBox1.Items.Add("1. Sınav");
            comboBox1.Items.Add("2. Sınav");

        }

        private void AddScore_Click(object sender, EventArgs e)
        {
            // Boş alan kontrolü
            if (string.IsNullOrWhiteSpace(comboBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(richTextBox1.Text) ||
                comboBox1.SelectedIndex == -1) // Sınav türü seçimi kontrolü
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Geçerli bir sayı girilip girilmediğini kontrol et
            if (!int.TryParse(textBox1.Text, out int score))
            {
                MessageBox.Show("Lütfen geçerli bir not girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

          SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            // Öğrenci numarasını kontrol et
            if (!int.TryParse(textBox2.Text, out int studentId))
            {
                MessageBox.Show("Lütfen geçerli bir öğrenci numarası girin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Not ekleme işlemi
            string insertQuery = "INSERT INTO Score_Table (student_id, score_course, score, score_desc) VALUES (@student_id, @score_course, @score, @score_desc)";

            // Sınav türüne göre sorguyu ayarlama
            if (comboBox1.SelectedItem.ToString() == "2. Sınav")
            {
                insertQuery = "INSERT INTO Score_Table (student_id, score_course, score1, score_desc) VALUES (@student_id, @score_course, @score1, @score_desc)";
            }

            using (SqlCommand command = new SqlCommand(insertQuery, sql))
            {
                sql.Open();
                command.Parameters.AddWithValue("@student_id", studentId);
                command.Parameters.AddWithValue("@score_course", comboBox2.Text); // Ders adı

                // Sınav türüne göre notu ekleme
                if (comboBox1.SelectedItem.ToString() == "1. Sınav")
                {
                    command.Parameters.AddWithValue("@score", score); // 1. sınav notu
                    command.Parameters.AddWithValue("@score1", DBNull.Value); // 2. sınav notu boş
                }
                else if (comboBox1.SelectedItem.ToString() == "2. Sınav")
                {
                    command.Parameters.AddWithValue("@score1", score); // 2. sınav notu
                    command.Parameters.AddWithValue("@score", DBNull.Value); // 1. sınav notu boş
                }

                command.Parameters.AddWithValue("@score_desc", richTextBox1.Text); // Açıklama ekle

                command.ExecuteNonQuery();
                sql.Close();
                MessageBox.Show("Yeni Not Eklendi");
            }

            // Formu temizleme
            textBox2.Clear();
            textBox1.Clear();
            comboBox2.Text = "";
            richTextBox1.Clear();
            comboBox1.SelectedIndex = -1; // Başlangıçta hiçbir şey seçilmesin
        }

        private void studentListData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox2.Text = studentListData.Rows[e.RowIndex].Cells[0].Value.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Sınav türü seçildiğinde yapılacak bir işlem varsa buraya eklenebilir.
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
