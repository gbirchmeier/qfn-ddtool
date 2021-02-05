using System;
using System.Collections.Generic;
using DDTool.Exceptions;

namespace DDTool.Structures
{
    public class DDGroup : IElement, IElementSequence
    {
        public int Tag { get { return CounterField.Tag; } }
        public string Name { get { return CounterField.Name; } }

        public DDField CounterField { get; private set; }
        public IElement DelimiterElement { get; private set; }

        /// <summary>
        /// Does not include delimiter (which is obviously present)
        /// </summary>
        public Dictionary<int, IElement> Elements { get; } = new Dictionary<int, IElement>();

        /// <summary>
        /// Does not include delimiter (which is always required)
        /// </summary>
        public HashSet<int> RequiredElements { get; } = new HashSet<int>();

        /// <summary>
        /// Does not include delimiter (which is alwasy first)
        /// </summary>
        public List<int> ElementOrder { get; } = new List<int>();

        public DDGroup(DDField counter, IElement delimiter) // TODO add ref to container, so exception can backtrace it
        {
            CounterField = counter;
            DelimiterElement = delimiter;
        }

        public void AddField(DDField field, bool required)
        {
            ElementSequenceImpl.AddField(this, field, required);
        }

        public void AddGroup(DDGroup group, bool required)
        {
            ElementSequenceImpl.AddGroup(this, group, required);
        }
    }
}
