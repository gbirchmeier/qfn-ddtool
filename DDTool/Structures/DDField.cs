using System;
using System.Collections.Generic;

namespace DDTool.Structures;

public class DDField : IElement {
    public int Tag { get; }
    public string Name { get; private set; }
    public Dictionary<string, string> EnumDict { get; private set; }
    public string FixFieldType { get; private set; }

    /// <summary>
    /// Represents a field from a DataDictionary file.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="name"></param>
    /// <param name="enums">dictionary of enum=>description values</param>
    /// <param name="fixFldType"></param>
    public DDField(int tag, string name, Dictionary<string, string> enums, string fixFieldType) {
        this.Tag = tag;
        this.Name = name;
        this.EnumDict = new Dictionary<string, string>(enums);
        this.FixFieldType = fixFieldType.ToUpperInvariant();
    }
}
