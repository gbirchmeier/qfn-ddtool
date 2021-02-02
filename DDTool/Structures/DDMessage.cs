using System;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public class DDMessage
    {
        public string Name { get; private set; }
        public string MsgType { get; private set; }
        public string Cat { get; private set; }

        public DDMessage(string name, string msgType, string msgCat)
        {
            Name = name;
            MsgType = msgType;
            Cat = msgCat;
        }
    }
}