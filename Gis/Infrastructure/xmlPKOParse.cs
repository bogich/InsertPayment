using System;
using System.Xml;
using System.Collections.Generic;

namespace Gis.Infrastructure
{
    public class XmlPko
    {
        public string PkoNumber { get; set; }
        public DateTime PkoDate { get; set; }
        public string PkoUID { get; set; }
        public string PkoINN { get; set; }
        public string PkoContragent { get; set; }
        public string PkoAgreement { get; set; }
        public string PkoBaseAgreement { get; set; }
        public string PkoComment { get; set; }
        public decimal PkoSum { get; set; }
    }

    public class XmlPkoParse
    {
        private List<XmlPko> PKOCollection;


        /// <summary>
        /// Генерирует случайную последовательность заданного количества символов
        /// </summary>
        /// <param name="_CountNumber">Длина кода</param>
        /// <returns></returns>
        private string GetRandomNumber(int _CountNumber)
        {
            string orderID = null;
            Random rnd = new Random();
            Char[] pwdChars = new Char[36] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < _CountNumber; i++)
            {
                orderID += pwdChars[rnd.Next(0, 35)];
            }

            return orderID;
        }

        /// <summary>
        /// Возвращает платежные кассовые ордера из заданного XML
        /// </summary>
        /// <param name="_FilePath"></param>
        /// <returns></returns>
        public List<XmlPko> GetItemsPKO(string _FilePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_FilePath);

                PKOCollection = new List<XmlPko>();
                foreach(XmlNode node in doc.DocumentElement)
                {
                    XmlPko _pko = new XmlPko
                    {
                        PkoNumber = GetRandomNumber(18),
                        //_pko.PkoNumber = node.ChildNodes[0].InnerText;
                        PkoDate = DateTime.Parse(node.ChildNodes[1].InnerText),
                        PkoUID = node.ChildNodes[2].InnerText,
                        PkoINN = node.ChildNodes[3].InnerText,
                        PkoContragent = node.ChildNodes[4].InnerText,
                        PkoAgreement = node.ChildNodes[5].InnerText,
                        PkoBaseAgreement = node.ChildNodes[6].InnerText,
                        PkoComment = node.ChildNodes[7].InnerText,
                        PkoSum = decimal.Parse(node.ChildNodes[8].InnerText)
                    };
                    PKOCollection.Add(_pko);
                }

                return PKOCollection;
            }
            catch
            {
                return null;
            }
        }
    }
}
