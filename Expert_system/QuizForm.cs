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

    public partial class QuizForm : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public QuizForm(int id, string type)
        {
            InitializeComponent();
            int quizID = id;
            quizType = type;
            getQuestion(quizID, quizType);
            writeQuestionData(counter);         
        }
        string quizType;
        List<question> questions = new List<question>();
        List<int> ans = new List<int>();
        int counter = 0;
        
        public List<option> getOpts(string sql)
        {
            List<option> options = new List<option>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            option option = new option();
                            option.id = Convert.ToInt32(reader.GetValue(0));
                            option.title = reader.GetValue(1).ToString();
                            option.description = reader.GetValue(2).ToString();
                            options.Add(option);
                        }
                        reader.NextResult();
                    }
                }
            }
            return options;
        }

        

        void getQuestion(int quizID, string quizType)
        {
            string sqlExpression = "SELECT quiz_questions.question_ID, question_Title, question_Description " +
                                    "FROM quiz_questions " +
                                    "JOIN quizToQuestions " +
                                    "ON quizToQuestions.question_ID = quiz_questions.question_ID " +
                                    "WHERE quizToQuestions.quiz_ID = " + quizID + "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            question question = new question(quizType);
                            question.id = Convert.ToInt32(reader.GetValue(0));
                            question.title = reader.GetValue(1).ToString();
                            question.description = reader.GetValue(2).ToString();                          

                            string sql;
                            if (quizType == "artist")
                            {
                                sql = "SELECT categories.category_ID, category_NAME, category_Description " +
                                                       "FROM categories " +
                                                       "JOIN categoryToQuestions " +
                                                       "ON categoryToQuestions.category_ID = categories.category_ID " +
                                                       "WHERE categoryToQuestions.question_ID = " + question.id + "";
                                question.addOptions(getOpts(sql));


                            }
                            if (quizType == "category")
                            {
                                sql = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                                       "FROM subcategories " +
                                                       "JOIN subCategoryToQuestions " +
                                                       "ON subCategoryToQuestions.subCategory_ID = subcategories.subCategory_ID " +
                                                       "WHERE subCategoryToQuestions.question_ID = " + question.id + "";
                                question.addOptions(getOpts(sql));
                            }
                            questions.Add(question);
                        }
                        reader.NextResult();
                    }
                }               
            }
        }
       

        void writeQuestionData(int counter)
        {
            textBox1.Text = questions[counter].getQuestionTitle();
            richTextBox1.Text = questions[counter].getQuestionDescription();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            foreach(var e in questions[counter].getQuestionOptions())
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                row.Cells[0].Value = e.id;
                row.Cells[1].Value = e.title;
                row.Cells[2].Value = e.description;
                dataGridView1.Rows.Add(row);
            }
        }
        private void backToStartBTN_Click(object sender, EventArgs e)
        {
            ChooseQuizForm nextForm = new ChooseQuizForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rowIndex = dataGridView1.CurrentCell.RowIndex;
            int columnindex = dataGridView1.CurrentCell.ColumnIndex;
            ans.Add(Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells[columnindex - 1].Value));
            if (counter < questions.Count-1)
            {
                
                ++counter;
                writeQuestionData(counter);
                
            }
            else
            {
                ResultForm nextForm = new ResultForm(ans, quizType);
                this.Hide();
                nextForm.ShowDialog();
                this.Close();
            }


        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView1.Rows[index];
            richTextBox2.Text = selectedRow.Cells[2].Value.ToString();
        }
    }

    public class question
    {
        public int id;
        public string title;
        public string description;
        public string quizType;
        List<option> options = new List<option>();

        public void addOptions(List<option> opts)
        {
            foreach(option op in opts)
            {
                options.Add(op);
            }
        }

        public question(string type)
        {
            quizType = type;
        }       
        
        public string getQuestionTitle()
        {
            return title;
        }

        public string getQuestionDescription()
        {
            return description;
        }

        public List<option> getQuestionOptions()
        {
            return options;
        }

    }

    public class option
    {
        public int id;
        public string title;
        public string description;
    }
}
