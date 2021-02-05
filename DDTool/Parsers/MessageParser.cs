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
                switch (childNode.Name.ToLowerInvariant())
                {
                    case "field":
                        ddMsg.AddField(
                            dd.LookupField(childNode.Attributes["name"].Value),
                            childNode.Attributes["required"]?.Value == "Y"); // TODO reject non-Y/N
                        break;

                    case "group":
                        ddMsg.AddGroup(
                            CreateGroup(childNode, dd),
                            childNode.Attributes["required"]?.Value == "Y");
                        break;

                    case "component":
                        throw new ParsingException("components not supported yet");

                    default:
                        throw new ParsingException($"Unrecognized or unsupported child node type \"{childNode.Name}\" within message \"{ddMsg.Name}\"");
                }
            }

            return ddMsg;
        }

        private static DDGroup CreateGroup(XmlNode node, DataDictionary dd)
        {
            var groupName = node.Attributes["name"].Value;
            var counterField = dd.LookupField(groupName);

            if (node.ChildNodes.Count < 1)
                throw new ParsingException($"Group {groupName} is illegal.  It must have at least one child.");

            // TODO pretty sure this can be a group too
            var delimiterField = dd.LookupField(
                node.ChildNodes[0].Attributes["name"].Value);

            DDGroup ddGroup = new DDGroup(counterField, delimiterField);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode == node.ChildNodes[0])
                    continue; // already did the first (delimiter) node

                switch (childNode.Name.ToLowerInvariant())
                {
                    case "field":
                        ddGroup.AddField(
                            dd.LookupField(childNode.Attributes["name"].Value),
                            childNode.Attributes["required"]?.Value == "Y");
                        break;

                    case "group":
                        ddGroup.AddGroup(
                            CreateGroup(childNode, dd),
                            childNode.Attributes["required"]?.Value == "Y");
                        break;

                    case "component":
                        throw new ParsingException("nested components not supported yet");

                    default:
                        throw new ParsingException($"Unrecognized or unsupported child node type \"{childNode.Name}\" within group \"{ddGroup.CounterField.Name}\"");
                }
            }

            return ddGroup;
        }
    }
}
