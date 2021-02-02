using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDTool.Parsers;

namespace UnitTests.Parsers
{
    [TestClass]
    public class FieldParserTests
    {
        [TestMethod]
        public void ParseFields()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<fix><fields>");
            xml.AppendLine("  <field number='1' name='Account' type='sTRING'/>");
            xml.AppendLine("  <field number='4' name='AdvSide' type='CHaR'>");
            xml.AppendLine("    <value enum='B' description='BUY'/>");
            xml.AppendLine("    <value enum='S' description='Sell'/>");
            xml.AppendLine("  </field>");
            xml.AppendLine("  <field number='14' name='CumQty' type='QTY' />");
            xml.AppendLine("</fields></fix>");

            var dd = ParserTestUtil.ReadDD(xml.ToString(), FieldParser.ParseFields);

            Assert.AreEqual(3, dd.FieldsByTag.Count);

            var fld = dd.FieldsByTag[1];
            Assert.AreEqual("1:Account:STRING", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");
            fld = dd.FieldsByTag[14];
            Assert.AreEqual("14:CumQty:QTY", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");

            fld = dd.FieldsByTag[4];
            Assert.AreEqual("4:AdvSide:CHAR", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");
            Assert.AreEqual(2, fld.EnumDict.Count);
            Assert.AreEqual("BUY", fld.EnumDict["B"]);
            Assert.AreEqual("Sell", fld.EnumDict["S"]);
        }
    }
}
