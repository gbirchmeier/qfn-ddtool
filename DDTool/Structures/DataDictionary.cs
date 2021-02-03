using System;
using System.Collections.Generic;
using DDTool.Exceptions;

namespace DDTool.Structures
{
    public class DataDictionary
    {
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string ServicePack { get; set; }
        public bool IsFIXT { get; set; } = false;

        public Dictionary<int, DDField> FieldsByTag { get; } = new Dictionary<int, DDField>();
        public Dictionary<string, DDField> FieldsByName { get; } = new Dictionary<string, DDField>();
        public Dictionary<string, DDMessage> Messages { get; } = new Dictionary<string, DDMessage>();

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
                throw new ParsingException($"Field tag is defined twice: {fld.Tag}");
            FieldsByTag[fld.Tag] = fld;

            if (FieldsByName.ContainsKey(fld.Name))
                throw new ParsingException($"Field name is defined twice: {fld.Tag}");
            FieldsByName[fld.Name] = fld;
        }

        public void AddMessage(DDMessage msg)
        {
            if (Messages.ContainsKey(msg.MsgType))
                throw new ParsingException($"Message type is defined twice: {msg.MsgType}");
            Messages[msg.MsgType] = msg;
        }
    }
}