using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Course_Table", sql); // Fetching data from the database
            DataTable dataTable = new DataTable();
            da.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                comboBox2.Items.Add(dataTable.Rows[i][1]);
            }
        }

        int rowIndex;
        string button;

        private void button3_Click(object sender, EventArgs e)
        {
            if (button == "score")
            {
                SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

                SqlCommand sqlCommand = new SqlCommand("DELETE FROM Score_Table WHERE student_id=@student_id AND score_course=@score_course", sql);
                sql.Open();
                sqlCommand.Parameters.AddWithValue("@student_id", studentListData.Rows[rowIndex].Cells[0].Value);
                sqlCommand.Parameters.AddWithValue("@score_course", studentListData.Rows[rowIndex].Cells[4].Value);
                sqlCommand.ExecuteNonQuery();
                sql.Close();
                studentListData.Rows.RemoveAt(rowIndex);
            }
            else
            {
                MessageBox.Show("Score Not Found");
            }
        }

        private void AddScore_Click(object sender, EventArgs e)
        {
            // List for empty field checks
            List<string> emptyFields = new List<string>();

            if (string.IsNullOrWhiteSpace(comboBox2.Text))
                emptyFields.Add("Course Name");
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                emptyFields.Add("Student ID");
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                emptyFields.Add("Score 1");
            if (string.IsNullOrWhiteSpace(textBox3.Text))
                emptyFields.Add("Score 2");
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
                emptyFields.Add("Description");

            // Stop the process if all fields are empty
            if (emptyFields.Count == 5)
            {
                MessageBox.Show("Please fill in all the fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Database connection
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
                MessageBox.Show("Course Already Registered!");
            }
            else
            {
                // Add scores to the database
                SqlCommand command = new SqlCommand("INSERT INTO Score_Table (student_id, score_course, score, score1, score_desc) VALUES (@student_id, @score_course, @score, @score1, @score_desc)", sql);
                sql.Open();
                command.Parameters.AddWithValue("@student_id", int.Parse(textBox2.Text));
                command.Parameters.AddWithValue("@score_course", comboBox2.Text);
                command.Parameters.AddWithValue("@score", string.IsNullOrWhiteSpace(textBox1.Text) ? (object)DBNull.Value : textBox1.Text);
                command.Parameters.AddWithValue("@score1", string.IsNullOrWhiteSpace(textBox3.Text) ? (object)DBNull.Value : textBox3.Text);
                command.Parameters.AddWithValue("@score_desc", string.IsNullOrWhiteSpace(richTextBox1.Text) ? (object)DBNull.Value : richTextBox1.Text);
                command.ExecuteNonQuery();
                sql.Close();

                // Warning message: Which fields were left empty?
                if (emptyFields.Count > 0)
                {
                    string emptyFieldMessage = string.Join(", ", emptyFields);
                    MessageBox.Show($"Some fields were left empty: {emptyFieldMessage}. However, the information was successfully added!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("New Score Added", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Calculate and update average score
                UpdateAverageScore(int.Parse(textBox2.Text), comboBox2.Text);
            }

            // Clear the form
            textBox2.Clear();
            textBox1.Clear();
            textBox3.Clear();
            comboBox2.Text = "";
            richTextBox1.Clear();
        }

        private void UpdateAverageScore(int studentId, string scoreCourse)
        {
            SqlConnection sql = new SqlConnection("Server=.\\MSSQLSERVER2019;Database=master;User Id=;Password=;TrustServerCertificate=True;");

            // Fetch scores
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
                        // Fetch data with type checks
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

                        // Calculate average
                        float averageScore = (score + score1) / 2;

                        // Update average score
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

            SqlDataAdapter da = new SqlDataAdapter("SELECT Student_Table.student_id , Student_Table.student_firstname , Student_Table.student_lastname , Course_Table.course_id , Score_Table.score_course ,Score_Table.score1, Score_Table.score FROM Student_Table,Course_Table,Score_Table WHERE Student_Table.student_id=Score_Table.student_id AND Score_Table.score_course=Course_Table.course_label", sql); // Fetching data from the database
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

            SqlDataAdapter da = new SqlDataAdapter("SELECT student_id , student_firstname , student_lastname , student_birthdate FROM Student_Table", sql); // Fetching data from the database

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
            rowIndex = e.RowIndex;
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
