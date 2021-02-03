using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Parsers
{
    [TestClass]
    public class MessageParserTests
    {
        [TestMethod]
        public void ParseEmptyMessages()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='Heartbeat' msgtype='0' msgcat='admin'>");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("  </message>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            Assert.AreEqual(2, dd.Messages.Count);

            var msg = dd.Messages["0"];
            Assert.AreEqual("0:Heartbeat:admin", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
            msg = dd.Messages["B"];
            Assert.AreEqual("B:News:app", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
        }

        [TestMethod]
        public void ParseMessagesWithJustFields()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("    <field name='OrigTime' required='N' />");
            xml.AppendLine("    <field name='Urgency' required='N' />");
            xml.AppendLine("    <field name='Headline' required='Y' />");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <fields>");
            xml.AppendLine("    <field number='42' name='OrigTime' type='UTCTIMESTAMP' />");
            xml.AppendLine("    <field number='61' name='Urgency' type='CHAR' />");
            xml.AppendLine("    <field number='148' name='Headline' type='STRING' />");
            xml.AppendLine("  </fields>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            Assert.AreEqual(1, dd.Messages.Count);

            var msg = dd.Messages["B"];
            Assert.AreEqual(3, msg.Elements.Count);

            CollectionAssert.AreEqual(new int[] { 42, 61, 148 }, msg.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new[] { 148 }, msg.RequiredElements.ToArray());
        }
    }
}
