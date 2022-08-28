using System;
using System.Text;
using System.Threading.Tasks;
using DDTool.Parsers;
using Xunit;

namespace UnitTests.Parsers
{
    public class FieldParserTests
    {
        [Theory]
        [InlineData("UnitTests.Resources.Fields.Example.txt")]
        public async Task ParseFields(string resourceName)
        {
            var fields = await resourceName.GetResourceStringAsync<FieldParserTests>();
            var dd = ParserTestUtil.ReadDD(fields, ParserTask.Fields);

            Assert.Equal(3, dd.FieldsByTag.Count);
            Assert.Equal(3, dd.FieldsByName.Count);

            Assert.Equal(dd.FieldsByTag[1], dd.FieldsByName["Account"]);
            Assert.Equal(dd.FieldsByTag[4], dd.FieldsByName["AdvSide"]);
            Assert.Equal(dd.FieldsByTag[14], dd.FieldsByName["CumQty"]);

            var fld = dd.FieldsByTag[1];
            Assert.Equal("1:Account:STRING", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");
            fld = dd.FieldsByTag[14];
            Assert.Equal("14:CumQty:QTY", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");

            fld = dd.FieldsByTag[4];
            Assert.Equal("4:AdvSide:CHAR", $"{fld.Tag}:{fld.Name}:{fld.FixFieldType}");
            Assert.Equal(2, fld.EnumDict.Count);
            Assert.Equal("BUY", fld.EnumDict["B"]);
            Assert.Equal("Sell", fld.EnumDict["S"]);
        }
    }
}
