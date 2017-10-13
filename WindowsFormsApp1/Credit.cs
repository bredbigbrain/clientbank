using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Credit: Operation
    {
        public Credit(Client _recipient, float _value, long _id, int _time)
        {
            id = _id;
            type = "Credit";

            if (_time < 0)
                throw new Exception("Недопустимый срок");
            if(_value < 0)
                throw new Exception("Недопустимая сумма");

            time = _time;

            sender = Client.GetBankAsClient();
            recipient = _recipient;
            value = _value;

            recipient.ChangeMoney(value);
            recipient.AddTransaction(this);
        }

        public Credit(long _id, int senderId, int recipientId, float _value, int _time)
        {
            id = _id;
            type = "Credit";
            time = _time;

            sender = Storage.FindClientByID(senderId);
            if (sender == null)
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

        public void AccrualInterest()
        {
            int k = 1;
            if (time < 0)
            {
                k = 2;
                recipient.delayMonths++;
            }
            value = value * (1 + 0.015f * k);
            time--;
        }

        public void MakePayment(float payment)
        {
            value -= payment;

            if(value == 0)
            {
                recipient.RemoveTransaction(this);                
            }
            if(value < 0)
            {
                recipient.RemoveTransaction(this);
                recipient.ChangeMoney(-value);
            }
        }
    }
}
