using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Client
    {
        public int id { get; }
        public string name { get; }
        private float money;
        public bool prevConvictions { get; }
        public float monthlyIncome { get; }

        private List<Transaction> transactions;

        public Client(int _id, string _name, float _money, bool _prevC, float _income)
        {
            id = _id;
            name = _name;
            money = _money;
            prevConvictions = _prevC;
            monthlyIncome = _income;
            transactions = new List<Transaction>();
        }

        public float GetMoney()
        {
            return money;
        }
        
        public void CangeMoney(float value)
        {
            money += value;
        }

        public void AddTransaction(Transaction trans)
        {
            transactions.Add(trans);
        }

        public void RemoveTransaction(Transaction trans)
        {
            if(transactions.Contains(trans))
                transactions.Remove(trans);
        }

        public void RevokeTransaction(Transaction trans)
        {
        }

        public List<Transaction> GetTransactionList()
        {
            return transactions;
        }

        public override string ToString()
        {
            return String.Concat("ID: ", id, ", Name: ", name, ", Money: ", money);
        }

        public static Client GetBankAsClient()
        {
            return new Client(-1, "Сельхозбанк", 0, false, 0);
        }
    }
}
