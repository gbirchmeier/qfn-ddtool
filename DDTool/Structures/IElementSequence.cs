using System;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public interface IElementSequence
    {
        public string Name { get; }

        public Dictionary<int, IElement> Elements { get; }
        public HashSet<int> RequiredElements { get; }
        public List<int> ElementOrder { get; }

        public void AddField(DDField field, bool required);
        public void AddGroup(DDGroup group, bool required);
    }
}
