using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    delegate void Check(int recipientID);

    public class TransactionAnaliser
    {
        private List<TransactionSequence> sequences;
        private Client Sender;
        public List<int> CheckRecipients { get; private set; }

        public TransactionAnaliser(Client _Sender)
        {
            sequences = new List<TransactionSequence>();
            CheckRecipients = new List<int>();
            Sender = _Sender;
        }

        public void AddTransaction(Transaction trans)
        {
            if (trans != null & trans.Sender == Sender)
            {
                bool added = false;
                if (sequences.Count != 0)
                {
                    foreach (TransactionSequence ts in sequences)
                    {
                        if (ts.Recipient.Equals(trans.Recipient))
                        {
                            ts.AddTransaction(trans);
                            added = true;
                            break;
                        }
                    }
                }

                if (!added || sequences.Count == 0)
                {
                    TransactionSequence seq = new TransactionSequence(trans.Recipient, AddCheck);
                    seq.AddTransaction(trans);
                    sequences.Add(seq);
                }
            }
        }
         
        public void RemoveTransaction(Transaction trans)
        {
            if(sequences.Count == 0)
            {
                throw new Exception("Sequences analiser error");
            }
            else
            {
                if (trans != null & trans.Sender == Sender)
                {
                    bool removed = false;
                    if (sequences.Count != 0)
                    {
                        foreach (TransactionSequence ts in sequences)
                        {
                            if (ts.Recipient.Equals(trans.Recipient))
                            {
                                ts.RemoveTransaction(trans);
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
                    return;
                }
            }
            throw new Exception("Sequences analiser error");
        }

        private void AddCheck(int recipientID)
        {
            CheckRecipients.Add(recipientID);
        }
    }

    class TransactionSequence
    {
        public Client Recipient { get; private set; }

        private int tailLenth;
        private int sequence;
        private byte errorChecks;
        private bool waitingCheckResult;
        private List<Transaction> currentMonthTransactions;
        Check check;

        public TransactionSequence(Client recip, Check ch) 
        {
            Recipient = recip;
            tailLenth = 2;
            sequence = 0;
            errorChecks = 0;
            waitingCheckResult = false;
            currentMonthTransactions = new List<Transaction>();
            check = ch;
        }

        public void AddTransaction(Transaction trans)
        {
            currentMonthTransactions.Add(trans);
        }

        public void RemoveTransaction(Transaction trans)
        {
            if (currentMonthTransactions.Contains(trans))
            {
                currentMonthTransactions.Remove(trans);
            }
            else
            {
                throw new Exception("Transaction sequence error");
            }
        }

        public void NextMonth()
        {
            if (currentMonthTransactions.Count > 0)
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

            currentMonthTransactions.Clear();
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
    }
}
