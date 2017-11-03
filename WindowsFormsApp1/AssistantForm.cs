using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class AssistantForm : Form
    {
        Bot_Assistant bot;

        public AssistantForm()
        {
            InitializeComponent();
            bot = new Bot_Assistant("QuestionsData.txt");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                label3.Text = bot.Answer(textBox1.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button10_Click(sender, e);
            }
        }

        private void AssistantForm_Load(object sender, EventArgs e)
        {

        }
    }
}
