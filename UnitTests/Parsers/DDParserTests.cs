using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DDTool.Parsers;
using DDTool.Structures;

namespace UnitTests.Parsers
{
    [TestClass]
    public class DDParserTests
    {
        delegate void ParserFunc(XmlDocument doc, DataDictionary dd);

        private static DataDictionary ReadDD(string xml, ParserFunc parserFunc)
        {
            var dd = new DataDictionary();

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreComments = true;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                using (var reader = XmlReader.Create(stream, readerSettings))
                {
                    var doc = new XmlDocument();
                    doc.Load(reader);

                    parserFunc(doc, dd);
                }
            }
            return dd;
        }

        [TestMethod]
        public void SetVersionInfo()
        {
            var dd = ReadDD("<fix major='4' minor='4'/>", DDParser.SetVersionInfo);
            Assert.AreEqual("4.4 [] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.4.4", dd.Identifier);

            dd = ReadDD("<fix major='5' type='FIX' servicepack='0' minor='0'/>", DDParser.SetVersionInfo);
            Assert.AreEqual("5.0 [0] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.5.0SP0", dd.Identifier);

            dd = ReadDD("<fix major='5' type='FIX' servicepack='2' minor='0'/>", DDParser.SetVersionInfo);
            Assert.AreEqual("5.0 [2] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.5.0SP2", dd.Identifier);

            dd = ReadDD("<fix type='FIXT' major='1' minor='1' servicepack='0'/>", DDParser.SetVersionInfo);
            Assert.AreEqual("1.1 [0] True", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIXT.1.1SP0", dd.Identifier);
        }

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

            var dd = ReadDD(xml.ToString(), DDParser.ParseFields);

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

            var dd = ReadDD(xml.ToString(), DDParser.ParseMessages);
            Assert.AreEqual(2, dd.Messages.Count);

            var msg = dd.Messages["0"];
            Assert.AreEqual("0:Heartbeat:admin", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
            msg = dd.Messages["B"];
            Assert.AreEqual("B:News:app", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
        }
    }
}
