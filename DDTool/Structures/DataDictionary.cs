using System;
using System.Collections.Generic;
using System.Linq;

namespace DDTool.Structures
{
    public class DataDictionary
    {
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string ServicePack { get; set; }
        public bool IsFIXT { get; set; } = false;

        private Dictionary<int, DDField> _fieldsByTag = new Dictionary<int, DDField>();

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
            _fieldsByTag[fld.Tag] = fld;
        }
    }
}