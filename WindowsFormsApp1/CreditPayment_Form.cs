﻿using System;
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
    public partial class CreditPayment_Form : Form
    {
        int clientID;
        long creditID;
        Credit credit;
        Client client;

        public CreditPayment_Form(int clID, long crID)
        {
            InitializeComponent();
            clientID = clID;
            creditID = crID;
        }

        private void CreditPayment_Form_Load(object sender, EventArgs e)
        {
            
            client = Storage.FindClientByID(clientID);
            label2.Text = client.name;
            label3.Text = client.GetMoney().ToString();

            foreach (Credit cr in client.GetTransactionList())
            {
                if(cr.id == creditID)
                {
                    label5.Text = cr.value.ToString();
                    label7.Text = cr.time.ToString();
                    credit = cr;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            float value;

            if (float.TryParse(textBox1.Text, out value))
            {
                credit.MakePayment(value);
                DialogResult = DialogResult.OK;
            }
        }
    }
}