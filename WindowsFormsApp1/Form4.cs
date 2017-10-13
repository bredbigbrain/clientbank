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
    public partial class Form4 : Form
    {
        Client client;
        StorageView stV;
        float wantedSumm;
        int wantedTime;

        public Form4(StorageView _stV ,int _id)
        {
            InitializeComponent();

            stV = _stV;
            client = Storage.FindClientByID(_id);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            label4.Text = stV.AllowedCreditSumm(client, 1.5f, 12);
            label9.Text = client.name;
            label8.Text = client.GetMoney().ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float allowedSumm;
            if (float.TryParse(textBox1.Text, out wantedSumm) & int.TryParse(textBox2.Text, out wantedTime))
            {
                string checkResult = stV.AllowedCreditSumm(client, 1.5f, wantedTime);
                if (float.TryParse(checkResult, out allowedSumm))
                {
                    if (allowedSumm >= wantedSumm)
                    {
                        DialogResult = DialogResult.OK;
                    }
                }
                else
                {
                    MessageBox.Show(checkResult, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public Client ReturnClient()
        {
            return client;
        }

        public float ReturnSumm()
        {
            return wantedSumm;
        }

        public int ReturnWantedTime()
        {
            return wantedTime;
        }
    }
}
