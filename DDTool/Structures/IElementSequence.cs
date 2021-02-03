using System;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public interface IElementSequence
    {
        public Dictionary<int, IElement> Elements { get; }
        public HashSet<int> RequiredElements { get; }
        public List<int> ElementOrder { get; }
    }
}
