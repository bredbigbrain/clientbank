﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class StorageView
    {
        private Storage storage;
        private string path;

        public List<Client> GetClients()    //возвращает лис клиентов
        {
            if (storage != null)
                return storage.GetClientList();
            return new List<Client>();
        }

        public void ClearClientsList()  //удаляет всех клиетнов
        {
            if(storage != null)
                storage.ClearClientsList();
        }

        public void AddClient(string _name, float _money, bool _prevC, float _income)  //добавляет клиента
        {
            storage.AddCient(_name, _money, _prevC, _income);
        }   

        public void Transaction(int senderId, int recipientId, float value )    //осуществляет перевод
        {
            storage.Transaction(senderId, recipientId, value);
        }

        public int NextMonth() //следующий месяц
        {
            return storage.NextMonth();
        }

        public int GetDate()
        {
            if (storage != null)
                return storage.GetDate();
            else
                throw new Exception("Storage null (GetDate)");
        }

        public void Credit(Client cl, float value, int time)  //кредит
        {
            storage.Credit(cl.id, value, time);
        }
        /*
        public void RevokeTransaction(int clientID, long transID)   //отменяет перевод
        {
            storage.RevokeTransaction(clientID, transID);
        }
        */

        public string[,] GetClientsTransactions(int _id)    //возвращает 2 мерный массив 0 - транзакции, 00 - тип, 01 - имя, 02 - сумма, 03 - id транзакции
        {
            if (Storage.FindClientByID(_id).GetTransactionList() != null)
            {
                string[,] transactions = new string[Storage.FindClientByID(_id).GetTransactionList().Count, 4];
                int i = 0;

                foreach (Operation tr in Storage.FindClientByID(_id).GetTransactionList())
                {
                    if (tr.Sender.id == _id)
                    {
                        transactions[i, 0] = "Отправил";
                        transactions[i, 1] = Storage.FindClientByID(tr.Recipient.id).name;
                    }
                    else
                    {
                        transactions[i, 0] = "Получил от";
                        transactions[i, 1] = Storage.FindClientByID(tr.Sender.id).name;
                    }

                    transactions[i, 2] = tr.value.ToString();
                    transactions[i, 3] = tr.id.ToString();

                    i++;
                }
                return transactions;
            }
            return null;
        }

        public void OpenBaseFile(string _path)  //создает базу из xml файла
        {
            storage = new Storage(_path);
            path = _path;
        }

        public void CreateNewBase(string _path)     //создает пустую базу
        {
            storage = new Storage();
            path = _path;
        }

        public void SaveBaseXML()   //сохраняет в xml базу
        {
            storage.SaveXML(path);
        }

        public void SaveBaseAsXML(string _path)     //сохраняет базу в новый xml файл
        {
            storage.SaveXML(_path);
            path = _path;
        }

        public string AllowedCreditSumm(Client cl, float precent, int time)    //доступная сумма кредита
        {
            if(time <= 0)
                throw new Exception("Недопустимый срок");

            if (cl.prevConvictions)
                return "Клиент имеет судимость, отказ в кредите";

            float summ = (cl.monthlyIncome - 10000) * time + (((cl.monthlyIncome - 10000) * time) * precent);
            if (summ <= 0)
                return "У клиента недостаточный доход, отказ в кредите";

            int delaySumm = 0;
            foreach(Client c in storage.GetClientList())
            {
                delaySumm += c.delayMonths;
            }

            float averageDelay = delaySumm / storage.GetClientList().Count;

            if (cl.delayMonths > averageDelay)
                return "У клиента плохая кредитная история";

            return summ.ToString();
        }
    }
}
