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

namespace ObisProjem.Course
{
    public partial class ManageCoursesForm : Form
    {
        public ManageCoursesForm()
        {
            InitializeComponent();
        }

        private void ManageCoursesForm_Load(object sender, EventArgs e)
        {
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            SqlCommand sqlCommand = new SqlCommand("DELETE from Course_Table where course_id=@course_id", sql);
            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Course_Table", sql);

            sql.Open();
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            sql.Close();

            foreach (DataRow item in dataTable.Rows)
            {
                listBox1.Items.Add(item[1]);
            }

            TotalCourse.Text = "Toplam Dersler: " + listBox1.Items.Count;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Course_Table", sql);

            sql.Open();
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            sql.Close();

            foreach (DataRow item in dataTable.Rows)
            {
                if (listBox1.Text == item[1].ToString())
                {
                    idText.Text = item[0].ToString();
                    labelText.Text = item[1].ToString();
                    numericUpDown1.Value = int.Parse(item[2].ToString());
                    richTextBox1.Text = item[3].ToString();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan gelen alanları kontrol ediyoruz.
            if (string.IsNullOrWhiteSpace(labelText.Text) ||
                numericUpDown1.Value == 0 ||
                string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurun. Hiçbir alan boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Alanlar eksikse işlemi sonlandırıyoruz.
            }

            // SQL bağlantısını oluşturuyoruz.
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            // Kullanıcıdan gelen verileri ekliyoruz.
            SqlCommand command = new SqlCommand(
                "INSERT INTO Course_Table (course_label, hourse_number, course_desciption) " +
                "VALUES (@label, @number, @description)", sql);

            // SQL parametrelerini tanımlıyoruz.
            command.Parameters.AddWithValue("@label", labelText.Text.Trim());
            command.Parameters.AddWithValue("@number", numericUpDown1.Value);
            command.Parameters.AddWithValue("@description", richTextBox1.Text.Trim());

            try
            {
                sql.Open();
                command.ExecuteNonQuery();
                sql.Close();

                MessageBox.Show("Yeni ders başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya detaylı olarak gösteriyoruz.
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Alanları temizliyoruz ve listeyi yeniliyoruz.
            labelText.Clear();
            numericUpDown1.Value = 0;
            richTextBox1.Clear();
            listBox1.Items.Clear();
            ManageCoursesForm_Load(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Ders seçilip seçilmediğini kontrol ediyoruz.
            if (string.IsNullOrWhiteSpace(idText.Text))
            {
                MessageBox.Show("Lütfen önce bir ders seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Seçim yapılmadıysa işlem sonlandırılır.
            }

            // SQL bağlantısını oluşturuyoruz.
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            try
            {
                sql.Open();

                // SQL sorgusunu güncellenmiş parametrelerle yazıyoruz.
                SqlCommand komut = new SqlCommand(
                    "UPDATE Course_Table SET course_label = @courseLabel, hourse_number = @hourseNumber, course_desciption = @courseDescription " +
                    "WHERE course_id = @courseId", sql);

                // Parametreleri ekliyoruz.
                komut.Parameters.AddWithValue("@courseId", idText.Text.Trim());
                komut.Parameters.AddWithValue("@courseLabel", labelText.Text.Trim());
                komut.Parameters.AddWithValue("@hourseNumber", numericUpDown1.Value);
                komut.Parameters.AddWithValue("@courseDescription", richTextBox1.Text.Trim());

                komut.ExecuteNonQuery();

                MessageBox.Show("Ders başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sql.Close();
            }

            // Alanları temizliyoruz ve listeyi yeniliyoruz.
            listBox1.Items.Clear();
            idText.Clear();
            labelText.Clear();
            numericUpDown1.Value = 0;
            richTextBox1.Clear();
            ManageCoursesForm_Load(sender, e);
        }


        int index = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            index = listBox1.Items.Count - 1;
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            index = 0;
            listBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (index < listBox1.Items.Count - 1)
            {
                index++;
                listBox1.SelectedIndex = index;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (index > 0)
            {
                index--;
                listBox1.SelectedIndex = index;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // idText boş veya null ise hata mesajı göster
            if (string.IsNullOrEmpty(idText.Text))
            {
                MessageBox.Show("Lütfen bir ders seçiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Eğer id boşsa, işlem yapılmaz
            }

            DialogResult result1 = MessageBox.Show("Are you sure?", "Warning!", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

                SqlCommand sqlCommand = new SqlCommand("DELETE from Course_Table where course_id=@Course_id", sql);
                sql.Open();
                sqlCommand.Parameters.AddWithValue("@Course_id", int.Parse(idText.Text)); // idText'yi int'e çevir
                sqlCommand.ExecuteNonQuery();
                sql.Close();

                // Formu temizle
                listBox1.Items.Clear();
                idText.Clear();
                labelText.Clear();
                numericUpDown1.Value = 0;
                richTextBox1.Clear();
                ManageCoursesForm_Load(sender, e);
            }
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LabelAddCourse_Click(object sender, EventArgs e)
        {

        }
    }
}
