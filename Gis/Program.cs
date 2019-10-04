using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Gis.Infrastructure.PaymentServiceAsync;
using Gis.Helpers.HelperPaymentAsync;
using Gis.Infrastructure;

namespace Gis
{
    class Program
	{
        //private const string _orgPPAGUID = "73075d22-be00-47c3-ad82-e95be27ac276"; // SIT01
        private const string _orgPPAGUID = "04b83d24-6daa-47f9-bb4f-ce9330c3094d"; //Prod

        static void Main(string[] args)
		{
            //Вариант с выгруженными в XML ПКО
            XmlPkoParse ItemPKO = new XmlPkoParse();
            var listPKO = ItemPKO.GetItemsPKO(ConfigurationManager.AppSettings["pkoPath"]);

            if (listPKO.Count != 0)
            {
                List<dynamic> listNotificationOfOrder = new List<dynamic>();
                foreach (var itemListPKO in listPKO)
                {
                    listNotificationOfOrder.Add(new object[] { itemListPKO.PkoNumber, itemListPKO.PkoDate, itemListPKO.PkoSum });
                }

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                HelperPaymentAsync helperPaymentAsync = new HelperPaymentAsync();

                string ack = helperPaymentAsync.SetSupplierNotificationsOfOrderExecution(_orgPPAGUID, listNotificationOfOrder).ToString();

                int requestState;
                int i = 0;
                do
                {
                    var resStateResult = helperPaymentAsync.P_GetState(_orgPPAGUID, ack).getStateResult;
                    requestState = resStateResult.RequestState;
                    switch (requestState)
                    {
                        case 1:
                            Console.Write("1 - получено");
                            Thread.Sleep(1000);
                            Console.WriteLine("\r\n{0} sec left", i++);
                            Console.SetCursorPosition(0, Console.CursorTop - 2);
                            break;
                        case 2:
                            Console.Write("2 - в обработке");
                            Thread.Sleep(1000);
                            Console.WriteLine("\r\n{0} sec left", i++);
                            Console.SetCursorPosition(0, Console.CursorTop - 2);
                            break;
                        case 3:
                            Console.SetCursorPosition(0, Console.CursorTop + 2);
                            foreach (var itemGetState in resStateResult.Items)
                            {
                                if (itemGetState.GetType() == typeof(ErrorMessageType))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("{0} - {1}", ((ErrorMessageType)itemGetState).ErrorCode, ((ErrorMessageType)itemGetState).Description);
                                    Console.ResetColor();
                                }
                                else
                                {
                                    var itemState = itemGetState as CommonResultType;
                                    Console.WriteLine("Create item: {0} - {1}", itemState.Items[0], itemState.Items[1]);

                                    File.AppendAllText(@"LogResult.csv", DateTime.Now + ";" + itemState.Items[0] + ";" + itemState.Items[1] + ";" + Environment.NewLine, Encoding.Default);
                                }
                            }
                            break;
                    }
                }
                while (requestState < 3);  
            }
            else
            {
                Console.WriteLine("List PKO is empty");
            }

            //Console.Read();
        }
    }
}