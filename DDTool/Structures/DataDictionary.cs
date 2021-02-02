using System;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public class DataDictionary
    {
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string ServicePack { get; set; }
        public bool IsFIXT { get; set; } = false;

        public Dictionary<int, DDField> FieldsByTag = new Dictionary<int, DDField>();
        public Dictionary<string, DDMessage> Messages = new Dictionary<string, DDMessage>();

        /// <summary>
        /// A combination of type/Major/Minor/SP.
        /// This is NOT the FIX BeginString.
        /// </summary>
        /// <value></value>
        public string Identifier
        {
            get
            {
                var prefix = IsFIXT ? "FIXT" : "FIX";
                var svcPack = string.IsNullOrWhiteSpace(ServicePack) ? "" : $"SP{ServicePack}";
                return $"{prefix}.{MajorVersion}.{MinorVersion}{svcPack}";
            }
        }

        public void AddField(DDField fld)
        {
            if (FieldsByTag.ContainsKey(fld.Tag))
                throw new ApplicationException($"dupe field tag: {fld.Tag}");
            FieldsByTag[fld.Tag] = fld;
        }

        public void AddMessage(DDMessage msg)
        {
            if(Messages.ContainsKey(msg.MsgType))
                throw new ApplicationException($"dupe message type: {msg.MsgType}");
            Messages[msg.MsgType] = msg;
        }
    }
}