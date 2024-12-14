using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ObisProjem.Score
{
    public partial class AvgScoreByCourse : Form
    {
        public AvgScoreByCourse()
        {
            InitializeComponent();
        }

        private void AvgScoreByCourse_Load(object sender, EventArgs e)
        {
            SqlConnection sql = new SqlConnection("Server=217.195.207.215\\MSSQLSERVER2019;Database=dunyani1_obs;User Id=obs;Password=2013061Murat;TrustServerCertificate=True;");

            // Dersleri çekme
            SqlDataAdapter da = new SqlDataAdapter("SELECT course_label FROM Course_Table", sql);
            DataTable dataTable = new DataTable(); // Dersler
            sql.Open();
            da.Fill(dataTable);
            sql.Close();

            // Ders seçilmediği kontrolü
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen bir ders seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Ders seçilmeden işlem yapılmasın
            }

            // Notları ve dersleri çekme
            SqlDataAdapter da1 = new SqlDataAdapter("SELECT Score_Table.score_course, Score_Table.score, Score_Table.score1 FROM Student_Table, Score_Table WHERE Student_Table.student_id = Score_Table.student_id", sql);
            DataTable dt = new DataTable(); // Notlar
            sql.Open();
            da1.Fill(dt);
            sql.Close();

            // Ortalama hesaplama
            foreach (DataRow item in dataTable.Rows)
            {
                float average = 0;
                int count = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (item[0].ToString().Trim() == dt.Rows[i][0].ToString().Trim())
                    {
                        // 1. sınav notunu kontrol et
                        if (dt.Rows[i][1] != DBNull.Value)
                        {
                            average += float.Parse(dt.Rows[i][1].ToString());
                            count++;
                        }

                        // 2. sınav notunu kontrol et
                        if (dt.Rows[i][2] != DBNull.Value)
                        {
                            average += float.Parse(dt.Rows[i][2].ToString());
                            count++;
                        }
                    }
                }

                // Ortalama hesapla
                if (count > 0)
                {
                    average /= count;
                    studentListData.Rows.Add(item[0], average.ToString("F2")); // İki ondalık basamakla göster
                }
                else
                {
                    studentListData.Rows.Add(item[0], "0.00"); // Eğer not yoksa 0.00 göster
                }
            }
        }

        private void studentListData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Hücre tıklama olayları burada işlenebilir
        }
    }
}
