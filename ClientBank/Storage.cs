using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClientBank
{
    public class Storage
    {
        private static List<Client> clients;
        private long transId;

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

            foreach (XmlNode clientNode in Doc.DocumentElement.ChildNodes)
            {
                int id;
                string name;
                float money;

                int.TryParse(clientNode.ChildNodes.Item(0).InnerText, out id);
                name = clientNode.ChildNodes.Item(1).InnerText;
                float.TryParse(clientNode.ChildNodes.Item(2).InnerText, out money);

                AddCient(id, name, money);
            }

            foreach (XmlNode clientNode in Doc.DocumentElement.ChildNodes)
            {
                if (clientNode.ChildNodes.Count >= 4)
                    foreach (XmlNode transactNode in clientNode.ChildNodes.Item(3))
                    {
                        long id;
                        int senderId;
                        int recipientId;
                        float value;

                        long.TryParse(clientNode.ChildNodes.Item(0).InnerText, out id);
                        int.TryParse(clientNode.ChildNodes.Item(1).InnerText, out senderId);
                        int.TryParse(clientNode.ChildNodes.Item(2).InnerText, out recipientId);
                        float.TryParse(clientNode.ChildNodes.Item(3).InnerText, out value);
                    }
            }
        }

        public void AddCient(int _id, string _name, float _money)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == _id)
                {
                    Console.WriteLine("Clien with ID: " + _id + "is already exists");
                    return;
                }
            }
            
            clients.Add(new Client(_id, _name, _money));
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

            Transaction transa = new Transaction(sender, recipient, value, transId);
            transId++;
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
                    Console.WriteLine("There is no transaction with ID: " + transId);
                }
            }
            Console.WriteLine("There is no client with ID: " + clientId);
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

        public void PrintClientList()
        {
            Console.WriteLine("---------------------------------------------------");
            foreach (Client cl in clients)
            {
                Console.WriteLine(cl.ToString());
            }
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
                foreach (Client cl in clients)
                {
                    if (cl.id == id)
                        return cl;
                }
            return null;
        }

        public void SaveXML(string path)
        {
            XmlWriterSettings sett = new XmlWriterSettings();
            sett.Indent = true;
            sett.IndentChars = " ";
            sett.NewLineChars = "\n";
            XmlWriter output = XmlWriter.Create(path, sett);
            output.WriteStartElement(path);

            foreach (Client cl in clients)
            {
                output.WriteStartElement("Client");

                output.WriteElementString("ID", cl.id.ToString());
                output.WriteElementString("Name", cl.name);
                output.WriteElementString("Money", cl.GetMoney().ToString());

                if ( cl.GetTransactionList() != null & cl.GetTransactionList().Count > 0)
                {
                    output.WriteStartElement("Transactions");

                    foreach (Transaction tr in cl.GetTransactionList())
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
                output.WriteEndElement();
            }
            output.WriteEndElement();

            output.Flush();
            output.Close();
        }
    }
}
