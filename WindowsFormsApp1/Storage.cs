using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WindowsFormsApp1
{
    public class Storage
    {
        private static List<Client> clients;
        private long transId;
        private int clientId;

        public Storage()
        {
            clients = new List<Client>();
            transId = 0;
        }

        public Storage(string path)
        {
            clients = new List<Client>();

            XmlDocument Doc = new XmlDocument();
            Doc.Load(path);

            foreach (XmlNode Node in Doc.DocumentElement.ChildNodes)
            {
                if (Node.Name.Equals("NumberOfTransactions"))
                {
                    long.TryParse(Node.InnerText, out transId);
                }
                else if (Node.Name.Equals("NumberOfClients"))
                {
                    int.TryParse(Node.InnerText, out clientId);
                }
                else if (Node.Name.Equals("Client"))
                {
                    int id;
                    string name;
                    float money;
                    bool prevC;
                    float income;

                    int.TryParse(Node.ChildNodes.Item(0).InnerText, out id);
                    name = Node.ChildNodes.Item(1).InnerText;
                    float.TryParse(Node.ChildNodes.Item(2).InnerText, out money);
                    bool.TryParse(Node.ChildNodes.Item(3).InnerText, out prevC);
                    float.TryParse(Node.ChildNodes.Item(4).InnerText, out income);

                    ReadClient(id, name, money, prevC, income);
                }
            }

            foreach (XmlNode clientNode in Doc.DocumentElement.ChildNodes)
            {
                if (clientNode.Name.Equals("Client") & clientNode.ChildNodes.Count == 6)
                    foreach (XmlNode transactNode in clientNode.ChildNodes.Item(5))
                    {
                        long id;
                        int senderId;
                        int recipientId;
                        float value;

                        long.TryParse(transactNode.ChildNodes.Item(0).InnerText, out id);
                        int.TryParse(transactNode.ChildNodes.Item(1).InnerText, out senderId);
                        int.TryParse(transactNode.ChildNodes.Item(2).InnerText, out recipientId);
                        float.TryParse(transactNode.ChildNodes.Item(3).InnerText, out value);

                        Transaction trans = new Transaction(id, senderId, recipientId, value);
                    }
            }
        }

        private void ReadClient(int id, string _name, float _money, bool _prevC, float _income)
        {
            clients.Add(new Client(id, _name, _money, _prevC, _income));
        }

        public void AddCient(string _name, float _money, bool _prevC, float _income)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == clientId)
                {
                    clientId = cl.id + 1;
                }
            }

            clients.Add(new Client(clientId, _name, _money, _prevC, _income));
            clientId++;
        }
        /*
        public void RemoveCientByID(int _id)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == _id)
                {
                    clients.Remove(cl);
                    return;
                }
            }
            Console.WriteLine("There is no client with ID: " + _id);
        }
        */
        public void Transaction(int senderId, int recipientId, float value)
        {
            if (recipientId == senderId)
            {
                Console.WriteLine("IDs is equal");
                return;
            }
            if (value <= 0)
            {
                Console.WriteLine("Value is <= 0");
                return;
            }

            Client sender = null;
            Client recipient = null;

            foreach (Client cl in clients)
            {
                if (cl.id == senderId)
                {
                    sender = cl;
                    if (sender.GetMoney() < value)
                    {
                        Console.WriteLine("Sender has not enough money: " + sender.GetMoney());
                        return;
                    }
                }
                if (cl.id == recipientId)
                {
                    recipient = cl;
                }
            }
            if (sender == null)
            {
                Console.WriteLine("There is no client with ID: " + senderId);
                return;
            }
            if (recipient == null)
            {
                Console.WriteLine("There is no client with ID: " + recipientId);
                return;
            }

            Transaction trans = new Transaction(sender, recipient, value, transId);
            transId++;
        }

        public void Credit(int recipientId, float value)
        {
            foreach(Client cl in clients)
            {
                if(cl.id == recipientId)
                {
                    Transaction trans = new Transaction(cl, value, transId);
                    return;
                }
            }
            
        }

        public void RevokeTransaction(int clientId, long transId)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == clientId)
                {
                    foreach (Transaction tran in cl.GetTransactionList())
                    {
                        if (tran.id == transId)
                        {
                            tran.Revoke();
                            return;
                        }
                    }
                    throw new Exception("Revoke error: no transaction");
                }
            }
            throw new Exception("Revoke error: no client");
        }

        public void ChangeClientsMoney(int id, float value)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == id)
                {
                    cl.CangeMoney(value);
                    return;
                }
            }
            Console.WriteLine("There is no client with ID: " + id);
        }

        public List<Client> GetClientList()
        {
            return clients;
        }

        public void ClearClientsList()
        {
            clientId = 0;
            transId = 0;
            clients.Clear();
        }

        public void PrintClienByID(int id)
        {
            Console.WriteLine("---------------------------------------------------");
            foreach (Client cl in clients)
            {
                if (cl.id == id)
                {
                    Console.WriteLine(cl.ToString());

                    List<Transaction> tr = cl.GetTransactionList();
                    foreach(Transaction t in tr)
                    {
                        Console.WriteLine(t.ToString());
                    }

                    return;
                }
            }
            Console.WriteLine("There is no client with ID: " + id);
        }

        public static Client FindClientByID( int id)
        {
            if (clients != null)
            {
                if (id == -1)
                    return Client.GetBankAsClient();
                foreach (Client cl in clients)
                {
                    if (cl.id == id)
                        return cl;
                }
            }
            throw new Exception("Cannont find client");
        }

        public void SaveXML(string path)
        {
            XmlWriterSettings sett = new XmlWriterSettings();
            sett.Indent = true;
            sett.IndentChars = " ";
            sett.NewLineChars = "\n";
            XmlWriter output = XmlWriter.Create(path, sett);
            output.WriteStartElement(path);

            output.WriteElementString("NumberOfTransactions", transId.ToString());
            output.WriteElementString("NumberOfClients", clientId.ToString());

            foreach (Client cl in clients)
            {
                output.WriteStartElement("Client");

                output.WriteElementString("ID", cl.id.ToString());
                output.WriteElementString("Name", cl.name);
                output.WriteElementString("Money", cl.GetMoney().ToString());
                output.WriteElementString("PrevConvictions", cl.prevConvictions.ToString());
                output.WriteElementString("MonthlyIncome", cl.monthlyIncome.ToString());

                if (cl.GetTransactionList() != null & cl.GetTransactionList().Count > 0)
                {
                    List<Transaction> sendedTrans = new List<Transaction>();

                    foreach (Transaction tr in cl.GetTransactionList())
                    {
                        if ((tr.sender.id == cl.id) || (tr.sender.id == -1))
                        {
                            sendedTrans.Add(tr);
                        }
                    }

                    if (sendedTrans.Count > 0)
                    {
                        output.WriteStartElement("Transactions");

                        foreach (Transaction tr in sendedTrans)
                        {
                            output.WriteStartElement("TransAct");

                            output.WriteElementString("ID", tr.id.ToString());
                            output.WriteElementString("SenderID", tr.sender.id.ToString());
                            output.WriteElementString("RecipientID", tr.recipient.id.ToString());
                            output.WriteElementString("Value", tr.value.ToString());

                            output.WriteEndElement();
                        }

                        output.WriteEndElement();
                    }
                }
                output.WriteEndElement();
            }
            output.WriteEndElement();

            output.Flush();
            output.Close();
        }
    }
}
