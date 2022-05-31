using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;

namespace Expert_system
{
    public partial class AdminForm : Form
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        int subcategoryIndex;
        string subcategoryTitle;
        string subcategoryDescription;
        string objectTitle;
        string objectDecription;
        int objectIndex;
        string picturePath;
        string[] pictureName;
        string folder;
        int objTagId;
        string questionTitle;
        string questionDescription;
        int questionIndex;

        void savePicture(string folder)
        {
            foreach (DataGridViewRow row in dataGridView5.Rows)
            {
                if (row.IsNewRow) continue;
                string[] paths = { folder, row.Cells[0].Value.ToString() };
                string path = Path.Combine(paths);
                string starterPath = row.Cells[1].Value.ToString();
                if (path != starterPath) File.Copy(starterPath, path);               
            }
        }
        void openPicture(string sqlExpression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using(var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read()) 
                        {
                            folder = reader.GetValue(0).ToString();
                        }
                    }
                }           
                string[] files = Directory.GetFiles(folder);
                foreach (string s in files)
                {
                    picturePath = s;
                    pictureName = picturePath.Split('\\');
                    DataGridViewRow row = (DataGridViewRow)dataGridView5.Rows[0].Clone();
                    row.Cells[0].Value = pictureName[pictureName.Length - 1];
                    row.Cells[1].Value = picturePath;
                    dataGridView5.Rows.Add(row);
                }
            }
        }

        void insertMethod(string sqlExpression, string tags)
        {
            if (!String.IsNullOrEmpty(objectTitleText.Text) && !String.IsNullOrEmpty(objectRichText.Text))
            {
                objectTitle = objectTitleText.Text;
                objectDecription = objectRichText.Text;
                folder = Path.Combine(Directory.GetCurrentDirectory(), objectTitle);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter titleParam = new SqlParameter("@title", objectTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@description", objectDecription);
                    command.Parameters.Add(descParam);
                    SqlParameter folderParam = new SqlParameter("@folder", folder);
                    command.Parameters.Add(folderParam);                  
                    int insertedObjectID = Convert.ToInt32(command.ExecuteScalar());
                    
                    foreach (DataGridViewRow row in dataGridView4.Rows)
                    {
                        if (row.IsNewRow) continue;
                        SqlCommand cmdTag = new SqlCommand(tags, connection);
                        SqlParameter tagTitle = new SqlParameter("@tag1", insertedObjectID);
                        cmdTag.Parameters.Add(tagTitle);
                        SqlParameter anotherTag = new SqlParameter("@tag2", row.Cells[0].Value);
                        cmdTag.Parameters.Add(anotherTag);
                        cmdTag.ExecuteNonQuery();
                    }                                     
                }
                if (artistRadio.Checked)
                {
                    string sql = "SELECT artist_ID, artist_NAME, artist_Description, pictureFolder_Path FROM artists";
                    getObjects(sql);
                }
                else if (categoryRadio.Checked)
                {
                    string sql = "SELECT category_ID, category_NAME, category_Description, pictureFolder_Path FROM categories";
                    getObjects(sql);
                }
                else
                {
                    MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                savePicture(folder);
            }
            clearObjectFields();
        }

        void updateMethod(string sqlExpression, string tags)
        {
            objectTitle = objectTitleText.Text;
            objectDecription = objectRichText.Text;
            folder = Path.Combine(Directory.GetCurrentDirectory(), objectTitle);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter titleParam = new SqlParameter("@title", objectTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@description", objectDecription);
                    command.Parameters.Add(descParam);
                    command.ExecuteNonQuery();
                    foreach (DataGridViewRow row in dataGridView4.Rows)
                    {
                        if (row.IsNewRow) continue;
                        SqlCommand cmdTag = new SqlCommand(tags, connection);
                        SqlParameter tagTitle = new SqlParameter("@tag1", objectIndex);
                        cmdTag.Parameters.Add(tagTitle);
                        SqlParameter anotherTag = new SqlParameter("@tag2", row.Cells[0].Value);
                        cmdTag.Parameters.Add(anotherTag);
                        cmdTag.ExecuteNonQuery();
                    }
                    savePicture(folder);
                    clearObjectFields();
                    if (artistRadio.Checked)
                    {
                        string sql = "SELECT artist_ID, artist_NAME, artist_Description, pictureFolder_Path FROM artists";
                        getObjects(sql);
                    }
                    else if (categoryRadio.Checked)
                    {
                        string sql = "SELECT category_ID, category_NAME, category_Description, pictureFolder_Path FROM categories";
                        getObjects(sql);
                    }
                    else
                    {
                        MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter titleParam = new SqlParameter("@title", questionTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@description", questionDescription);
                    command.Parameters.Add(descParam);
                    command.ExecuteNonQuery();
                    foreach (DataGridViewRow row in dataGridView11.Rows)
                    {
                        if (row.IsNewRow) continue;
                        SqlCommand cmdTag = new SqlCommand(tags, connection);
                        SqlParameter tagTitle = new SqlParameter("@questionID", questionIndex);
                        cmdTag.Parameters.Add(tagTitle);
                        SqlParameter anotherTag = new SqlParameter("@option", row.Cells[0].Value);
                        cmdTag.Parameters.Add(anotherTag);
                        cmdTag.ExecuteNonQuery();
                    }
                }
            }           
        }

        void deleteMethod(string folder, string sqlLink, string sqlMain)
        {
            clearObjectFields();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(folder, connection);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            folder = reader.GetValue(0).ToString();
                        }
                    }
                }               
                command = new SqlCommand(sqlLink, connection);
                command.ExecuteNonQuery();
                command = new SqlCommand(sqlMain, connection);
                command.ExecuteNonQuery();
            }
            Directory.Delete(folder, true);
        }

        void deleteQuestion(string sqlOption,string sqlLink, string sqlMain)
        {
            clearObjectFields();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlOption, connection);
                command.ExecuteNonQuery();
                command = new SqlCommand(sqlLink, connection);
                command.ExecuteNonQuery();
                command = new SqlCommand(sqlMain, connection);
                command.ExecuteNonQuery();
            }
        }

        void clearObjectFields()
        {
            dataGridView3.Rows.Clear();
            dataGridView3.Refresh();
            dataGridView4.Rows.Clear();
            dataGridView4.Refresh();
            dataGridView5.Rows.Clear();
            dataGridView5.Refresh();
            dataGridView6.Rows.Clear();
            dataGridView6.Refresh();
            pictureBox1.Image = null;
            objectTitleText.Clear();
            objectRichText.Clear();
            textBox3.Clear();
            richTextBox2.Clear();
        }

        void getObjects(string sqlExpression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                connection.Close();
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
                {
                    dataGridView10.DataSource = ds.Tables[0];
                    dataGridView10.Columns[1].HeaderText = "Назва Опитування";
                    dataGridView10.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView10.Columns[0].Visible = false;
                }
                if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                {
                    dataGridView2.DataSource = ds.Tables[0];
                    dataGridView2.Columns[1].HeaderText = "Назва Об’єкту";
                    dataGridView2.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView2.Columns[0].Visible = false;
                    dataGridView2.Columns[2].Visible = false;
                    dataGridView2.Columns[3].Visible = false;
                }
            }
        }

        void getObjectTags(string sqlExpression)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                dataGridView6.Rows.Clear();
                dataGridView6.Refresh();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                            {

                                DataGridViewRow row = (DataGridViewRow)dataGridView6.Rows[0].Clone();
                                row.Cells[0].Value = reader.GetValue(0).ToString();
                                row.Cells[1].Value = reader.GetValue(1).ToString();
                                row.Cells[2].Value = reader.GetValue(2).ToString();
                                dataGridView6.Rows.Add(row);
                            }
                            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
                            {
                                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                row.Cells[0].Value = reader.GetValue(0).ToString();
                                row.Cells[1].Value = reader.GetValue(1).ToString();
                                row.Cells[2].Value = reader.GetValue(2).ToString();
                                dataGridView1.Rows.Add(row);
                            }
                        }
                        reader.NextResult();
                    }
                }
            }
        }      

        void getAllTags(string sqlExpression)
        {            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                dataGridView8.Rows.Clear();
                dataGridView8.Refresh();
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage3"])
                            {
                                DataGridViewRow row = (DataGridViewRow)dataGridView8.Rows[0].Clone();
                                row.Cells[0].Value = reader.GetValue(0).ToString();
                                row.Cells[1].Value = reader.GetValue(1).ToString();
                                row.Cells[2].Value = reader.GetValue(2).ToString();
                                dataGridView8.Rows.Add(row);
                            }
                            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage1"])
                            {
                                DataGridViewRow row = (DataGridViewRow)dataGridView3.Rows[0].Clone();
                                row.Cells[0].Value = reader.GetValue(0).ToString();
                                row.Cells[1].Value = reader.GetValue(1).ToString();
                                row.Cells[2].Value = reader.GetValue(2).ToString();
                                dataGridView3.Rows.Add(row);
                            }
                        }
                        reader.NextResult();
                    }
                }                
            }
        }

        void getSubcategories()
        {         
            string sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                connection.Close();
                
                dataGridView7.DataSource = ds.Tables[0];
                dataGridView7.Columns[1].HeaderText = "Назва";
                dataGridView7.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView7.Columns[0].Visible = false;
                dataGridView7.Columns[2].Visible = false;
            }
        }    

        public AdminForm()
        {
            InitializeComponent();
            getSubcategories();
        }

        private void backToStartBTN_Click(object sender, EventArgs e)
        {
            startForm nextForm = new startForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        private void addObjectBTN_Click(object sender, EventArgs e)
        {
            string sqlExpression;
            if (artistRadio.Checked)
            {
                string sqlQuery = "INSERT INTO artists (aritst_NAME, artist_Description, pictureFolder_Path) output INSERTED.artist_ID VALUES (@title, @description, @folder)";
                string tags = "INSERT INTO artistsToCategoriesLinks (artist_ID, category_ID) VALUES (@tag1, @tag2)";
                insertMethod(sqlQuery, tags);
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryRadio.Checked)
            {
                string sqlQuery = "INSERT INTO categories (category_NAME, category_Description, pictureFolder_Path) output INSERTED.category_ID VALUES (@title, @description, @folder)";
                string tags = "INSERT INTO subCategoriesToCategoriesLinks (category_ID, subCategory_ID) VALUES (@tag1, @tag2)";
                insertMethod(sqlQuery, tags);
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void browseImgBTN_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                using (Image img = Image.FromFile(open.FileName))
                {
                    Bitmap bmpCopy = new Bitmap(img);
                    picturePath = open.FileName;
                    pictureName = picturePath.Split('\\');
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }
                    pictureBox1.Image = bmpCopy;
                }
            }
        }

        private void addImgBTN_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = (DataGridViewRow)dataGridView5.Rows[0].Clone();
            row.Cells[0].Value = pictureName[pictureName.Length - 1];
            row.Cells[1].Value = picturePath;
            dataGridView5.Rows.Add(row);
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView5.Rows[index];
            string path = selectedRow.Cells[1].Value.ToString();

            Image img;
            using (var bmpTemp = new Bitmap(path))
            {
                img = new Bitmap(bmpTemp);
                pictureBox1.Image = new Bitmap(img);
            }
        }

        private void rmvImgBTN_Click(object sender, EventArgs e)
        {
            int rowIndex = dataGridView5.CurrentCell.RowIndex;
            int columnindex = dataGridView5.CurrentCell.ColumnIndex;
            string startpath = dataGridView5.Rows[rowIndex].Cells[columnindex + 1].Value.ToString();
            string name = dataGridView5.Rows[rowIndex].Cells[columnindex].Value.ToString();
            string path = folder+"\\"+name;
            if (startpath == path) File.Delete(startpath);
            dataGridView5.Rows.RemoveAt(rowIndex);
        }

        private void categoryRadio_CheckedChanged(object sender, EventArgs e)
        {
            clearObjectFields();
            string sqlExpression = "SELECT category_ID, category_NAME, category_Description, pictureFolder_Path FROM categories";
            getObjects(sqlExpression);
            sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
            getAllTags(sqlExpression);
        }

        private void artistRadio_CheckedChanged(object sender, EventArgs e)
        {
            clearObjectFields();
            string sqlExpression = "SELECT artist_ID, artist_NAME, artist_Description, pictureFolder_Path FROM artists";
            getObjects(sqlExpression);
            sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
            getAllTags(sqlExpression);
        }

        private void objectsSearchTXT_TextChanged(object sender, EventArgs e)
        {
            if (artistRadio.Checked)
            {
                (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"artist_NAME LIKE '%{objectsSearchTXT.Text}%'";
            }
            else if (categoryRadio.Checked)
            {
                (dataGridView2.DataSource as DataTable).DefaultView.RowFilter = $"category_NAME LIKE '%{objectsSearchTXT.Text}%'";
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            clearObjectFields();
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView2.Rows[index];
            objectIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            objectTitleText.Text = selectedRow.Cells[1].Value.ToString();
            objectRichText.Text = selectedRow.Cells[2].Value.ToString();
            if (artistRadio.Checked)
            {
                string sqlExpression = "SELECT categories.category_ID, category_NAME, category_Description " +
                                       "FROM categories " +
                                       "JOIN artistsToCategoriesLinks " +
                                       "ON artistsToCategoriesLinks.category_ID = categories.category_ID " +
                                       "WHERE artistsToCategoriesLinks.artist_ID = " + objectIndex + "";
                getObjectTags(sqlExpression);
                string sqlEx = "SELECT pictureFolder_Path FROM artists WHERE artist_ID = " + objectIndex + "";

                openPicture(sqlEx);
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryRadio.Checked)
            {
                string sqlExpression = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                       "FROM subcategories " +
                                       "JOIN subCategoriesToCategoriesLinks " +
                                       "ON subCategoriesToCategoriesLinks.subCategory_ID = subcategories.subCategory_ID " +
                                       "WHERE subCategoriesToCategoriesLinks.category_ID = " + objectIndex + "";
                getObjectTags(sqlExpression);
                string sqlEx = "SELECT pictureFolder_Path FROM categories WHERE category_ID = " + objectIndex + "";
                openPicture(sqlEx);
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void addTagBTN_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView3.CurrentCell.RowIndex;
            int columnindex = dataGridView3.CurrentCell.ColumnIndex;
            string id = dataGridView3.Rows[rowindex].Cells[columnindex - 1].Value.ToString();
            string name = dataGridView3.Rows[rowindex].Cells[columnindex].Value.ToString();
            string description = dataGridView3.Rows[rowindex].Cells[columnindex + 1].Value.ToString();
            DataGridViewRow row = (DataGridViewRow)dataGridView4.Rows[0].Clone();
            row.Cells[0].Value = id;
            row.Cells[1].Value = name;
            row.Cells[2].Value = description;
            dataGridView4.Rows.Add(row);
            dataGridView3.Rows.RemoveAt(rowindex);
        }

        private void rmvTagBTN_Click(object sender, EventArgs e)
        {
            string sqlExpression;
            int rowindex = dataGridView6.CurrentCell.RowIndex;
            int columnindex = dataGridView6.CurrentCell.ColumnIndex;
            objTagId = Convert.ToInt32(dataGridView3.Rows[rowindex].Cells[columnindex - 1].Value);
            if (artistRadio.Checked)
            {
                sqlExpression = "DELETE FROM artistsToCategoriesLinks WHERE category_ID = " + objTagId + " AND artist_ID = " + objectIndex + "";
                rmvObjectTag(sqlExpression);
            }
            else if (categoryRadio.Checked)
            {
                sqlExpression = "DELETE FROM subCategoriesToCategoriesLinks WHERE subCategory_ID = " + objTagId + " AND category_ID = " + objectIndex + "";
                rmvObjectTag(sqlExpression);
            }
            dataGridView6.Rows.RemoveAt(rowindex);
        }

        void rmvObjectTag(string sqlExpression)
        {
            string sqlExp;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                command.ExecuteNonQuery();
            }
            if (artistRadio.Checked)
            {
                sqlExp = "SELECT categories.category_ID, category_NAME, category_Description " +
                                       "FROM categories " +
                                       "JOIN artistsToCategoriesLinks " +
                                       "ON artistsToCategoriesLinks.category_ID = categories.category_ID " +
                                       "WHERE artistsToCategoriesLinks.artist_ID = " + objectIndex + "";
                getObjectTags(sqlExp);
            }
            else if (categoryRadio.Checked)
            {
                sqlExp = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                       "FROM subcategories " +
                                       "JOIN subCategoriesToCategoriesLinks " +
                                       "ON subCategoriesToCategoriesLinks.subCategory_ID = subcategories.subCategory_ID " +
                                       "WHERE subCategoriesToCategoriesLinks.category_ID = " + objectIndex + "";
                getObjectTags(sqlExp);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(richTextBox1.Text))
            {
                subcategoryTitle = textBox2.Text;
                subcategoryDescription = richTextBox1.Text;
                string sqlExpression = "INSERT INTO subcategories (subCategory_NAME, subCategory_Description) VALUES (@title, @description)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter titleParam = new SqlParameter("@title", subcategoryTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@description", subcategoryDescription);
                    command.Parameters.Add(descParam);

                    command.ExecuteNonQuery();
                }
                getSubcategories();
            }
            textBox2.Clear();
            richTextBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(richTextBox1.Text))
            {
                subcategoryTitle = textBox2.Text;
                subcategoryDescription = richTextBox1.Text;
                string sqlExpression = "UPDATE subcategories SET subCategory_NAME=@title, subCategory_Description = @description WHERE subCategory_ID = '" + subcategoryIndex + "' ";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlParameter titleParam = new SqlParameter("@title", subcategoryTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@description", subcategoryDescription);
                    command.Parameters.Add(descParam);

                    command.ExecuteNonQuery();
                }
                getSubcategories();
            }
            textBox2.Clear();
            richTextBox1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sqlExpression = "DELETE FROM subcategories WHERE subCategory_ID = '" + subcategoryIndex + "' ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);

                command.ExecuteNonQuery();
            }
            getSubcategories();
            textBox2.Clear();
            richTextBox1.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            startForm nextForm = new startForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            (dataGridView7.DataSource as DataTable).DefaultView.RowFilter = $"subCategory_NAME LIKE '%{textBox1.Text}%'";
        }

        private void dataGridView7_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox2.Clear();
            richTextBox1.Clear();
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView7.Rows[index];
            subcategoryIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            textBox2.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox1.Text = selectedRow.Cells[2].Value.ToString();
        }

        private void changeObjectBTN_Click(object sender, EventArgs e)
        {                      
            if (!String.IsNullOrEmpty(objectTitleText.Text) && !String.IsNullOrEmpty(objectRichText.Text))
            {
                if (artistRadio.Checked)
                {
                    string sqlQuery = "UPDATE artists SET artist_NAME=@title, artist_Description = @description WHERE artist_ID = '" + objectIndex + "' ";
                    string tags = "INSERT INTO artistsToCategoriesLinks (artist_ID, category_ID) VALUES (@tag1, @tag2)";
                    updateMethod(sqlQuery, tags);
                }
                else if (categoryRadio.Checked)
                {
                    string sqlQuery = "UPDATE categories SET category_NAME=@title, category_Description = @description WHERE category_ID = '" + objectIndex + "' ";
                    string tags = "INSERT INTO subCategoriesToCategoriesLinks (category_ID, subCategory_ID) VALUES (@tag1, @tag2)";
                    updateMethod(sqlQuery, tags);
                }
                else
                {
                    MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        private void dataGridView6_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView6.Rows[index];
            textBox3.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox2.Text = selectedRow.Cells[2].Value.ToString();
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView3.Rows[index];
            textBox3.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox2.Text = selectedRow.Cells[2].Value.ToString();
            int tagIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            bool added = false;
            foreach (DataGridViewRow row in dataGridView6.Rows)
            {
                if (row.IsNewRow) continue;
                if (tagIndex == Convert.ToInt32(row.Cells[0].Value)) added = true;
            }
            if (added) addTagBTN.Enabled = false;
            else addTagBTN.Enabled = true;

        }

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView4.Rows[index];
            textBox3.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox2.Text = selectedRow.Cells[2].Value.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sqlExpression;
            int rowindex = dataGridView4.CurrentCell.RowIndex;
            dataGridView4.Rows.RemoveAt(rowindex);
            if (artistRadio.Checked)
            {
                dataGridView3.Rows.Clear();
                dataGridView3.Refresh();
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryRadio.Checked)
            {
                dataGridView3.Rows.Clear();
                dataGridView3.Refresh();
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
        }

        private void delObjectBTN_Click(object sender, EventArgs e)
        {
            string sqlLink, sqlMain, folder, sqlEx;          
            if (artistRadio.Checked)
            {
                folder = "SELECT pictureFolder_Path FROM artists WHERE artist_ID = " + objectIndex + "";
                sqlLink = "DELETE FROM artistsToCategoriesLinks WHERE artist_ID = " + objectIndex + "";
                sqlMain = "DELETE FROM artists WHERE artist_ID = " + objectIndex + "";
                deleteMethod(folder, sqlLink, sqlMain);
                sqlEx = "SELECT artist_ID, artist_NAME, artist_Description, pictureFolder_Path FROM artists";
                getObjects(sqlEx);
            }
            else if (categoryRadio.Checked)
            {
                folder = "SELECT pictureFolder_Path FROM categories WHERE category_ID = " + objectIndex + "";
                sqlLink = "DELETE FROM subCategoriesToCategoriesLinks WHERE category_ID = " + objectIndex + "";
                sqlMain = "DELETE FROM categories WHERE category_ID = " + objectIndex + "";
                deleteMethod(folder, sqlLink, sqlMain);
                sqlEx = "SELECT category_ID, category_NAME, category_Description, pictureFolder_Path FROM categories";
                getObjects(sqlEx);
            }
            clearObjectFields();
        }

        private void categoryQuizRadion_CheckedChanged(object sender, EventArgs e)
        {
            string type = "category";
            clearQuizFields();
            string sqlExpression = "SELECT quiz_ID, quiz_Title FROM quiz WHERE quiz_Type = '" + type + "'";
            getObjects(sqlExpression);
            sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
            getAllTags(sqlExpression);
            dataGridView10.Rows[1].Cells[0].Selected = false;
        }

        void clearQuizFields()
        {
            dataGridView8.Rows.Clear();
            dataGridView8.Refresh();
            dataGridView9.Rows.Clear();
            dataGridView9.Refresh();
            richTextBox3.Clear();
            richTextBox4.Clear();
        }  

        void insertQuiz(string sql, string type) 
        {
            if (!String.IsNullOrEmpty(textBox7.Text))
            {
                string quizTitle = textBox7.Text;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    SqlParameter titleParam = new SqlParameter("@title", quizTitle);
                    command.Parameters.Add(titleParam);
                    SqlParameter descParam = new SqlParameter("@type", type);
                    command.Parameters.Add(descParam);
                    command.ExecuteNonQuery();
                }
                string sqlExpression = "SELECT quiz_ID, quiz_Title FROM quiz WHERE quiz_Type = '" + type + "'";
                getObjects(sqlExpression);            
            }
        }

        void updateQuiz(string sql) { }
        void deleteQuiz(string sql) 
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }

        private void artistQuizRadio_CheckedChanged(object sender, EventArgs e)
        {
            string type = "artist";
            clearQuizFields();
            string sqlExpression = "SELECT quiz_ID, quiz_Title FROM quiz WHERE quiz_Type = '" + type + "'";
            getObjects(sqlExpression);
            sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
            getAllTags(sqlExpression);
            dataGridView10.Rows[1].Cells[0].Selected = false;

        }

        private void addQuizBTN_Click(object sender, EventArgs e)
        {
            if (artistQuizRadio.Checked)
            {
                string type = "artist";
                string sqlQuery = "INSERT INTO quiz (quiz_Title, quiz_Type) VALUES (@title, @type)";
                insertQuiz(sqlQuery, type);
            }
            else if (categoryQuizRadion.Checked)
            {
                string type = "category";
                string sqlQuery = "INSERT INTO quiz (quiz_Title, quiz_Type) VALUES (@title, @type)";
                insertQuiz(sqlQuery, type);
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView10_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView9.Rows.Clear();
            dataGridView9.Refresh();
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView10.Rows[index];
            objectIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            textBox7.Text = selectedRow.Cells[1].Value.ToString();
            string sqlExpression =  "SELECT quiz_questions.question_ID, question_Title, question_Description " +
                                    "FROM quiz_questions " +
                                    "JOIN quizToQuestions " +
                                    "ON quizToQuestions.question_ID = quiz_questions.question_ID " +
                                    "WHERE quizToQuestions.quiz_ID = " + objectIndex + "";
            getQuizInfo(sqlExpression);
            if (artistQuizRadio.Checked)
            {
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryQuizRadion.Checked)
            {
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void getQuizInfo(string sqlExpression)
        {
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
                            DataGridViewRow row = (DataGridViewRow)dataGridView9.Rows[0].Clone();
                            row.Cells[0].Value = reader.GetValue(0).ToString();
                            row.Cells[1].Value = reader.GetValue(1).ToString();
                            row.Cells[2].Value = reader.GetValue(2).ToString();
                            dataGridView9.Rows.Add(row);
                        }
                        reader.NextResult();
                    }
                }
            }
        }

        private void addQstnBTN_Click(object sender, EventArgs e)
        {
            dataGridView9.Rows.Clear();
            dataGridView9.Refresh();
            if (!String.IsNullOrEmpty(textBox6.Text) && !String.IsNullOrEmpty(richTextBox3.Text))
            {
                questionTitle = textBox6.Text;
                questionDescription = richTextBox3.Text;
                string sql = "INSERT INTO quiz_questions (question_Title, question_Description) output INSERTED.question_ID VALUES (@title, @description)";
                addQuestion(sql);
            }
            string sqlExpression =  "SELECT quiz_questions.question_ID, question_Title, question_Description " +
                                    "FROM quiz_questions " +
                                    "JOIN quizToQuestions " +
                                    "ON quizToQuestions.question_ID = quiz_questions.question_ID " +
                                    "WHERE quizToQuestions.quiz_ID = " + objectIndex + "";
            getQuizInfo(sqlExpression);

        }

        void addQuestion(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sql, connection);
                SqlParameter titleParam = new SqlParameter("@title", questionTitle);
                command.Parameters.Add(titleParam);
                SqlParameter descParam = new SqlParameter("@description", questionDescription);
                command.Parameters.Add(descParam);
                int insertedQuestionID = Convert.ToInt32(command.ExecuteScalar());

                sql = "INSERT INTO quizToQuestions (quiz_ID, question_ID) VALUES (@quizID, @questionID)";
                SqlCommand link = new SqlCommand(sql, connection);
                SqlParameter quizID = new SqlParameter("@quizID", objectIndex);
                link.Parameters.Add(quizID);
                SqlParameter questionID = new SqlParameter("@questionID", insertedQuestionID);
                link.Parameters.Add(questionID);
                link.ExecuteNonQuery();
            }
        }

        private void dataGridView8_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView8.Rows[index];
            textBox11.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox4.Text = selectedRow.Cells[2].Value.ToString();
            int tagIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            bool added = false;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                if (tagIndex == Convert.ToInt32(row.Cells[0].Value)) added = true;
            }
            if (added) attachOptionBTN.Enabled = false;
            else attachOptionBTN.Enabled = true;
        }

        private void attachOptionBTN_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView8.CurrentCell.RowIndex;
            int columnindex = dataGridView8.CurrentCell.ColumnIndex;
            string id = dataGridView8.Rows[rowindex].Cells[columnindex - 1].Value.ToString();
            string name = dataGridView8.Rows[rowindex].Cells[columnindex].Value.ToString();
            string description = dataGridView8.Rows[rowindex].Cells[columnindex + 1].Value.ToString();
            DataGridViewRow row = (DataGridViewRow)dataGridView11.Rows[0].Clone();
            row.Cells[0].Value = id;
            row.Cells[1].Value = name;
            row.Cells[2].Value = description;
            dataGridView11.Rows.Add(row);
            dataGridView8.Rows.RemoveAt(rowindex);
        }

        private void undoAttachOptionBTN_Click(object sender, EventArgs e)
        {
            string sqlExpression;
            int rowindex = dataGridView11.CurrentCell.RowIndex;
            dataGridView11.Rows.RemoveAt(rowindex);
            if (artistQuizRadio.Checked)
            {
                dataGridView8.Rows.Clear();
                dataGridView8.Refresh();
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryQuizRadion.Checked)
            {
                dataGridView8.Rows.Clear();
                dataGridView8.Refresh();
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
        }

        private void rmvOptionBTN_Click(object sender, EventArgs e)
        {
            string sqlExpression;
            int rowindex = dataGridView1.CurrentCell.RowIndex;
            int columnindex = dataGridView1.CurrentCell.ColumnIndex;
            objTagId = Convert.ToInt32(dataGridView1.Rows[rowindex].Cells[columnindex - 1].Value);
            if (artistQuizRadio.Checked)
            {
                sqlExpression = "DELETE FROM categoryToQuestions WHERE category_ID = " + objTagId + " AND question_ID = " + questionIndex + "";
                rmvObjectTag(sqlExpression);
            }
            else if (categoryQuizRadion.Checked)
            {
                sqlExpression = "DELETE FROM subCategoryToQuestions WHERE subCategory_ID = " + objTagId + " AND question_ID = " + questionIndex + "";
                rmvObjectTag(sqlExpression);
            }
            dataGridView1.Rows.RemoveAt(rowindex);
        }

        private void dataGridView9_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            int index = e.RowIndex;
            DataGridViewRow selectedRow = dataGridView9.Rows[index];
            questionIndex = Convert.ToInt32(selectedRow.Cells[0].Value.ToString());
            textBox6.Text = selectedRow.Cells[1].Value.ToString();
            richTextBox3.Text = selectedRow.Cells[2].Value.ToString();
            if (artistQuizRadio.Checked)
            {
                string sqlExpression = "SELECT categories.category_ID, category_NAME, category_Description " +
                                       "FROM categories " +
                                       "JOIN categoryToQuestions " +
                                       "ON categoryToQuestions.category_ID = categories.category_ID " +
                                       "WHERE categoryToQuestions.question_ID = " + questionIndex + "";
                getObjectTags(sqlExpression);
                sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                getAllTags(sqlExpression);
            }
            else if (categoryQuizRadion.Checked)
            {
                string sqlExpression = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                       "FROM subcategories " +
                                       "JOIN subCategoryToQuestions " +
                                       "ON subCategoryToQuestions.subCategory_ID = subcategories.subCategory_ID " +
                                       "WHERE subCategoryToQuestions.question_ID = " + questionIndex + "";
                getObjectTags(sqlExpression);
                sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                getAllTags(sqlExpression);
            }
            else
            {
                MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updQstnBTN_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox6.Text) && !String.IsNullOrEmpty(richTextBox3.Text))
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                questionTitle = textBox6.Text;
                questionDescription = richTextBox3.Text;
                if (artistQuizRadio.Checked)
                {
                    string sqlQuery = "UPDATE quiz_questions SET question_Title=@title, question_Description = @description WHERE question_ID = '" + questionIndex + "' ";
                    string options = "INSERT INTO categoryToQuestions (question_ID, category_ID) VALUES (@questionID, @option)";
                    updateMethod(sqlQuery, options);
                    sqlQuery = "SELECT categories.category_ID, category_NAME, category_Description " +
                                       "FROM categories " +
                                       "JOIN categoryToQuestions " +
                                       "ON categoryToQuestions.category_ID = categories.category_ID " +
                                       "WHERE categoryToQuestions.question_ID = " + questionIndex + "";
                    getObjectTags(sqlQuery);
                    dataGridView11.Rows.Clear();
                    dataGridView11.Refresh();
                    string sqlExpression = "SELECT category_ID, category_NAME, category_Description FROM categories";
                    getAllTags(sqlExpression);
                }
                else if (categoryQuizRadion.Checked)
                {
                    string sqlQuery = "UPDATE quiz_questions SET question_Title=@title, question_Description = @description WHERE question_ID = '" + questionIndex + "' ";
                    string tags = "INSERT INTO subCategoryToQuestions (question_ID, subCategory_ID) VALUES (@questionID, @option)";
                    updateMethod(sqlQuery, tags);
                    sqlQuery = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                       "FROM subcategories " +
                                       "JOIN subCategoryToQuestions " +
                                       "ON subCategoryToQuestions.subCategory_ID = subcategories.subCategory_ID " +
                                       "WHERE subCategoryToQuestions.question_ID = " + questionIndex + "";
                    getObjectTags(sqlQuery);
                    dataGridView11.Rows.Clear();
                    dataGridView11.Refresh();
                    string sqlExpression = "SELECT subCategory_ID, subCategory_NAME, subCategory_Description FROM subcategories";
                    getAllTags(sqlExpression);
                }
                else
                {
                    MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void backToMainBTN_Click(object sender, EventArgs e)
        {
            startForm nextForm = new startForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        private void delQstnBTN_Click(object sender, EventArgs e)
        {
            string sqlLink, sqlMain, sqlOptions;
            if (artistQuizRadio.Checked)
            {
                sqlOptions = "DELETE FROM categoryToQuestions WHERE question_ID = " + questionIndex + "";
                sqlLink = "DELETE FROM quizToQuestions WHERE question_ID = " + questionIndex + "";
                sqlMain = "DELETE FROM quiz_questions WHERE question_ID = " + questionIndex + "";
                deleteQuestion(sqlOptions, sqlLink, sqlMain);
            }
            else if (categoryQuizRadion.Checked)
            {
                sqlOptions = "DELETE FROM subCategoryToQuestions WHERE question_ID = " + questionIndex + "";
                sqlLink = "DELETE FROM quizToQuestions WHERE question_ID = " + questionIndex + "";
                sqlMain = "DELETE FROM quiz_questions WHERE question_ID = " + questionIndex + "";
                deleteQuestion(sqlOptions, sqlLink, sqlMain);

            }
            string sqlExpression = "SELECT quiz_questions.question_ID, question_Title, question_Description " +
                                    "FROM quiz_questions " +
                                    "JOIN quizToQuestions " +
                                    "ON quizToQuestions.question_ID = quiz_questions.question_ID " +
                                    "WHERE quizToQuestions.quiz_ID = " + objectIndex + "";
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView9.Rows.Clear();
            dataGridView9.Refresh();
            getQuizInfo(sqlExpression);
        }

        private void updQuizBTN_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBox7.Text))
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                questionTitle = textBox6.Text;
                questionDescription = richTextBox3.Text;
                if (artistQuizRadio.Checked)
                {
                    string sqlQuery = "UPDATE quiz_questions SET question_Title=@title, question_Description = @description WHERE question_ID = '" + questionIndex + "' ";
                    string options = "INSERT INTO categoryToQuestions (question_ID, category_ID) VALUES (@questionID, @option)";
                    updateMethod(sqlQuery, options);
                    sqlQuery = "SELECT categories.category_ID, category_NAME, category_Description " +
                                       "FROM categories " +
                                       "JOIN categoryToQuestions " +
                                       "ON categoryToQuestions.category_ID = categories.category_ID " +
                                       "WHERE categoryToQuestions.question_ID = " + questionIndex + "";
                    getObjectTags(sqlQuery);
                }
                else if (categoryQuizRadion.Checked)
                {
                    string sqlQuery = "UPDATE quiz_questions SET question_Title=@title, question_Description = @description WHERE question_ID = '" + questionIndex + "' ";
                    string tags = "INSERT INTO subCategoryToQuestions (question_ID, subCategory_ID) VALUES (@questionID, @option)";
                    updateMethod(sqlQuery, tags);
                    sqlQuery = "SELECT subcategories.subCategory_ID, subCategory_NAME, subCategory_Description " +
                                       "FROM subcategories " +
                                       "JOIN subCategoryToQuestions " +
                                       "ON subCategoryToQuestions.subCategory_ID = subcategories.subCategory_ID " +
                                       "WHERE subCategoryToQuestions.question_ID = " + questionIndex + "";
                    getObjectTags(sqlQuery);
                }
                else
                {
                    MessageBox.Show("Choose Data Type with RadioButton", "Some text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void delQuizBTN_Click(object sender, EventArgs e)
        {
            string sqlLink, sqlMain, sqlOptions;
            if (artistQuizRadio.Checked)
            {
                sqlOptions = "DELETE FROM categoryToQuestions WHERE question_ID = @id";
                sqlLink = "DELETE FROM quizToQuestions WHERE question_ID = @id";
                sqlMain = "DELETE FROM quiz_questions WHERE question_ID = @id";
                delQuestion(sqlOptions, sqlLink, sqlMain);
                string type = "artist";
                string sqlExpression = "SELECT quiz_ID, quiz_Title FROM quiz WHERE quiz_Type = '" + type + "'";
                getObjects(sqlExpression);
            }
            else if (categoryQuizRadion.Checked)
            {
                sqlOptions = "DELETE FROM subCategoryToQuestions WHERE question_ID = @id";
                sqlLink = "DELETE FROM quizToQuestions WHERE question_ID = @id";
                sqlMain = "DELETE FROM quiz_questions WHERE question_ID = @id";
                delQuestion(sqlOptions, sqlLink, sqlMain);
                string type = "category";
                string sqlExpression = "SELECT quiz_ID, quiz_Title FROM quiz WHERE quiz_Type = '" + type + "'";
                getObjects(sqlExpression);
            }           
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            dataGridView9.Rows.Clear();
            dataGridView9.Refresh();
        }

        void delQuestion(string sqlOptions, string sqlLink, string sqlMain)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (DataGridViewRow row in dataGridView9.Rows) 
                {
                    if (row.IsNewRow) continue;
                    SqlCommand command = new SqlCommand(sqlOptions, connection);
                    SqlParameter id = new SqlParameter("@id", row.Cells[0].Value);
                    SqlParameter ident = new SqlParameter("@id", row.Cells[0].Value);
                    SqlParameter identity = new SqlParameter("@id", row.Cells[0].Value);
                    command.Parameters.Add(id);
                    command.ExecuteNonQuery();
                    SqlCommand cmd = new SqlCommand(sqlLink, connection);
                    cmd = new SqlCommand(sqlLink, connection);
                    cmd.Parameters.Add(ident);
                    cmd.ExecuteNonQuery();
                    SqlCommand cm = new SqlCommand(sqlLink, connection);
                    cm = new SqlCommand(sqlLink, connection);
                    cm.Parameters.Add(identity);
                    cm.ExecuteNonQuery();
                }
            }
            string sql = "DELETE FROM quiz WHERE quiz_ID = "+ objectIndex +"";
            deleteQuiz(sql);
        }
    }
}
