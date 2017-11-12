using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsFormsApp1
{
    public class Client
    {
        public int id { get; private set; }
        public string name { get; private set; }
        private float money;
        public bool prevConvictions { get; private set; }
        public float monthlyIncome { get; private set; }
        public int delayMonths { get; set; }
        public List<int> CheckRecipients { get; private set; }

        private TransactionAnaliser analiserTR;
        private List<Operation> transactions;

        public Client(int _id, string _name, float _money, bool _prevC, float _income, int _delayMonths)
        {
            id = _id;
            name = _name;
            money = _money;
            prevConvictions = _prevC;
            monthlyIncome = _income;
            transactions = new List<Operation>();
            delayMonths = _delayMonths;
            analiserTR = new TransactionAnaliser(this);
            CheckRecipients = new List<int>();
        }

        public float GetMoney()
        {
            return money;
        }
        
        public void ChangeMoney(float value)
        {
            money += value;
        }

        public void AddTransaction(Operation oper)
        {
            transactions.Add(oper);
            if(oper.type == OperationTypes.Transaction)
                analiserTR.AddTransaction((Transaction)oper);
        }

        public void RemoveTransaction(Operation oper)
        {
            if (transactions.Contains(oper))
            {
                transactions.Remove(oper);
                if (oper.type == OperationTypes.Transaction)
                    analiserTR.RemoveTransaction((Transaction)oper);
            }
        }
        
        public List<Operation> GetTransactionList()
        {
            return transactions;
        }
        
        public void RecieveSalary()
        {
            money += monthlyIncome;
        }

        public void AccrualCreditsInterest()
        {
            foreach(Operation op in transactions)
            {
                if (op.type == OperationTypes.Credit)
                {
                    Credit cr = op as Credit;
                    cr.AccrualInterest();
                }
            }
        }

        public void AnaliseTransactions()
        {
            analiserTR.NextMonth();
            if(analiserTR.CheckRecipients.Count > 0)
            {
                CheckRecipients = analiserTR.CheckRecipients;
            }
        }

        public void TransactionChecked(int recipientID, bool isCheckCorrect)
        {
            analiserTR.SetCheckResult(recipientID, isCheckCorrect);
            CheckRecipients = analiserTR.CheckRecipients;
        }

        public void SaveAnaliserDataXml(XmlWriter writer)
        {
            if (analiserTR != null)
            {
                analiserTR.SaveXml(writer);
            }
        }

        public void SetAnaliserData(List<string> data)
        {
            analiserTR = new TransactionAnaliser(data, this);
            if (analiserTR.CheckRecipients.Count > 0)
            {
                CheckRecipients = analiserTR.CheckRecipients;
            }
        }

        public static Client GetBankAsClient()
        {
            return new Client(-1, "Сельхозбанк", 0, false, 0, 0);
        }
    }
}
