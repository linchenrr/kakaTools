using System;
using System.Collections.Generic;
using System.Text;

namespace KLib
{
    public class KRPCError
    {

        public int id;
        public String message;

        public KRPCError(int id = 0, String message = "")
        {

            this.id = id;
            this.message = message;

        }

    }
}
