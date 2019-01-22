using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace KLib
{
    public class MD5Utils
    {
        static public string BytesToMD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] outBytes = md5.ComputeHash(bytes);

            var sb = new StringBuilder();
            for (int i = 0; i < outBytes.Length; i++)
            {
                sb.Append(outBytes[i].ToString("x2"));
            }
            return sb.ToString().ToLower();
        }

    }
}
