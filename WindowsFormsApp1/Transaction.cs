using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Transaction: Operation
    { 
        public Transaction(Client _Sender, Client _Recipient, float _value, long _id)
        {
            id = _id;
            type = "Transaction";
            time = 0;

            Sender = _Sender;
            Recipient = _Recipient;
            value = _value;

            Sender.ChangeMoney(-value);
            Sender.AddTransaction(this);

            Recipient.ChangeMoney(value);
            Recipient.AddTransaction(this);
        }

        public Transaction(long _id, int senderId, int recipientId, float _value)
        {
            id = _id;
            type = "Transaction";
            time = 0;

            Sender = Storage.FindClientByID(senderId);
            if(Sender == null)
            {
                throw new Exception("Transaction reading fail");
            }
            Sender.AddTransaction(this);

            Recipient = Storage.FindClientByID(recipientId);
            if (Recipient == null)
            {
                throw new Exception("Transaction reading fail");
            }
            Recipient.AddTransaction(this);

            value = _value;
        }

        public void Revoke()
        {
            if (Sender != null)
            {
                Sender.ChangeMoney(value);
                Sender.RemoveTransaction(this);
            }

            if (Recipient != null)
            {
                Recipient.ChangeMoney(-value);
                Recipient.RemoveTransaction(this);
            }
        }
    }
}
