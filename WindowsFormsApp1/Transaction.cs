using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Transaction: Operation
    { 
        public Transaction(Client _sender, Client _recipient, float _value, long _id)
        {
            id = _id;
            type = "Transaction";
            time = 0;

            sender = _sender;
            recipient = _recipient;
            value = _value;

            sender.ChangeMoney(-value);
            sender.AddTransaction(this);

            recipient.ChangeMoney(value);
            recipient.AddTransaction(this);
        }

        public Transaction(long _id, int senderId, int recipientId, float _value)
        {
            id = _id;
            type = "Transaction";
            time = 0;

            sender = Storage.FindClientByID(senderId);
            if(sender == null)
            {
                throw new Exception("Transaction reading fail");
            }
            sender.AddTransaction(this);

            recipient = Storage.FindClientByID(recipientId);
            if (recipient == null)
            {
                throw new Exception("Transaction reading fail");
            }
            recipient.AddTransaction(this);

            value = _value;
        }

        public void Revoke()
        {
            if (sender != null)
            {
                sender.ChangeMoney(value);
                sender.RemoveTransaction(this);
            }

            if (recipient != null)
            {
                recipient.ChangeMoney(-value);
                recipient.RemoveTransaction(this);
            }
        }
    }
}
