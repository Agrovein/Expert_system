using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Expert_system
{
    public partial class startForm : Form
    {
        public startForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseQuizForm nextForm = new ChooseQuizForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdminForm nextForm = new AdminForm();
            this.Hide();
            nextForm.ShowDialog();
            this.Close();
        }
    }
}
