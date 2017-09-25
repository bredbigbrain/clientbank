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
    public partial class Form3 : Form
    {
        string[] data;
        StorageView storV;

        public Form3(StorageView storageV)
        {
            InitializeComponent();
            data = new string[3];
            storV = storageV;

            
            comboBox2.Enabled = false;

            foreach(Client cl in storV.GetClients())
            {
                comboBox1.Items.Add( String.Concat( cl.id, " ", cl.name, " ", cl.GetMoney()));
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            data[0] = storV.GetClients()[comboBox1.SelectedIndex].id.ToString();
            if(comboBox2.SelectedIndex >= comboBox1.SelectedIndex)
                data[1] = storV.GetClients()[comboBox2.SelectedIndex + 1].id.ToString();
            else
                data[1] = storV.GetClients()[comboBox2.SelectedIndex].id.ToString();
            data[2] = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        public string[] ReturnData()
        {
            return data;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Enabled = true;
            comboBox2.Items.Clear();
            comboBox2.Text = "";

            int i = 0;
            foreach (Client cl in storV.GetClients())
            {
                if (comboBox1.SelectedIndex != i)
                {
                    comboBox2.Items.Add(String.Concat(cl.id, " ", cl.name, " ", cl.GetMoney()));
                }
                i++;
            }
        }
    }
}
