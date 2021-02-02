using System;
using System.Xml;
using DDTool.Structures;

namespace DDTool.Parsers
{
    public class MessageParser
    {
        public static void ParseMessages(XmlDocument doc, DataDictionary dd)
        {
            XmlNodeList nodeList = doc.SelectNodes("//messages/message");
            foreach (XmlNode msgNode in nodeList)
            {
                dd.AddMessage(CreateMessage(msgNode));
            }
        }

        private static DDMessage CreateMessage(XmlNode node)
        {
            var ddMsg = new DDMessage(
                node.Attributes["name"].Value,
                node.Attributes["msgtype"].Value,
                node.Attributes["msgcat"].Value);

            return ddMsg;
        }
    }
}
