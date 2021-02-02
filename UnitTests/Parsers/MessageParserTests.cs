using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDTool.Parsers;

namespace UnitTests.Parsers
{
    [TestClass]
    public class MessageParserTests
    {
        [TestMethod]
        public void ParseMessages()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='Heartbeat' msgtype='0' msgcat='admin'>");
            xml.AppendLine("    <field name='TestReqID' required='N' />");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("    <field name='OrigTime' required='N' />");
            xml.AppendLine("    <field name='Urgency' required='N' />");
            xml.AppendLine("    <field name='Headline' required='Y' />");
            xml.AppendLine("  </message>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), MessageParser.ParseMessages);
            Assert.AreEqual(2, dd.Messages.Count);

            var msg = dd.Messages["0"];
            Assert.AreEqual("0:Heartbeat:admin", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
            msg = dd.Messages["B"];
            Assert.AreEqual("B:News:app", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
        }
    }
}
