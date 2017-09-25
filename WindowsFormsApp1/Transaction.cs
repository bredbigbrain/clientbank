using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Transaction
    { 
        public long id { get; }
        public Client sender { get; }
        public Client recipient { get; }
        public float value { get; }

        public Transaction(Client _sender, Client _recipient, float _value, long _id)
        {
            id = _id;

            sender = _sender;
            recipient = _recipient;
            value = _value;

            sender.CangeMoney(-value);
            sender.AddTransaction(this);

            recipient.CangeMoney(value);
            recipient.AddTransaction(this);

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(ToString());
            Console.WriteLine(sender.ToString());
            Console.WriteLine(recipient.ToString());
        }

        public Transaction(long _id, int senderId, int recipientId, float _value)
        {
            id = _id;

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
            sender.CangeMoney(value);
            sender.RemoveTransaction(this);

            recipient.CangeMoney(-value);
            recipient.RemoveTransaction(this);

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine(sender.ToString());
            Console.WriteLine(recipient.ToString());
        }

        public override string ToString()
        {
            return String.Concat("TransID: ", id, ", SenderID: ", sender.id, ", RecipienetID: ", recipient.id, ", Value: ", value);
        }
    }
}
