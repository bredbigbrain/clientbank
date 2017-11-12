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
    public partial class TransNotificationForm : Form
    {
        private int value;

        public TransNotificationForm(string recipientName)
        {
            InitializeComponent();
            label3.Text = recipientName;
        }

        private void TransNotificationForm_Load(object sender, EventArgs e)
        {

        }

        public int ReturnData()
        {
            return value;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out value))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Некорректная сумма", "error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
