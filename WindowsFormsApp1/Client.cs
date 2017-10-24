using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public float GetMoney()
        {
            return money;
        }
        
        public void ChangeMoney(float value)
        {
            money += value;
        }

        public void AddTransaction(Operation trans)
        {
            transactions.Add(trans);
        }

        public void RemoveTransaction(Operation trans)
        {
            if(transactions.Contains(trans))
                transactions.Remove(trans);
        }
        /*
        public void RevokeTransaction(Transaction trans)
        {
        }
        */
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
                if (op.type.Equals("Credit"))
                {
                    Credit cr = op as Credit;
                    cr.AccrualInterest();
                }
            }
        }

        public static Client GetBankAsClient()
        {
            return new Client(-1, "Сельхозбанк", 0, false, 0, 0);
        }
    }
}
