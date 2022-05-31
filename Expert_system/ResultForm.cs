using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Expert_system
{
    public partial class ResultForm : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        List<int> answ = new List<int>();
        public ResultForm(List<int> answers, string type)
        {
            InitializeComponent();
            getResults(type, answers);
            foreach(var i in answers)
            {
                answ.Add(i);
            }           
            writeResultData();
        }
        List<quizObject> quizObjects = new List<quizObject>();

        void writeResultData()
        {
            /* textBox1.Text = questions[counter].getQuestionTitle();
             richTextBox1.Text = questions[counter].getQuestionDescription();
             dataGridView1.Rows.Clear();
             dataGridView1.Refresh();*/

            foreach (var e in quizObjects)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                row.Cells[0].Value = e.id;
                row.Cells[1].Value = e.title;
                row.Cells[2].Value = e.description;
                row.Cells[3].Value = e.folder;
                row.Cells[4].Value = e.precision+"%";
                dataGridView1.Rows.Add(row);
            }
            dataGridView1.Sort(dataGridView1.Columns[4], ListSortDirection.Descending);

        }

        string typeChecker(string type)
        {
            if (type == "artist")
            {
                string sqlExpression = "SELECT artist_ID, artist_NAME, artist_Description, pictureFolder_Path " +
                                       "FROM artists";
                return sqlExpression;
            }
            if (type == "category")
            {
                string sqlExpression = "SELECT category_ID, category_NAME, category_Description, pictureFolder_Path " +
                                       "FROM categories";
                return sqlExpression;
            }
            return "";
        }

        void getResults(string type, List<int> answ)
        {
            string sqlExpression = typeChecker(type);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        int equalCounter = 0;                        
                        while (reader.Read())
                        {
                            quizObject quizObject = new quizObject();
                            quizObject.id = Convert.ToInt32(reader.GetValue(0));
                            quizObject.title = reader.GetValue(1).ToString();
                            quizObject.description = reader.GetValue(2).ToString();
                            quizObject.folder = reader.GetValue(3).ToString();
                            string sql;
                            if (type == "artist")
                            {
                                sql = "SELECT category_ID " +
                                      "FROM artistsToCategoriesLinequalCounters  " +
                                      "WHERE artist_ID = " + quizObject.id + "";
                               quizObject.addOptions(getOpts(sql));
                            }
                            if (type == "category")
                            {
                                sql = "SELECT subCategory_ID " +
                                      "FROM subCategoriesToCategoriesLinks  " +    
                                      "WHERE category_ID = " + quizObject.id + "";
                                quizObject.addOptions(getOpts(sql));
                            }
                            List<optionB> op = new List<optionB>();
                            op = quizObject.getObjectOptions();
                            List<int> nameList = op.ConvertAll(x => x.id);
                            equalCounter = compareOptions(answ, nameList);
                            quizObject.precision = equalCounter;
                            quizObjects.Add(quizObject);
                        }
                        reader.NextResult();
                    }
                }
            }
        }

        int compareOptions(List<int> answ, List<int> opts)
        {
            int count = 0;
            int counter = opts.Count;
            foreach(var e in opts)
            {
                foreach(var i in answ)
                {
                    if (e == i) count++;
                }
            }
            int result = 100 / counter * count;
            return result;
        }

        public List<optionB> getOpts(string sql)
        {
            List<optionB> options = new List<optionB>();
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
                            optionB option = new optionB();
                            option.id = Convert.ToInt32(reader.GetValue(0));
                            options.Add(option);
                        }
                        reader.NextResult();
                    }
                }
            }
            return options;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ChooseQuizForm nextForm = new ChooseQuizForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }
        void openPicture(string folder)
        {            
            string[] files = Directory.GetFiles(folder);
            string picturePath;
            string[] pictureName;
            dataGridView2.Rows.Clear();
            dataGridView2.Refresh();
            foreach (string s in files)
            {
                picturePath = s;
                pictureName = picturePath.Split('\\');
                DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[0].Clone();
                row.Cells[0].Value = pictureName[pictureName.Length - 1];
                row.Cells[1].Value = picturePath;
                dataGridView2.Rows.Add(row);
            }
            
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView1.Rows[index];
            string folder = quizObjects[index].getObjectFolder();
            openPicture(folder);
            richTextBox1.Text = quizObjects[index].getObjectDescription();
            //richTextBox2.Text = selectedRow.Cells[2].Value.ToString();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView2.Rows[index];
            string path = selectedRow.Cells[1].Value.ToString();

            Image img;
            using (var bmpTemp = new Bitmap(path))
            {
                img = new Bitmap(bmpTemp);
                pictureBox1.Image = new Bitmap(img);
            }
        }
    }

    public class quizObject
    {
        public int id;
        public string title;
        public string description;
        public string folder;
        public int precision;
        public int count;
        List<optionB> options = new List<optionB>();

        public void addOptions(List<optionB> opts)
        {
            foreach (optionB op in opts)
            {
                options.Add(op);
            }
            count = options.Count;
        }
        
        public string getObjectTitle()
        {
            return title;
        }

        public string getObjectDescription()
        {
            return description;
        }

        public string getObjectFolder()
        {
            return folder;
        }

        public int getObjectPrecision()
        {
            return precision;
        }

        public int getObjectCount()
        {
            return count;
        }

        public List<optionB> getObjectOptions()
        {
            return options;
        }
    }

    public class optionB
    {
        public int id;
    }
}
