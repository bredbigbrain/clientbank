using System;
using System.IO;
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
    public partial class Form1 : Form
    {
        StorageView storageV;   //ViewModel базы
        string[,] transactions; //массив транзакций выбранного клиента, 2 мерный массив 0 - транзакции, 00 - тип, 01 - имя, 02 - сумма, 03 - id транзакции
        int id; //id выбранного клиента
        DataGridViewCellEventArgs e1;
        private bool isClietnsSheetUpdating;
        private bool isTransactionsSheetUpdating;


        private enum UI_States: int     //состояния интерфейса
        {
            BASE_NULL,
            BASE_INITIALIZED,
            BASE_EMPTY,
            CREDIT_SELECTED,
            TRANSACTION_SELECTED,
            OPERATION_NOT_SELECTED,
            NOTIFICATION_TRUE,
            NOTIFICATION_FALSE
        }

        public Form1()
        {
            InitializeComponent();
            UpdateUI(UI_States.BASE_NULL);
        }

        private void Form1_Load(object Sender, EventArgs e) 
        {
            storageV = new StorageView();
            e1 = null;
            isClietnsSheetUpdating = false;
            isTransactionsSheetUpdating = false;
        }

        void UpdateClientsSheets()  //обновить таблицу клиентов
        {
            isClietnsSheetUpdating = true;
            try
            {
                dataGridView1.Rows.Clear();
                label4.Text = storageV.GetDate().ToString();
                foreach (Client cl in storageV.GetClients())
                {
                    bool notif = cl.CheckRecipients.Count != 0;
                    CheckState state;
                    state = notif ? CheckState.Checked : CheckState.Unchecked;

                    dataGridView1.Rows.Add(cl.id, cl.name, cl.GetMoney(), state);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            isClietnsSheetUpdating = false;
        }   

        void ClearSheets()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
        }

        void UpdateTransactionsSheet(DataGridViewCellEventArgs e)   //обновить таблицу транзакций
        {
            if (!isClietnsSheetUpdating)
            {
                isTransactionsSheetUpdating = true;
                try
                {
                    dataGridView2.Rows.Clear();
                    e1 = e;

                    if (e1 != null)
                    {
                        if (int.TryParse(dataGridView1[0, e.RowIndex].Value.ToString(), out id))
                            if (storageV.GetClientsTransactions(id) != null)
                            {
                                transactions = storageV.GetClientsTransactions(id);

                                for (int i = 0; i < transactions.GetLength(0); i++)
                                {
                                    dataGridView2.Rows.Add(transactions[i, 0], transactions[i, 1], transactions[i, 2]);
                                }

                                OperationtDetermination();
                            }
                    }
                    else
                    {
                        UpdateUI(UI_States.OPERATION_NOT_SELECTED);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                isTransactionsSheetUpdating = false;
            }
        }   

        private void открытьToolStripMenuItem_Click(object Sender, EventArgs e) //открыть
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "xml files (*.xml)|*.xml";
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            string file_name = Path.GetFileName(dialog.FileName);

            try
            {
                storageV.OpenBaseFile(file_name);

                if (storageV.GetClients() == null)
                    UpdateUI(UI_States.BASE_EMPTY);
                else
                    UpdateUI(UI_States.BASE_INITIALIZED);

                UpdateClientsSheets();
                UpdateTransactionsSheet(e1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Open File Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                storageV.ClearClientsList();
                ClearSheets();
                UpdateUI(UI_States.BASE_NULL);
            }
        }   

        private void сохранитьToolStripMenuItem_Click(object Sender, EventArgs e)   //сохранить
        {
            try
            {
                storageV.SaveBaseXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save File Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }   

        private void новаяБазаToolStripMenuItem_Click_1(object Sender, EventArgs e) //новая база
        {
            e1 = null;
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Create XML file";
            save.Filter = "xml files (*.xml)|*.xml";
            if (save.ShowDialog() != DialogResult.OK)
                return;
            string file_name = Path.GetFileName(save.FileName);

            storageV.CreateNewBase(file_name);

            UpdateUI(UI_States.BASE_EMPTY);
            UpdateClientsSheets();
            UpdateTransactionsSheet(e1);
        }       

        private void сохранитьКакToolStripMenuItem_Click(object Sender, EventArgs e)    //сохранить как...
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "xml files (*.xml)|*.xml";
            if (save.ShowDialog() != DialogResult.OK)
                return;
            string file_name = Path.GetFileName(save.FileName);
            
            try
            {
                storageV.SaveBaseAsXML(file_name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save File Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }   

        private void button1_Click(object Sender, EventArgs e)  //добавить клиента
        {
            try
            {
                Form2 form2 = new Form2();
                form2.ShowDialog();
                if (form2.DialogResult == DialogResult.OK)
                {
                    bool prevConvict;
                    if(form2.ReturnData()[2].Equals("Unchecked"))
                    {
                        prevConvict = false;
                    }
                    else
                    {
                        prevConvict = true;
                    }
                    storageV.AddClient(form2.ReturnData()[0], float.Parse(form2.ReturnData()[1]), prevConvict, float.Parse(form2.ReturnData()[3]));

                    UpdateUI(UI_States.BASE_INITIALIZED);

                    UpdateClientsSheets();
                    UpdateTransactionsSheet(e1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Incorrect values", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }   

        private void button3_Click(object Sender, EventArgs e)  //удалить всех
        {
            try
            {
                e1 = null;
                transactions = null;

                storageV.ClearClientsList();

                UpdateClientsSheets();
                UpdateTransactionsSheet(e1);

                UpdateUI(UI_States.BASE_EMPTY);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object Sender, EventArgs e)  //перевод средств
        {
            try
            {
                Form3 form3 = new Form3(storageV);
                form3.ShowDialog();
                if (form3.DialogResult == DialogResult.OK)
                {
                    string[] data = form3.ReturnData();
                    storageV.Transaction(int.Parse(data[0]), int.Parse(data[1]), float.Parse(data[2]));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateClientsSheets();
            UpdateTransactionsSheet(e1);
        }

        private void button2_Click(object Sender, EventArgs e)  //удаление клиента
        {

        }

        private void dataGridView1_CellEnter(object Sender, DataGridViewCellEventArgs e)    //выбор клиента в таблице
        {
            UpdateTransactionsSheet(e);
            
            if(dataGridView1[3, e.RowIndex].Value.Equals(CheckState.Checked))
            {
                UpdateUI(UI_States.NOTIFICATION_TRUE);
            }
            else
            {
                UpdateUI(UI_States.NOTIFICATION_FALSE);
            }
        }

        private void button6_Click(object Sender, EventArgs e)  //отмена транзакции
        {
            try
            {
                int rowIdex = dataGridView1.CurrentCell.RowIndex;
                string cellValue = dataGridView1.Rows[rowIdex].Cells[0].Value.ToString();

                int clientID = int.Parse(cellValue);
                long transID = long.Parse(transactions[dataGridView2.CurrentCell.RowIndex, 3]);
                Storage.RevokeTransaction(clientID, transID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            UpdateClientsSheets();
            UpdateTransactionsSheet(e1);
        }

        private void button8_Click(object Sender, EventArgs e)  //кредит
        {
            try
            {
                float a;
                string checkResult = storageV.AllowedCreditSumm(Storage.FindClientByID(id), 1.5f, 12);

                if (float.TryParse(checkResult, out a))
                {
                        Form4 form4 = new Form4(storageV, id);
                        form4.ShowDialog();
                    if (form4.DialogResult == DialogResult.OK)
                    {
                        Client cl = form4.ReturnClient();
                        storageV.Credit(cl, form4.ReturnSumm(), form4.ReturnWantedTime());
                    }
                }
                else
                {
                    MessageBox.Show(checkResult, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                UpdateClientsSheets();
                UpdateTransactionsSheet(e1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateUI(UI_States state)  //обновить активность элементов интерфейса
        {
            switch (state)
            {
                case UI_States.BASE_NULL:
                    {
                        button1.Enabled = false;
                        button2.Enabled = false;
                        button3.Enabled = false;
                        button4.Enabled = false;
                        button5.Enabled = false;
                        button6.Enabled = false;
                        button7.Enabled = false;
                        button8.Enabled = false;
                        button9.Enabled = false;
                        button10.Enabled = false;
                        button11.Enabled = false;
                        button12.Enabled = true;

                        label3.Visible = false;
                        label4.Visible = false;

                        ToolStripMenuItem fileItem = menuStrip1.Items[0] as ToolStripMenuItem;
                        fileItem.DropDownItems[1].Enabled = false;
                        fileItem.DropDownItems[2].Enabled = false;

                        break;
                    }
                case UI_States.BASE_INITIALIZED:
                    {
                        button1.Enabled = true;
                        button3.Enabled = true;
                        button4.Enabled = true;
                        button8.Enabled = true;
                        button10.Enabled = false;
                        button11.Enabled = true;
                        button12.Enabled = true;

                        label3.Visible = true;
                        label4.Visible = true;

                        ToolStripMenuItem fileItem = menuStrip1.Items[0] as ToolStripMenuItem;
                        fileItem.DropDownItems[1].Enabled = true;
                        fileItem.DropDownItems[2].Enabled = true;
                        break;
                    }
                case UI_States.BASE_EMPTY:
                    {
                        button1.Enabled = true;
                        button2.Enabled = false;
                        button3.Enabled = false;
                        button4.Enabled = false;
                        button5.Enabled = false;
                        button6.Enabled = false;
                        button7.Enabled = false;
                        button8.Enabled = false;
                        button9.Enabled = false;
                        button10.Enabled = false;
                        button11.Enabled = false;
                        button12.Enabled = true;

                        label3.Visible = true;
                        label4.Visible = true;

                        break;
                    }
                case UI_States.OPERATION_NOT_SELECTED:
                    {
                        button6.Enabled = false;
                        button7.Enabled = false;
                        break;
                    }
                case UI_States.CREDIT_SELECTED:
                    {
                        button6.Enabled = false;
                        button7.Enabled = true;
                        break;
                    }
                case UI_States.TRANSACTION_SELECTED:
                    {
                        button6.Enabled = true;
                        button7.Enabled = false;
                        break;
                    }
                case UI_States.NOTIFICATION_TRUE:
                    {
                        button10.Enabled = true;
                        break;
                    }
                case UI_States.NOTIFICATION_FALSE:
                    {
                        button10.Enabled = false;
                        break;
                    }
            }
                
        }

        private void button11_Click(object Sender, EventArgs e) //следующий месяц
        {
            label4.Text = storageV.NextMonth().ToString();

            UpdateClientsSheets();
            UpdateTransactionsSheet(e1);
        }

        private void dataGridView2_CellEnter(object Sender, DataGridViewCellEventArgs e)    //выбор операции в таблице
        {
            if (!isTransactionsSheetUpdating)
            {
                OperationtDetermination();
            }
        }

        private void OperationtDetermination()  //определение типа выбранной операции из таблицы операций
        {
            if (dataGridView2.CurrentCell != null)
            {
                if (transactions[dataGridView2.CurrentCell.RowIndex, 1].Equals(Client.GetBankAsClient().name))
                {
                    UpdateUI(UI_States.CREDIT_SELECTED);
                }
                else
                {
                    UpdateUI(UI_States.TRANSACTION_SELECTED);
                }
            }
        }

        private void button7_Click(object Sender, EventArgs e)  //платеж по кредиту
        {
            try
            {
                int rowIdex = dataGridView1.CurrentCell.RowIndex;
                string cellValue = dataGridView1.Rows[rowIdex].Cells[0].Value.ToString();

                int clientID = int.Parse(cellValue);
                long creditID = long.Parse(transactions[dataGridView2.CurrentCell.RowIndex, 3]);

                CreditPayment_Form form = new CreditPayment_Form(clientID, creditID);

                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    UpdateClientsSheets();
                    UpdateTransactionsSheet(e1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button12_Click(object Sender, EventArgs e)
        {
            try
            {
                AssistantForm form = new AssistantForm();
                
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button10_Click(object sender, EventArgs e) //открыть уведомления
        {
            try
            {
                int clientID = (int)dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value;
                Client cl = Storage.FindClientByID(clientID);

                foreach (int recipientID in cl.CheckRecipients)
                {
                    TransNotificationForm form = new TransNotificationForm(Storage.FindClientByID(recipientID).name);
                    form.ShowDialog();

                    if (form.DialogResult == DialogResult.OK)
                    {
                        storageV.Transaction(clientID, recipientID, form.ReturnData());
                        cl.TransactionChecked(recipientID, true);

                        UpdateClientsSheets();
                        UpdateTransactionsSheet(e1);
                    }
                    else
                    {
                        cl.TransactionChecked(recipientID, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
