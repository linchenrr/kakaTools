using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KLib
{
    public interface ICompresser
    {
        void compress(Stream inStream, Stream outStream);

        void uncompress(Stream inStream, Stream outStream);
    }
}
