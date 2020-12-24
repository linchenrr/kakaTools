using System;
using System.Collections.Generic;
using System.Text;
using static Unity3dPacth.Lib.PatchToolsUtility;

namespace Unity3dPacth.Lib
{
    public class PatchUnityEXE
    {
        public static FilePatchCode[] FilePatchCodes = new FilePatchCode[]
        {
            //2019.4
            new FilePatchCode
            {
                 patchCodes=new PatchCode[]
                 {
                     new PatchCode
                     {
                         SignatureCode = new byte[] { 0x55,0x57,0x41,0x56,0x48,0x8D,0xA8,0x68,0xFD,0xFF,0xFF,0x48,0x81 },
                         Patch = new byte[] { 0xC3 }
                     }
                 }
            },
            //2020.1
            new FilePatchCode
            {
                 patchCodes=new PatchCode[]
                 {
                     new PatchCode
                     {
                         SignatureCode = new byte[] { 0x75, 0x11, 0xB8, 0x02, 0x00, 0x00, 0x00, 0xE9 },
                         Patch = new byte[] { 0xEB }
                     }
                 }
            },
            //2020.2
            new FilePatchCode
            {
                 patchCodes=new PatchCode[]
                 {
                     new PatchCode
                     {
                         SignatureCode = new byte[] { 0x75,0x16,0xB8,0x02,0x00,0x00,0x00,0xE9,0xEA,0x03,0x00 },
                         Patch = new byte[] { 0xEB }
                     }
                 }
            },
        };
    }
}
