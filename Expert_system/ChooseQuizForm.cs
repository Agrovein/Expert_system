using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Expert_system
{
    public partial class ChooseQuizForm : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ChooseQuizForm()
        {
            InitializeComponent();
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            getQuizList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            startForm nextForm = new startForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        void getQuizList()
        {
            string sql = "SELECT quiz_ID, quiz_Title, quiz_Type FROM quiz";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                connection.Close();

                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Columns[1].HeaderText = "Назва Опитування";
                dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns[0].Visible = false;
                dataGridView1.Columns[2].Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView1.CurrentCell.RowIndex;
            int columnindex = dataGridView1.CurrentCell.ColumnIndex;
            int quizID = Convert.ToInt32(dataGridView1.Rows[rowindex].Cells[columnindex - 1].Value);
            string type = dataGridView1.Rows[rowindex].Cells[columnindex + 1].Value.ToString();

            QuizForm nextForm = new QuizForm(quizID, type);
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }
    }
}
