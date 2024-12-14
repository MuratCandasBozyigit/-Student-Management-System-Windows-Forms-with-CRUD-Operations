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
    public partial class AddCourseForm : Form
    {
        public AddCourseForm()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Boş alan kontrolü
            if (string.IsNullOrEmpty(textLabel.Text) || hourseNumber.Value == 0 || string.IsNullOrEmpty(desciption.Text))
            {
                MessageBox.Show("Boş alan bırakmayınız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Eğer herhangi bir alan boşsa işlem yapılmaz
            }

            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            // SQL komutunu parametreli hale getirelim, SQL injection'a karşı koruma sağlansın
            SqlCommand command = new SqlCommand("insert into Course_Table (course_label, hourse_number, course_desciption) values (@course_label, @hourse_number, @course_desciption)", sql);
            command.Parameters.AddWithValue("@course_label", textLabel.Text);
            command.Parameters.AddWithValue("@hourse_number", hourseNumber.Value);
            command.Parameters.AddWithValue("@course_desciption", desciption.Text);

            try
            {
                sql.Open();
                command.ExecuteNonQuery();
                sql.Close();
                MessageBox.Show("Yeni Ders Eklendi");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }

            // Formu temizle
            textLabel.Clear();
            hourseNumber.Value = 0;
            desciption.Clear();
        }


        private void AddCourseForm_Load(object sender, EventArgs e)
        {

        }

        private void desciption_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
