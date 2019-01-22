using System;
using System.Collections.Generic;
using System.Text;

namespace KLib
{
    public class ResourceInfoGenerator : IResourceInfoGenerator
    {

        public String resourceInfoFileName
        {
            get
            {
                return "fileInfo.txt";
            }
        }

        public String buildVersionFileName
        {
            get
            {
                return "buildVersion.js";
            }
        }

        public Byte[] resourceInfoToBytes(ResourceInfo[] resInfoList)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            int c = resInfoList.Length;
            while (i < c)
            {
                ResourceInfo resInfo = resInfoList[i];

                sb.Append(resInfo.name);
                sb.Append(",");
                sb.Append(resInfo.version);
                sb.Append(",");
                sb.Append(Convert.ToString(resInfo.bytesTotal));
                sb.Append("\r\n");

                i++;
            }

            if (resInfoList.Length != 0) sb.Remove(sb.Length - 2, 2);

            Byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return bytes;
        }

        public Byte[] buildVersionToBytes(String buildVersion)
        {

            String ver = @"function getBuildVersion() {
    return " + buildVersion + @"
}
";
            Byte[] bytes = Encoding.UTF8.GetBytes(ver);
            //Byte[] bytes = Encoding.UTF8.GetBytes(buildVersion);
            return bytes;
        }

    }
}
