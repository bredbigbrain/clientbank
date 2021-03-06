﻿using System;
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
        private int date;
        private const string VERSION = "2";

        public Storage()
        {
            clients = new List<Client>();
            transId = 0;
            date = 0;
        }

        public int GetDate()
        {
            return date;
        }

        public Storage(string path)
        {
            clients = new List<Client>();

            XmlDocument Doc = new XmlDocument();
            Doc.Load(path);

            if (Doc.DocumentElement.FirstChild.Name.Equals("Version"))
            {
                if (!Doc.DocumentElement.FirstChild.InnerText.Equals(VERSION))
                {
                    throw new Exception("Файл неподходящей версии. " + Doc.DocumentElement.FirstChild.InnerText);
                }
            }
            else
            {
                throw new Exception("Файл неподходящей версии");
            }

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
                else if (Node.Name.Equals("Date"))
                {
                    int.TryParse(Node.InnerText, out date);
                }
                else if (Node.Name.Equals("Client"))
                {
                    int id, delayMonths;
                    string name;
                    float money;
                    bool prevC;
                    float income;

                    int.TryParse(Node.ChildNodes.Item(0).InnerText, out id);
                    name = Node.ChildNodes.Item(1).InnerText;
                    float.TryParse(Node.ChildNodes.Item(2).InnerText, out money);
                    bool.TryParse(Node.ChildNodes.Item(3).InnerText, out prevC);
                    float.TryParse(Node.ChildNodes.Item(4).InnerText, out income);
                    int.TryParse(Node.ChildNodes.Item(5).InnerText, out delayMonths);

                    ReadClient(id, name, money, prevC, income, delayMonths);
                }
            }

            foreach (XmlNode clientNode in Doc.DocumentElement.ChildNodes)
            {
                if (clientNode.Name.Equals("Client") & clientNode.ChildNodes.Count >= 7)
                {
                    foreach (XmlNode transactNode in clientNode.ChildNodes.Item(6))
                    {
                        long id;
                        int senderId, recipientId, time;
                        float value;

                        long.TryParse(transactNode.ChildNodes.Item(0).InnerText, out id);
                        int.TryParse(transactNode.ChildNodes.Item(1).InnerText, out senderId);
                        int.TryParse(transactNode.ChildNodes.Item(2).InnerText, out recipientId);
                        float.TryParse(transactNode.ChildNodes.Item(3).InnerText, out value);
                        int.TryParse(transactNode.ChildNodes.Item(4).InnerText, out time);

                        Operation op;

                        if (senderId == -1)
                            op = new Credit(id, senderId, recipientId, value, time);
                        else
                            op = new Transaction(id, senderId, recipientId, value);
                    }
                }
                if (clientNode.Name.Equals("Client") & clientNode.ChildNodes.Count >= 8)
                {
                    List<string> data = new List<string>();
                    foreach (XmlNode sequenceNode in clientNode.ChildNodes.Item(7))
                    {
                        foreach (XmlNode node in sequenceNode.ChildNodes)
                        {
                            data.Add(node.InnerText);
                        }
                    }
                    Client client = FindClientByID(int.Parse(clientNode.ChildNodes.Item(0).InnerText));
                    client.SetAnaliserData(data);
                }
            }
        }

        public int NextMonth()
        {
            date++;

            foreach(Client cl in clients)
            {
                cl.RecieveSalary();
                cl.AccrualCreditsInterest();
                cl.AnaliseTransactions();
            }

            return date;
        }

        private void ReadClient(int id, string _name, float _money, bool _prevC, float _income, int delayMonths)
        {
            clients.Add(new Client(id, _name, _money, _prevC, _income, delayMonths));
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

            clients.Add(new Client(clientId, _name, _money, _prevC, _income, 0));
            clientId++;
        }

        public void Transaction(int senderId, int recipientId, float value)
        {
            if (recipientId == senderId)
            {
                throw new Exception("IDs is equal");
            }
            if (value <= 0)
            {
                throw new Exception("Value is <= 0");
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
                        throw new Exception("sender has not enough money: " + sender.GetMoney());
                    }
                }
                if (cl.id == recipientId)
                {
                    recipient = cl;
                }
            }
            if (sender == null)
            {
                throw new Exception("There is no client with ID: " + senderId);
            }
            if (recipient == null)
            {
                throw new Exception("There is no client with ID: " + recipientId);
            }

            Transaction trans = new Transaction(sender, recipient, value, transId);
            transId++;
        }

        public void Credit(int recipientId, float value, int time)
        {
            foreach(Client cl in clients)
            {
                if(cl.id == recipientId)
                {
                    Credit trans = new Credit(cl, value, transId, time);
                    transId++;
                    return;
                }
            }
        }

        public static void RevokeTransaction(int clientId, long transId)
        {
            foreach (Client cl in clients)
            {
                if (cl.id == clientId)
                {
                    foreach (Operation oper in cl.GetTransactionList())
                    {
                        if (oper.id == transId)
                        {
                            ((Transaction)oper).Revoke();
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
                    cl.ChangeMoney(value);
                    return;
                }
            }
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

            output.WriteElementString("Version", VERSION);
            output.WriteElementString("NumberOfTransactions", transId.ToString());
            output.WriteElementString("NumberOfClients", clientId.ToString());
            output.WriteElementString("Date", date.ToString());

            foreach (Client cl in clients)
            {
                output.WriteStartElement("Client");

                output.WriteElementString("ID", cl.id.ToString());
                output.WriteElementString("Name", cl.name);
                output.WriteElementString("Money", cl.GetMoney().ToString());
                output.WriteElementString("PrevConvictions", cl.prevConvictions.ToString());
                output.WriteElementString("MonthlyIncome", cl.monthlyIncome.ToString());
                output.WriteElementString("DelayMonths", cl.delayMonths.ToString());

                if (cl.GetTransactionList() != null & cl.GetTransactionList().Count > 0)
                {
                    List<Operation> sendedTrans = new List<Operation>();

                    foreach (Operation tr in cl.GetTransactionList())
                    {
                        if ((tr.Sender.id == cl.id) || (tr.Sender.id == -1))
                        {
                            sendedTrans.Add(tr);
                        }
                    }

                    if (sendedTrans.Count > 0)
                    {
                        output.WriteStartElement("Operations");

                        foreach (Operation tr in sendedTrans)
                        {
                            output.WriteStartElement("Operation");

                            output.WriteElementString("ID", tr.id.ToString());
                            output.WriteElementString("senderId", tr.Sender.id.ToString());
                            output.WriteElementString("recipientId", tr.Recipient.id.ToString());
                            output.WriteElementString("Value", tr.value.ToString());
                            output.WriteElementString("Time", tr.time.ToString());

                            output.WriteEndElement();
                        }

                        output.WriteEndElement();
                    }

                    cl.SaveAnaliserDataXml(output);
                }
                output.WriteEndElement();
            }
            output.WriteEndElement();

            output.Flush();
            output.Close();
        }
    }
}
