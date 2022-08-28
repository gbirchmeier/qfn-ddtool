using System;
using DDTool.Exceptions;

namespace DDTool.Structures
{
    /* It was either this, or create an AbstractElementSequence class
         and further muddy up the class hierarchy.
       I'm still not sure which is less ugly.
       */

    public static class ElementSequenceImpl
    {
        public static void AddElement(IElementSequence seq, IElement element, bool required)
        {
            if (seq.Elements.ContainsKey(element.Tag))
                throw new ParsingException($"Tag {element.Tag} appears twice in {seq.Name}");

            seq.Elements[element.Tag] = element;
            seq.ElementOrder.Add(element.Tag);
            if (required)
                seq.RequiredElements.Add(element.Tag);
        }
    }
}
