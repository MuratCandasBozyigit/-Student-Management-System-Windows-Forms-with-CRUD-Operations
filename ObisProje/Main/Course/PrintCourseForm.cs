using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObisProjem.Course
{
    public partial class PrintCourseForm : Form
    {
        public PrintCourseForm()
        {
            InitializeComponent();
        }

        private void PrintCourseForm_Load(object sender, EventArgs e)
        {
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            sql.Open();
            SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM Course_Table", sql);//database e veriler çekiliyor.
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            sql.Close();
            dataGridView1.DataSource = dataTable;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Kullanıcı belgeler dizinini almak
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Dosya yolu
            string filePath = Path.Combine(documentsPath, "Course_Print.txt");

            // Dosyanın var olup olmadığını kontrol et
            if (!File.Exists(filePath))
            {
                // Dosya yoksa, dosyayı oluştur
                File.Create(filePath).Close();
            }

            // Dosyayı yazma işlemine başla
            using (TextWriter yaz = new StreamWriter(filePath))
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    yaz.Write("    " + dataGridView1.Rows[i].Cells[0].Value.ToString() + "\t" + "|");
                    yaz.Write("    " + dataGridView1.Rows[i].Cells[1].Value.ToString() + "\t" + "|");
                    yaz.Write("    " + dataGridView1.Rows[i].Cells[2].Value.ToString() + "\t" + "|");
                    yaz.Write("    " + dataGridView1.Rows[i].Cells[3].Value.ToString() + "     \t|");

                    yaz.WriteLine("");
                    yaz.Write("-----------------------------------------------------------------");
                    yaz.WriteLine("");
                }
            }
            MessageBox.Show("Data Exported");
        }


    }
}
