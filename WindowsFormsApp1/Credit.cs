using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Credit: Operation
    {
        public Credit(Client _Recipient, float _value, long _id, int _time)
        {
            id = _id;
            type = OperationTypes.Credit;

            if (_time < 0)
                throw new Exception("Недопустимый срок");
            if(_value < 0)
                throw new Exception("Недопустимая сумма");

            time = _time;

            Sender = Client.GetBankAsClient();
            Recipient = _Recipient;
            value = _value;

            Recipient.ChangeMoney(value);
            Recipient.AddTransaction(this);
        }

        public Credit(long _id, int senderId, int recipientId, float _value, int _time)
        {
            id = _id;
            type = OperationTypes.Credit;
            time = _time;

            Sender = Storage.FindClientByID(senderId);
            if (Sender == null)
            {
                throw new Exception("Transaction reading fail");
            }

            Recipient = Storage.FindClientByID(recipientId);
            if (Recipient == null)
            {
                throw new Exception("Transaction reading fail");
            }

            value = _value;

            Sender.AddTransaction(this);
            Recipient.AddTransaction(this);
        }

        public void AccrualInterest()
        {
            int k = 1;
            if (time < 0)
            {
                k = 2;
                Recipient.delayMonths++;
            }
            value = value * (1 + 0.015f * k);
            time--;
        }

        public void MakePayment(float payment)
        {
            if (payment >= 0)
            {
                value -= payment;
                Recipient.ChangeMoney(-payment);

                if (value == 0)
                {
                    Recipient.RemoveTransaction(this);
                }
                else if (value < 0)
                {
                    Recipient.RemoveTransaction(this);
                    Recipient.ChangeMoney(-value);
                }
            }
        }
    }
}
