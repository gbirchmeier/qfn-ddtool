using System;
using DDTool.Parsers;
using Xunit;

namespace UnitTests.Parsers
{
    public class VersionParserTests
    {
        [Fact]
        public void SetVersionInfo()
        {
            var dd = ParserTestUtil.ReadDD("<fix major='4' minor='4'/>", ParserTask.Version);
            Assert.Equal("4.4 [] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.Equal("FIX.4.4", dd.Identifier);

            dd = ParserTestUtil.ReadDD("<fix major='5' type='FIX' servicepack='0' minor='0'/>", ParserTask.Version);
            Assert.Equal("5.0 [0] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.Equal("FIX.5.0SP0", dd.Identifier);

            dd = ParserTestUtil.ReadDD("<fix major='5' type='FIX' servicepack='2' minor='0'/>", ParserTask.Version);
            Assert.Equal("5.0 [2] False", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.Equal("FIX.5.0SP2", dd.Identifier);

            dd = ParserTestUtil.ReadDD("<fix type='FIXT' major='1' minor='1' servicepack='0'/>", ParserTask.Version);
            Assert.Equal("1.1 [0] True", $"{dd.MajorVersion}.{dd.MinorVersion} [{dd.ServicePack}] {dd.IsFIXT}");
            Assert.Equal("FIXT.1.1SP0", dd.Identifier);
        }
    }
}
