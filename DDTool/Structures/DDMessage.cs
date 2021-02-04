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
            if (Elements.ContainsKey(field.Tag))
                throw new ParsingException($"Field tag {field.Tag} appears twice in top-level of message {MsgType}/{Name}");

            Elements[field.Tag] = field;
            ElementOrder.Add(field.Tag);
            if (required)
                RequiredElements.Add(field.Tag);
        }

        public void AddGroup(DDGroup group, bool required)
        {
            if (Elements.ContainsKey(group.Tag))
                throw new ParsingException($"Group tag {group.Tag} appears twice in top-level of message {MsgType}/{Name}");

            Elements[group.Tag] = group;
            ElementOrder.Add(group.Tag);
            if (required)
                RequiredElements.Add(group.Tag);
        }
    }
}