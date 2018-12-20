using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KLib
{
    public class KLibInvalid
    {
        //是否已经过期
        static public bool IsInvalid
        {
            get
            {
                var date = DateTime.Now;
                return date > ExpiresTime;
            }
        }

        static public DateTime ExpiresTime
        {
            get { return new DateTime(2019, 3, 22); }
        }

    }
}
