using System;
using System.Collections.Generic;
using System.Text;
using KLib;

namespace KLib
{
	public interface IResourceInfoGenerator
	{

        String resourceInfoFileName
        {
            get;
        }

        String buildVersionFileName
        {
            get;
        }

        Byte[] resourceInfoToBytes(ResourceInfo[] resInfoList);

        Byte[] buildVersionToBytes(String buildVersion);
        
           
	}
}
