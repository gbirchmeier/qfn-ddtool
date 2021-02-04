using System.Xml.Linq;
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
        public void ParseMessageWithJustFields()
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
            CollectionAssert.AreEqual(new int[] { 148 }, msg.RequiredElements.ToArray());
        }

        [TestMethod]
        public void ParseMessageWithOneGroup()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("    <field name='Headline' required='Y' />");
            xml.AppendLine("    <group name='LinesOfText' required='Y'>");
            xml.AppendLine("       <field name='Text' required='Y' />");
            xml.AppendLine("       <field name='Wahwah' required='Y' />");
            xml.AppendLine("       <field name='EncodedTextLen' required='N' />");
            xml.AppendLine("       <field name='EncodedText' required='N' />");
            xml.AppendLine("    </group>");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <fields>");
            xml.AppendLine("    <field number='33' name='LinesOfText' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='58' name='Text' type='STRING' />");
            xml.AppendLine("    <field number='148' name='Headline' type='STRING' />");
            xml.AppendLine("    <field number='354' name='EncodedTextLen' type='LENGTH' />");
            xml.AppendLine("    <field number='355' name='EncodedText' type='DATA' />");
            xml.AppendLine("    <field number='999' name='Wahwah' type='INT' />");
            xml.AppendLine("  </fields>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            Assert.AreEqual(1, dd.Messages.Count);

            var msg = dd.Messages["B"];
            Assert.AreEqual(2, msg.Elements.Count);

            CollectionAssert.AreEqual(new int[] { 148, 33 }, msg.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 33, 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var headline = msg.Elements[148];
            Assert.IsInstanceOfType(headline, typeof(DDTool.Structures.DDField));

            var linesOfText = (DDTool.Structures.DDGroup)msg.Elements[33];
            Assert.AreEqual(3, linesOfText.Elements.Count);
            Assert.AreEqual(33, linesOfText.CounterField.Tag);
            Assert.AreEqual(58, linesOfText.DelimiterElement.Tag);
            // Delimiter is not included in the following
            CollectionAssert.AreEqual(new int[] { 999, 354, 355 }, linesOfText.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 999 }, linesOfText.RequiredElements.ToArray());
        }
    }
}
