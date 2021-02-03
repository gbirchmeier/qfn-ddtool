using System.Reflection.Metadata;
using System;
using System.Xml;
using DDTool.Exceptions;
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
                dd.AddMessage(CreateMessage(msgNode, dd));
            }
        }

        private static DDMessage CreateMessage(XmlNode node, DataDictionary dd)
        {
            var ddMsg = new DDMessage(
                node.Attributes["name"].Value,
                node.Attributes["msgtype"].Value,
                node.Attributes["msgcat"].Value);


            foreach (XmlNode childNode in node.ChildNodes)
            {
                var nameAtt = childNode.Attributes["name"].Value;
                
                switch (childNode.Name.ToLowerInvariant())
                {
                    case "field":
                        if(!dd.FieldsByName.ContainsKey(nameAtt))
                            throw new ParsingException($"Field '{nameAtt}' is not defined in <fields> section.");
                        DDField fld = dd.FieldsByName[nameAtt];

                        bool required = childNode.Attributes["required"]?.Value == "Y";

                        ddMsg.AddField(fld, required);
                        break;

                    case "group":
                        throw new ParsingException("groups not supported yet");

                    case "component":
                        throw new ParsingException("components not supported yet");

                    default:
                        throw new ParsingException($"Unrecognized or unsupported child node type \"{childNode.Name}\" within message \"{ddMsg.Name}\"");
                }
            }

            return ddMsg;
        }
    }
}
