using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityToolsCollect.ExtensionMethodLibrary.BytesEx
{

    public static class BoyerMooreSearch
    {
        public static IEnumerable<long> IndexOf(this byte[] Source, byte[] Pattern, long StartIndex = 0)
        {
            if (!Check(Source, Pattern))
                yield break;
            long PatternLength = Pattern.Length;
            long Limit = Source.LongLength - PatternLength;
            long LastPatternByte = PatternLength - 1;
            long[] Buffer = GetBuffer(Pattern, PatternLength);
            while (StartIndex <= Limit)
            {
                for (long i = LastPatternByte; Source[StartIndex + i] == Pattern[i]; i--)
                {
                    if (i == 0)
                    {
                        yield return StartIndex;
                        break;
                    }
                }

                StartIndex += Buffer[Source[StartIndex + LastPatternByte]];
            }
        }
        private static long[] GetBuffer(byte[] Pattern, long PatternLength)
        {
            long[] Buffer = new long[256];
            //Buffer.Fill(PatternLength);
            Array.Fill(Buffer, PatternLength);
            //for (long index = 0; index < 256; index++)
            //    Buffer[index] = PatternLength;
            long lastPatternByte = PatternLength - 1;
            for (long i = 0; i < lastPatternByte; i++)
                Buffer[Pattern[i]] = lastPatternByte - i;
            return Buffer;
        }
        private static bool Check(byte[] Source, byte[] Pattern) => Source != null && Source.Length > 0 && Pattern != null && Pattern.Length > 0;
    }

}

