using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
namespace Unity3dPacth.Lib
{
    public static class EqualsEx
    {

        public static bool EqualsEXD([DisallowNull] this byte[] Source, [DisallowNull] byte[] Value)
        {
            if (Source is null || Value is null) return false;
            if (Source.Length != Value.Length) return false;

            if (Source.Length == 1)
                return Source[0] == Value[0];

            long Remainder = Source.Length % 2;
            long count = Source.Length - Remainder;
            long next;
            for (long i = 0; i < count; i += 2)
            {
                if (Source[i] != Value[i] || Source[next = i + 1] != Value[next])
                    return false;
            }
            if (Remainder == 1 && Source[count] != Value[count])
                return false;

            return true;
        }

        static bool EqualsEX([DisallowNull] byte[] Source, [DisallowNull] byte[] Value)
        {
            if (Source is null || Value is null) return false;
            if (Source.Length != Value.Length) return false;

            if (Source.Length == 1)
                return Source[0] == Value[0];

            for (long i = 0; i < Source.Length; i++)
            {
                if (Source[i] != Value[i])
                    return false;
            }
            return true;
        }
    }
}
