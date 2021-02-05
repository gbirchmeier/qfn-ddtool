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
        public void ParseMessageWithFlatGroups()
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
            xml.AppendLine("    <group name='NoDoots' required='N'>");
            xml.AppendLine("       <field name='Doot' required='Y' />");
            xml.AppendLine("    </group>");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <fields>");
            xml.AppendLine("    <field number='33' name='LinesOfText' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='58' name='Text' type='STRING' />");
            xml.AppendLine("    <field number='148' name='Headline' type='STRING' />");
            xml.AppendLine("    <field number='354' name='EncodedTextLen' type='LENGTH' />");
            xml.AppendLine("    <field number='355' name='EncodedText' type='DATA' />");
            xml.AppendLine("    <field number='500' name='NoDoots' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='501' name='Doot' type='STRING' />");
            xml.AppendLine("    <field number='999' name='Wahwah' type='INT' />");
            xml.AppendLine("  </fields>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            Assert.AreEqual(1, dd.Messages.Count);

            var msg = dd.Messages["B"];
            Assert.AreEqual(3, msg.Elements.Count);

            CollectionAssert.AreEqual(new int[] { 148, 33, 500 }, msg.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 33, 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var headline = msg.Elements[148];
            Assert.IsInstanceOfType(headline, typeof(DDTool.Structures.DDField));

            var linesOfText = (DDTool.Structures.DDGroup)msg.Elements[33];
            Assert.AreEqual(3, linesOfText.Elements.Count);
            Assert.AreEqual(33, linesOfText.CounterField.Tag);
            Assert.AreEqual(58, linesOfText.DelimiterElement.Tag);
            CollectionAssert.AreEqual(new int[] { 999, 354, 355 }, linesOfText.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 999 }, linesOfText.RequiredElements.ToArray());

            var noDoots = (DDTool.Structures.DDGroup)msg.Elements[500];
            Assert.AreEqual(0, noDoots.Elements.Count);
            Assert.AreEqual(500, noDoots.CounterField.Tag);
            Assert.AreEqual(501, noDoots.DelimiterElement.Tag);
            Assert.AreEqual(0, noDoots.ElementOrder.Count);
            Assert.AreEqual(0, noDoots.RequiredElements.Count);
        }

        [TestMethod]
        public void ParseMessageWithNestedGroup()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("    <field name='Headline' required='Y' />");
            xml.AppendLine("    <group name='LinesOfText' required='Y'>");
            xml.AppendLine("      <field name='Text' required='Y' />");
            xml.AppendLine("      <group name='FooNest' required='N'>");
            xml.AppendLine("        <field name='FooDelim' required='Y' />");
            xml.AppendLine("        <field name='FooField' required='Y' />");
            xml.AppendLine("      </group>");
            xml.AppendLine("      <group name='BarNest' required='Y'>");
            xml.AppendLine("        <field name='BarDelim' required='Y' />");
            xml.AppendLine("        <field name='BarField' required='N' />");
            xml.AppendLine("      </group>");
            xml.AppendLine("    </group>");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <fields>");
            xml.AppendLine("    <field number='33' name='LinesOfText' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='58' name='Text' type='STRING' />");
            xml.AppendLine("    <field number='148' name='Headline' type='STRING' />");
            xml.AppendLine("    <field number='200' name='FooNest' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='201' name='FooDelim' type='STRING' />");
            xml.AppendLine("    <field number='202' name='FooField' type='STRING' />");
            xml.AppendLine("    <field number='300' name='BarNest' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='301' name='BarDelim' type='STRING' />");
            xml.AppendLine("    <field number='302' name='BarField' type='STRING' />");
            xml.AppendLine("  </fields>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            Assert.AreEqual(1, dd.Messages.Count);

            var linesOfText = (DDTool.Structures.DDGroup)dd.Messages["B"].Elements[33];
            Assert.AreEqual(2, linesOfText.Elements.Count);

            CollectionAssert.AreEqual(new int[] { 200, 300 }, linesOfText.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 300 }, linesOfText.RequiredElements.ToArray());

            var fooNest = (DDTool.Structures.DDGroup)linesOfText.Elements[200];
            Assert.AreEqual(200, fooNest.CounterField.Tag);
            Assert.AreEqual(201, fooNest.DelimiterElement.Tag);
            CollectionAssert.AreEqual(new int[] { 202 }, fooNest.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 202 }, fooNest.RequiredElements.ToArray());

            var barNest = (DDTool.Structures.DDGroup)linesOfText.Elements[300];
            Assert.AreEqual(300, barNest.CounterField.Tag);
            Assert.AreEqual(301, barNest.DelimiterElement.Tag);
            CollectionAssert.AreEqual(new int[] { 302 }, barNest.ElementOrder.ToArray());
            Assert.AreEqual(0, barNest.RequiredElements.Count);
        }

        [TestMethod]
        public void ParseMessageWithTopLevelComponentsThatAreNotGroups()
        {
            // The components are not groups, but MAY CONTAIN groups.
            // (Thus 'required' field can be absent and is effectively ignored)
            var xml = new StringBuilder();
            xml.AppendLine("<fix><messages>");
            xml.AppendLine("  <message name='News' msgtype='B' msgcat='app'>");
            xml.AppendLine("    <field name='Headline' required='Y' />");
            xml.AppendLine("    <component name='XComponent' />");
            xml.AppendLine("    <component name='YComponent' />");
            xml.AppendLine("  </message>");
            xml.AppendLine("  <components>");
            xml.AppendLine("    <component name='XComponent'>");
            xml.AppendLine("      <field name='Field76' required='Y' />");
            xml.AppendLine("      <group name='FooGroup' required='N'>");
            xml.AppendLine("        <field name='FooDelim' required='Y' />");
            xml.AppendLine("        <field name='FooField' required='Y' />");
            xml.AppendLine("      </group>");
            xml.AppendLine("    </component>");
            xml.AppendLine("    <component name='YComponent'>");
            xml.AppendLine("      <field name='Field77' required='N' />");
            xml.AppendLine("      <field name='Field78' required='Y' />");
            xml.AppendLine("    </component>");
            xml.AppendLine("  </components>");
            xml.AppendLine("  <fields>");
            xml.AppendLine("    <field number='58' name='Text' type='STRING' />");
            xml.AppendLine("    <field number='76' name='Field76' type='STRING' />");
            xml.AppendLine("    <field number='77' name='Field77' type='STRING' />");
            xml.AppendLine("    <field number='78' name='Field78' type='STRING' />");
            xml.AppendLine("    <field number='148' name='Headline' type='STRING' />");
            xml.AppendLine("    <field number='200' name='FooGroup' type='NUMINGROUP' />");
            xml.AppendLine("    <field number='201' name='FooDelim' type='STRING' />");
            xml.AppendLine("    <field number='202' name='FooField' type='STRING' />");
            xml.AppendLine("  </fields>");
            xml.AppendLine("</messages></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), ParserTask.Messages);
            var msg = dd.Messages["B"];
            Assert.AreEqual(5, msg.Elements.Count);
            CollectionAssert.AreEqual(new int[] { 148, 76, 200, 77, 78 }, msg.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 76, 78, 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var fooGroup = (DDTool.Structures.DDGroup)msg.Elements[200];
            Assert.AreEqual(200, fooGroup.CounterField.Tag);
            Assert.AreEqual(201, fooGroup.DelimiterElement.Tag);
            CollectionAssert.AreEqual(new int[] { 202 }, fooGroup.ElementOrder.ToArray());
            CollectionAssert.AreEqual(new int[] { 202 }, fooGroup.RequiredElements.ToArray());
        }
    }
}
