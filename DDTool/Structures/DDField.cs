using System;
using System.Collections.Generic;

namespace DDTool.Structures
{
    public class DDField
    {
        public int Tag { get; private set; }
        public String Name { get; private set; }
        public Dictionary<String, String> EnumDict { get; private set; }
        public String FixFieldType { get; private set; }

        /// <summary>
        /// Represents a field from a DataDictionary file.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="name"></param>
        /// <param name="enums">dictionary of enum=>description values</param>
        /// <param name="fixFldType"></param>
        public DDField(int tag, String name, Dictionary<String, String> enums, String fixFieldType)
        {
            this.Tag = tag;
            this.Name = name;
            this.EnumDict = new Dictionary<string, string>(enums);
            this.FixFieldType = fixFieldType.ToUpperInvariant();
        }
    }
}
