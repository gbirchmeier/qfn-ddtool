using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public class DDGroup : IElement, IElementSequence
    {
        public int Tag { get { return CounterField.Tag; } }
        public string Name { get { return CounterField.Name; } }

        public DDField CounterField { get; private set; }
        public IElement Delimiter
        {
            get
            {
                if (ElementOrder.Count < 1)
                    throw new InvalidDataException($"Group {Name}/{Tag} has no elements");
                return Elements[ElementOrder.First()];
            }
        }

        /// <summary>
        /// The first element is the delimiter
        /// </summary>
        public Dictionary<int, IElement> Elements { get; } = new Dictionary<int, IElement>();

        /// <summary>
        /// Includes delimiter
        /// </summary>
        public HashSet<int> RequiredElements { get; } = new HashSet<int>();

        /// <summary>
        /// Includes delimiter (which is always first)
        /// </summary>
        public List<int> ElementOrder { get; } = new List<int>();

        public DDGroup(DDField counter)
        {
            CounterField = counter;
        }

        public void AddElement(IElement element, bool required)
        {
            ElementSequenceImpl.AddElement(this, element, required);
        }
    }
}
