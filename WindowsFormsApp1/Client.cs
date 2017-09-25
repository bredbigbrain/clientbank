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

        private List<Transaction> transactions;

        public Client(int _id, string _name, float _money)
        {
            id = _id;
            name = _name;
            money = _money;
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
    }
}
