using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientBank
{
    class Program
    {
        static void Main(string[] args)
        {
            Storage storage = null;

            while (true)
            {
                int a = 0;
                bool bdCreated = false;

                Console.WriteLine("1 - Create base\n2 - Open base");

                if (int.TryParse(Console.ReadLine(), out a))
                {

                    if (a == 1)
                    {
                        storage = new Storage();
                        bdCreated = true;
                    }
                    if (a == 2)
                    {
                        Console.WriteLine("Enter path to XML file");
                        try
                        {
                            storage = new Storage(Console.ReadLine());
                            bdCreated = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

                while (bdCreated)
                {
                    Console.WriteLine("---------------------------------------------------");
                    Console.WriteLine("1 - Add client\n2 - Print clients\n3 - Print client by ID\n4 - Make a transaction\n5 - Revoke transaction\n6 - Withdraw/deposit money\n9 - Save base");
                    Console.WriteLine("0 - Exit");

                    if (int.TryParse(Console.ReadLine(), out a))
                    {
                        if (a == 1)
                        {
                            string str;
                            int id;
                            float money;

                            Console.WriteLine("ID_Name_Money");

                            if ((str = Console.ReadLine()) != "")
                            {
                                string[] input = str.Split('_');

                                if (input.Length == 3)
                                    if (int.TryParse(input[0], out id) & float.TryParse(input[2], out money))
                                    {
                                        storage.AddCient(id, input[1], money);
                                    }
                            }
                        }

                        if(a == 2)
                        {
                            storage.PrintClientList();
                        }

                        if (a == 3)
                        {
                            int id;
                            Console.WriteLine("Enter client ID");

                            if(int.TryParse(Console.ReadLine(), out id))
                                storage.PrintClienByID(id);
                            else
                                Console.WriteLine("Incorrect ID");
                        }

                        if(a == 4)
                        {
                            Console.WriteLine("SenderID_RecipientID_Value");

                            string str;
                            int senderId, recipientId;
                            float value;

                            if ((str = Console.ReadLine()) != "")
                            {
                                string[] input = str.Split('_');

                                if (input.Length == 3)
                                    if (int.TryParse(input[0], out senderId) & int.TryParse(input[1], out recipientId) & float.TryParse(input[2], out value))
                                    {
                                        storage.Transaction(senderId, recipientId, value);
                                    }
                            }
                        }

                        if (a == 5)
                        {
                            Console.WriteLine("ClientID_TransactionID");

                            string str;
                            int clientID;
                            long transID;

                            if ((str = Console.ReadLine()) != "")
                            {
                                string[] input = str.Split('_');

                                if (input.Length == 2)
                                    if (int.TryParse(input[0], out clientID) & long.TryParse(input[1], out transID))
                                    {
                                        storage.RevokeTransaction(clientID, transID);
                                    }
                            }
                        }

                        if(a == 6)
                        {
                            Console.WriteLine("ClientID_Value");

                            string str;
                            int clientID;
                            float value;

                            if ((str = Console.ReadLine()) != "")
                            {
                                string[] input = str.Split('_');

                                if (input.Length == 2)
                                    if (int.TryParse(input[0], out clientID) & float.TryParse(input[1], out value))
                                    {
                                        storage.ChangeClientsMoney(clientID, value);
                                    }
                            }
                        }

                        if (a == 9)
                        { 
                            Console.WriteLine("Path to XML file");
                            try
                            {
                                storage.SaveXML(Console.ReadLine());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }

                        if (a == 0)
                            return;
                    }
                }
            }
        }
    }
}
