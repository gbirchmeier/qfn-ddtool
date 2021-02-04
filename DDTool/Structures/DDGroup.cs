using System;
using System.Collections.Generic;
using DDTool.Exceptions;

namespace DDTool.Structures
{
    public class DDGroup : IElement, IElementSequence
    {
        public int Tag { get { return CounterField.Tag; } }

        public DDField CounterField { get; private set; }
        public IElement DelimiterElement { get; private set; }

        public Dictionary<int, IElement> Elements { get; } = new Dictionary<int, IElement>();
        public HashSet<int> RequiredElements { get; } = new HashSet<int>();
        public List<int> ElementOrder { get; } = new List<int>();

        public DDGroup(DDField counter, IElement delimiter) // TODO add ref to container, so exception can backtrace it
        {
            CounterField = counter;
            DelimiterElement = delimiter;
        }

        public void AddField(DDField field, bool required)
        {
            if (Elements.ContainsKey(field.Tag))
                throw new ParsingException($"Field tag {field.Tag} appears twice in group {Tag}");

            Elements[field.Tag] = field;
            ElementOrder.Add(field.Tag);
            if (required)
                RequiredElements.Add(field.Tag);
        }
    }
}
