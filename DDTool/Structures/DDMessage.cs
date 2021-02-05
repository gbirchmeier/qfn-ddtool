using System;
using System.Collections.Generic;
using DDTool.Exceptions;

namespace DDTool.Structures
{
    public class DDMessage : IElementSequence
    {
        public string Name { get; private set; }
        public string MsgType { get; private set; }
        public string Cat { get; private set; }

        public Dictionary<int, IElement> Elements { get; } = new Dictionary<int, IElement>();
        public HashSet<int> RequiredElements { get; } = new HashSet<int>();
        public List<int> ElementOrder { get; } = new List<int>();

        public DDMessage(string name, string msgType, string msgCat)
        {
            Name = name;
            MsgType = msgType;
            Cat = msgCat;
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