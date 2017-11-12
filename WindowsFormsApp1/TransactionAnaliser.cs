using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsFormsApp1
{
    delegate void Check(int recipientID);

    public class TransactionAnaliser
    {
        private List<TransactionSequence> sequences;
        private Client sender;
        public List<int> CheckRecipients { get; private set; }

        public TransactionAnaliser(Client _sender)
        {
            sequences = new List<TransactionSequence>();
            CheckRecipients = new List<int>();
            sender = _sender;
        }

        public TransactionAnaliser(List<string> data, Client _sender)
        {
            sequences = new List<TransactionSequence>();
            CheckRecipients = new List<int>();
            sender = _sender;

            for (int i = 0; i < data.Count / 6; i++)
            {
                TransactionSequence seq = new TransactionSequence(Storage.FindClientByID(int.Parse(data[i * 6])), AddCheck);
                seq.SetData(data.GetRange(i * 6 + 1, 5));
                sequences.Add(seq);
            }
        }

        public void AddTransaction(Transaction trans)
        {
            if (trans != null & trans.Sender == sender)
            {
                bool added = false;
                if (sequences.Count != 0)
                {
                    foreach (TransactionSequence ts in sequences)
                    {
                        if (ts.Recipient.Equals(trans.Recipient))
                        {
                            ts.AddTransaction();
                            added = true;
                            break;
                        }
                    }
                }

                if (!added || sequences.Count == 0)
                {
                    TransactionSequence seq = new TransactionSequence(trans.Recipient, AddCheck);
                    seq.AddTransaction();
                    sequences.Add(seq);
                }
            }
        }

        public void RemoveTransaction(Transaction trans)
        {
            if (trans != null & trans.Sender == sender)
            {
                bool removed = false;
                if (sequences.Count != 0)
                {
                    foreach (TransactionSequence ts in sequences)
                    {
                        if (ts.Recipient.Equals(trans.Recipient))
                        {
                            ts.RemoveTransaction();
                            removed = true;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        throw new Exception("Sequences analiser error");
                    }
                }
            }
        }

        public void NextMonth()
        {
            CheckRecipients.Clear();

            foreach (TransactionSequence ts in sequences)
            {
                ts.NextMonth();
            }
        }

        public void SetCheckResult(int recipientID, bool isCheckCorrect)
        {
            foreach (TransactionSequence ts in sequences)
            {
                if(ts.Recipient.id == recipientID)
                {
                    ts.SetCheckResult(isCheckCorrect);
                    CheckRecipients.Remove(recipientID);
                    return;
                }
            }
            throw new Exception("Sequences analiser error");
        }

        private void AddCheck(int recipientID)
        {
            CheckRecipients.Add(recipientID);
        }

        public void SaveXml(XmlWriter output)
        {
            if(sequences != null && sequences.Count > 0)
            {
                output.WriteStartElement("AnaliserData");



                foreach (TransactionSequence ts in sequences)
                {
                    ts.SaveXml(output);
                }

                output.WriteEndElement();
            }
        }
    }

    class TransactionSequence
    {
        public Client Recipient { get; private set; }

        private byte tailLenth;
        private byte sequence;
        private byte errorChecks;
        private bool waitingCheckResult;
        private byte thisMonthTransactions;
        Check check;

        public TransactionSequence(Client recip, Check ch) 
        {
            Recipient = recip;
            tailLenth = 2;
            sequence = 0;
            errorChecks = 0;
            waitingCheckResult = false;
            thisMonthTransactions = 0;
            check = ch;
        }

        public void AddTransaction()
        {
            thisMonthTransactions++;
        }

        public void RemoveTransaction()
        {
            thisMonthTransactions--;
        }

        public void NextMonth()
        {
            if (thisMonthTransactions > 0)
            {
                sequence++;
            }
            else
            {
                if (!waitingCheckResult)
                {
                    if (sequence >= tailLenth)
                    {
                        check(Recipient.id);
                        waitingCheckResult = true;
                    }
                    else
                    {
                        sequence = 0;
                    }
                }
                else
                {
                    SetCheckResult(false);
                }
            }

            thisMonthTransactions = 0;
        }

        public void SetCheckResult(bool isCorrect)
        {
            if(isCorrect)
            {
                sequence++;
            }
            else
            {
                errorChecks++;
                sequence = 0;

                if(errorChecks == 2)
                {
                    errorChecks = 0;
                    tailLenth++;
                }
            }
            waitingCheckResult = false;
        }

        public void SetData(List<string> data)
        {
            tailLenth = byte.Parse(data[0]);
            sequence = byte.Parse(data[1]);
            errorChecks = byte.Parse(data[2]);
            waitingCheckResult = bool.Parse(data[3]);
            thisMonthTransactions = byte.Parse(data[4]);

            if(waitingCheckResult)
            {
                check(Recipient.id);
            }
        }

        public void SaveXml(XmlWriter output)
        {
            output.WriteStartElement("Sequence");

            output.WriteElementString("RecipientID", Recipient.id.ToString());
            output.WriteElementString("Tail", tailLenth.ToString());
            output.WriteElementString("Sequen", sequence.ToString());
            output.WriteElementString("ErrorChs", errorChecks.ToString());
            output.WriteElementString("Waiting", waitingCheckResult.ToString());
            output.WriteElementString("Transactions", thisMonthTransactions.ToString());

            output.WriteEndElement();
        }
    }
}
