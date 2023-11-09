using System;
using System.Collections.Generic;

namespace DDTool.Structures;

public class DDMessage : IElementSequence {
    public string Name { get; }
    public string MsgType { get; private set; }
    public string Cat { get; private set; }

    public Dictionary<int, IElement> Elements { get; } = new();
    public HashSet<int> RequiredElements { get; } = new();
    public List<int> ElementOrder { get; } = new();

    public DDMessage(string name, string msgType, string msgCat) {
        Name = name;
        MsgType = msgType;
        Cat = msgCat;
    }

    public void AddElement(IElement element, bool required) {
        ElementSequenceImpl.AddElement(this, element, required);
    }
}