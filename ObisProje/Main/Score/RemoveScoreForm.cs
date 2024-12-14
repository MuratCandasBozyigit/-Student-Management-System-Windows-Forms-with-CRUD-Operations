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
    public partial class RemoveScoreForm : Form
    {
        public RemoveScoreForm()
        {
            InitializeComponent();
        }

        private void RemoveScoreForm_Load(object sender, EventArgs e)
        {
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            SqlDataAdapter da1 = new SqlDataAdapter(
     "SELECT Student_Table.student_id, " +
     "Student_Table.student_firstname, " +
     "Student_Table.student_lastname, " +
     "Score_Table.student_id AS score_student_id, " +
     "Score_Table.score_course, " +
     "Score_Table.score, " +
     "Score_Table.score1 " +
     "FROM Student_Table " +
     "JOIN Score_Table ON Student_Table.student_id = Score_Table.student_id", sql);

            // DataTable oluşturuluyor
            DataTable ds = new DataTable();

            // Bağlantı açılıyor ve veriler çekiliyor
            sql.Open();
            da1.Fill(ds);
            sql.Close();

            studentListData.DataSource = ds;        
        }

        int rowindex;
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // Seçili satır kontrolü
            if (studentListData.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir not seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Hiçbir şey seçilmemişse işlem yapılmasın
            }

            int rowindex = studentListData.SelectedRows[0].Index; // Seçilen satırın indexini alıyoruz

            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            SqlCommand sqlCommand = new SqlCommand("DELETE from Score_Table WHERE student_id=@student_id AND score_course=@score_course", sql);
            sql.Open();
            sqlCommand.Parameters.AddWithValue("@student_id", studentListData.Rows[rowindex].Cells[0].Value);
            sqlCommand.Parameters.AddWithValue("@score_course", studentListData.Rows[rowindex].Cells[4].Value);
            sqlCommand.ExecuteNonQuery();
            sql.Close();

            // Seçili satırı DataGridView'den kaldırma
            studentListData.Rows.RemoveAt(rowindex);
        }


        private void studentListData_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            rowindex = e.RowIndex;
        }

        private void studentListData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
