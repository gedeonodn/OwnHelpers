using System;
using System.Xml;

namespace sms
{
    public class HuaweiParser
    {
        private XmlDocument xmlDoc;
        public HuaweiParser(XmlDocument xmlDoc)
        {
            this.xmlDoc = xmlDoc;
            
        }

        public Smses Parse()
        {
            Smses smses = new Smses();

            XmlNodeList smsNodes = xmlDoc.SelectNodes("response/Messages/Message");
            foreach(XmlNode smsNode in smsNodes)
            {
                int id = Convert.ToInt32(smsNode.SelectSingleNode("Index").InnerText);
                string phone = smsNode.SelectSingleNode("Phone").InnerText;
                string message = smsNode.SelectSingleNode("Content").InnerText;
                DateTime dateTime = Convert.ToDateTime(smsNode.SelectSingleNode("Date").InnerText);
                //
                int smstat = Convert.ToInt32(smsNode.SelectSingleNode("Smstat").InnerText);
                string sca = smsNode.SelectSingleNode("Sca").InnerText;
                int saveType = Convert.ToInt32(smsNode.SelectSingleNode("SaveType").InnerText);
                int priorityd = Convert.ToInt32(smsNode.SelectSingleNode("Priority").InnerText);
                int smsType = Convert.ToInt32(smsNode.SelectSingleNode("SmsType").InnerText);

                smses.Add(new Sms(id, phone, message, dateTime, smstat, sca, saveType, priorityd, smsType));
            }

            return smses;
        }
    }
}