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
        private static DataDictionary ReadDD(string xml)
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

                    DDParser.SetVersionInfo(doc, dd);
                }
            }
            return dd;
        }

        [TestMethod]
        public void SetVersionInfo()
        {
            var dd = ReadDD("<fix major='4' minor='4'></fix>");
            Assert.AreEqual("4.4 [] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.4.4", dd.Identifier);

            dd = ReadDD("<fix major='5' type='FIX' servicepack='0' minor='0'></fix>");
            Assert.AreEqual("5.0 [0] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.5.0SP0", dd.Identifier);

            dd = ReadDD("<fix major='5' type='FIX' servicepack='2' minor='0'></fix>");
            Assert.AreEqual("5.0 [2] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIX.5.0SP2", dd.Identifier);

            dd = ReadDD("<fix type='FIXT' major='1' minor='1' servicepack='0'></fix>");
            Assert.AreEqual("1.1 [0] True", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.AreEqual("FIXT.1.1SP0", dd.Identifier);
        }
    }
}
