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
                dd.AddMessage(CreateMessage(msgNode, dd, doc));
            }
        }

        private static DDMessage CreateMessage(XmlNode node, DataDictionary dd, XmlDocument doc)
        {
            var ddMsg = new DDMessage(
                node.Attributes["name"].Value,
                node.Attributes["msgtype"].Value,
                node.Attributes["msgcat"].Value);

            foreach (XmlNode childNode in node.ChildNodes)
                ReadChildNode(childNode, ddMsg, dd, doc);

            return ddMsg;
        }

        private static DDGroup CreateGroup(XmlNode node, DataDictionary dd, XmlDocument doc)
        {
            var groupName = node.Attributes["name"].Value;
            var counterField = dd.LookupField(groupName);

            if (node.ChildNodes.Count < 1)
                throw new ParsingException($"Group {groupName} is illegal.  It must have at least one child (the delimiter).");

            // TODO pretty sure this can be a group too, so need to handle this later
            var delimiterField = dd.LookupField(
                node.ChildNodes[0].Attributes["name"].Value);

            DDGroup ddGroup = new DDGroup(counterField, delimiterField);

            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (childNode == node.ChildNodes[0])
                    continue; // already did the first (delimiter) node

                ReadChildNode(childNode, ddGroup, dd, doc);
            }

            return ddGroup;
        }

        private static void ReadChildNode(
            XmlNode childNode, IElementSequence elSeq, DataDictionary dd, XmlDocument doc)
        {
            switch (childNode.Name.ToLowerInvariant())
            {
                case "field":
                    elSeq.AddField(
                        dd.LookupField(childNode.Attributes["name"].Value),
                        childNode.Attributes["required"]?.Value == "Y");
                    break;

                case "group":
                    elSeq.AddGroup(
                        CreateGroup(childNode, dd, doc),
                        childNode.Attributes["required"]?.Value == "Y");
                    break;

                case "component":
                    var componentName = childNode.Attributes["name"].Value;
                    XmlNode compNode = doc.SelectSingleNode($"//components/component[@name='{componentName}']");

                    if (compNode == null)
                        throw new ParsingException($"Can't find component: {componentName}");

                    foreach (XmlNode innerCompNode in compNode.ChildNodes)
                        ReadChildNode(innerCompNode, elSeq, dd, doc);

                    break;

                default:
                    throw new ParsingException($"Unrecognized or unsupported child node type \"{childNode.Name}\" within \"{elSeq.Name}\"");
            }
        }
    }
}
