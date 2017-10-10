using System;
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

        public void Transaction(int senderID, int recipientID, float value )    //осуществляет перевод
        {
            storage.Transaction(senderID, recipientID, value);
        }

        public void Credit(Client cl, float value)
        {
            storage.Credit(cl.id, value);
        }

        public void RevokeTransaction(int clientID, long transID)   //отменяет перевод
        {
            storage.RevokeTransaction(clientID, transID);
        }

        public string[,] GetClientsTransactions(int _id)    //возвращает 2 мерный массив 0 - транзакции, 00 - тип, 01 - имя, 02 - сумма, 03 - id транзакции
        {
            if (Storage.FindClientByID(_id).GetTransactionList() != null)
            {
                string[,] transactions = new string[Storage.FindClientByID(_id).GetTransactionList().Count, 4];
                int i = 0;

                foreach (Transaction tr in Storage.FindClientByID(_id).GetTransactionList())
                {
                    if (tr.sender.id == _id)
                    {
                        transactions[i, 0] = "Отправил";
                        transactions[i, 1] = Storage.FindClientByID(tr.recipient.id).name;
                    }
                    else
                    {
                        transactions[i, 0] = "Получил от";
                        transactions[i, 1] = Storage.FindClientByID(tr.sender.id).name;
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

        public static string AllowedCreditSumm(Client cl, float precent, int time)    //доступная сумма кредита
        {
            if (cl.prevConvictions)
                return "Клиент имеет судимость, отказ в кредите";

            float summ = (cl.monthlyIncome - 10000) * time + (((cl.monthlyIncome - 10000) * time) * precent);
            if (summ <= 0)
                return "У клиента недостаточный доход, отказ в кредите";

            return summ.ToString();
        }
    }
}
