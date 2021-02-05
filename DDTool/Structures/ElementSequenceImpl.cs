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
        public static void AddField(IElementSequence seq, DDField field, bool required)
        {
            if (seq.Elements.ContainsKey(field.Tag))
                throw new ParsingException($"Field tag {field.Tag} appears twice in top-level of {SeqKind(seq)} {seq.Name}");

            seq.Elements[field.Tag] = field;
            seq.ElementOrder.Add(field.Tag);
            if (required)
                seq.RequiredElements.Add(field.Tag);
        }

        public static void AddGroup(IElementSequence seq, DDGroup group, bool required)
        {
            if (seq.Elements.ContainsKey(group.Tag))
                throw new ParsingException($"Group tag {group.Tag} appears twice in top-level of {SeqKind(seq)} {seq.Name}");

            seq.Elements[group.Tag] = group;
            seq.ElementOrder.Add(group.Tag);
            if (required)
                seq.RequiredElements.Add(group.Tag);
        }

        // This is a lot of lines for a dumb utility function, but here we are.
        private static string SeqKind(IElementSequence seq)
        {
            if (seq.GetType() == typeof(DDMessage))
                return "message";
            else if (seq.GetType() == typeof(DDGroup))
                return "group";
            throw new NotImplementedException($"This function not implemented for type {seq.GetType().Name}");
        }
    }
}
