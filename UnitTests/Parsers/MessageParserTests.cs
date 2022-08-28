using DDTool.Structures;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Parsers
{
    public class MessageParserTests
    {
        [Theory]
        [InlineData("UnitTests.Resources.Messages.Empty.txt")]
        public async Task Empty(string resourceName)
        {
            var emptyMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(emptyMessage, ParserTask.Messages);
            Assert.Equal(2, dd.Messages.Count);

            var msg = dd.Messages["0"];
            Assert.Equal("0:Heartbeat:admin", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
            msg = dd.Messages["B"];
            Assert.Equal("B:News:app", $"{msg.MsgType}:{msg.Name}:{msg.Cat}");
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.JustFields.txt")]
        public async Task JustFields(string resourceName)
        {
            var fieldMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(fieldMessage, ParserTask.Messages);
            Assert.Single(dd.Messages);

            var msg = dd.Messages["B"];
            Assert.Equal(3, msg.Elements.Count);

            Assert.Equal(new int[] { 42, 61, 148 }, msg.ElementOrder.ToArray());
            Assert.Equal(new int[] { 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.FlatGroups.txt")]
        public async Task FlatGroups(string resourceName)
        {
            var flatGroupMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(flatGroupMessage, ParserTask.Messages);
            Assert.Single(dd.Messages);

            var msg = dd.Messages["B"];
            Assert.Equal(3, msg.Elements.Count);
            Assert.Equal(new int[] { 148, 33, 500 }, msg.ElementOrder.ToArray());
            Assert.Equal(new int[] { 33, 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var headline = msg.Elements[148];
            Assert.IsType<DDField>(headline);

            var linesOfText = (DDGroup)msg.Elements[33];
            Assert.Equal(4, linesOfText.Elements.Count);
            Assert.Equal(33, linesOfText.CounterField.Tag);
            Assert.Equal(58, linesOfText.Delimiter.Tag);
            Assert.Equal(new int[] { 58, 999, 354, 355 }, linesOfText.ElementOrder.ToArray());
            Assert.Equal(new int[] { 58, 999 }, linesOfText.RequiredElements.OrderBy(x => x).ToArray());

            var noDoots = (DDGroup)msg.Elements[500];
            Assert.Single(noDoots.Elements);
            Assert.Equal(500, noDoots.CounterField.Tag);
            Assert.Equal(501, noDoots.Delimiter.Tag);
            Assert.Single(noDoots.ElementOrder);
            Assert.Single(noDoots.RequiredElements);
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.NestedGroup.txt")]
        public async Task NestedGroup(string resourceName)
        {
            var nestedGroupMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(nestedGroupMessage, ParserTask.Messages);
            Assert.Single(dd.Messages);

            var linesOfText = (DDGroup)dd.Messages["B"].Elements[33];
            Assert.Equal(3, linesOfText.Elements.Count);
            Assert.Equal(new int[] { 58, 200, 300 }, linesOfText.ElementOrder.ToArray());
            Assert.Equal(new int[] { 58, 300 }, linesOfText.RequiredElements.OrderBy(x => x).ToArray());

            var fooNest = (DDGroup)linesOfText.Elements[200];
            Assert.Equal(200, fooNest.CounterField.Tag);
            Assert.Equal(201, fooNest.Delimiter.Tag);
            Assert.Equal(new int[] { 201, 202 }, fooNest.ElementOrder.ToArray());
            Assert.Equal(new int[] { 201, 202 }, fooNest.RequiredElements.OrderBy(x => x).ToArray());

            var barNest = (DDGroup)linesOfText.Elements[300];
            Assert.Equal(300, barNest.CounterField.Tag);
            Assert.Equal(301, barNest.Delimiter.Tag);
            Assert.Equal(new int[] { 301, 302 }, barNest.ElementOrder.ToArray());
            Assert.Equal(new int[] { 301 }, barNest.RequiredElements.OrderBy(x => x).ToArray());
            Assert.Single(barNest.RequiredElements);
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.DelimiterIsNestedGroup.txt")]
        public async Task DelimiterIsNestedGroup(string resourceName)
        {
            var delimiterNestedGroupMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(delimiterNestedGroupMessage, ParserTask.Messages);

            var group11 = (DDGroup)dd.Messages["B"].Elements[11];
            Assert.Single(group11.Elements);
            Assert.Equal(12, group11.Delimiter.Tag);

            var group12 = (DDGroup)group11.Delimiter;
            Assert.Single(group12.Elements);
            Assert.Equal(13, group12.Delimiter.Tag);

            var group13 = (DDGroup)group12.Delimiter;
            Assert.Equal(2, group13.Elements.Count);
            Assert.Equal(100, group13.Delimiter.Tag);
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.FlatComponents.txt")]
        public async Task FlatComponents(string resourceName)
        {
            var flatComponentsMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(flatComponentsMessage, ParserTask.Messages);
            var msg = dd.Messages["B"];
            Assert.Equal(4, msg.Elements.Count);
            Assert.Equal(new int[] { 66, 67, 77, 78 }, msg.ElementOrder.ToArray());
            Assert.Equal(new int[] { 66, 78 }, msg.RequiredElements.OrderBy(x => x).ToArray());
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.ComponentIsGroup.txt")]
        public async Task ComponentIsGroup(string resourceName)
        {
            var componentGroupMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(componentGroupMessage, ParserTask.Messages);
            var msg = dd.Messages["B"];
            Assert.Equal(2, msg.Elements.Count);
            Assert.Equal(new int[] { 148, 900 }, msg.ElementOrder.ToArray());
            Assert.Equal(new int[] { 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var group = (DDGroup)msg.Elements[900];
            Assert.Equal(2, group.Elements.Count);
            Assert.Equal(new int[] { 66, 67 }, group.ElementOrder.ToArray());
        }

        [Theory]
        [InlineData("UnitTests.Resources.Messages.ComponentContentIsGroup.txt")]
        public async Task ComponentContentIsGroup(string resourceName)
        {
            var componentContentGroupMessage = await resourceName.GetResourceStringAsync<MessageParserTests>();
            var dd = ParserTestUtil.ReadDD(componentContentGroupMessage, ParserTask.Messages);
            var msg = dd.Messages["B"];
            Assert.Equal(2, msg.Elements.Count);
            Assert.Equal(new int[] { 148, 900 }, msg.ElementOrder.ToArray());
            Assert.Equal(new int[] { 148 }, msg.RequiredElements.OrderBy(x => x).ToArray());

            var group = (DDGroup)msg.Elements[900];
            Assert.Equal(2, group.Elements.Count);
            Assert.Equal(new int[] { 66, 67 }, group.ElementOrder.ToArray());
        }
    }
}
