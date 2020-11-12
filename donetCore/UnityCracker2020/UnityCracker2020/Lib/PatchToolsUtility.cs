using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UtilityToolsCollect.ExtensionMethodLibrary.BytesEx;

namespace Unity3dPacth.Lib
{
    public static class PatchToolsUtility
    {
        /****************************Patch工具****************************/
        public static bool Patch(this PatchCode Source, byte[] FileBytes)
        {
            if (!Source || FileBytes == null || FileBytes.Length < 1)
                return false;
            long[] Indexs = FileBytes.IndexOf(Source.SignatureCode).ToArray();
            if (!Source.EnableAllPatchPosition && Indexs.Length != Source.PatchPositionNum)
                return false;
            Array.ForEach(Indexs, Index => Source.Patch.CopyTo(FileBytes, Index));
            return true;
        }
        public static bool Patch(this FilePatchCode Source, ref byte[] FileBytes,ref byte[] Buffer)
        {
            if (!Source || FileBytes == null || FileBytes.Length < 1) 
                return false;
            if (Buffer == null || FileBytes.Length != Buffer.Length)
                Buffer = new byte[FileBytes.Length];

            System.Buffer.BlockCopy(FileBytes, 0, Buffer, 0, FileBytes.Length);

            foreach (ref readonly PatchCode item in Source.PatchCodes)
            {
                if (!item) continue;
                if (!item.Patch(Buffer))
                    return false;
            }
            FileBytes = Buffer;
            return true;
        }
        public static bool Patch(this FilePatchCode[] Source, ref byte[] FileBytes)
        {
            byte[] Buffer = null;
            foreach (ref readonly FilePatchCode item in (ReadOnlySpan<FilePatchCode>)Source)
                if (item.Patch(ref FileBytes, ref Buffer))
                    return true;
            return false;
        }
        public static bool Patch(this FileInfo fileInfo, FilePatchCode[] patchCodes,out byte[] FileBytes)
        {
            FileBytes = File.ReadAllBytes(fileInfo.FullName);
            return patchCodes.Patch(ref FileBytes);
        }

        public static bool Patch(string path, FilePatchCode[] patchCodes, out byte[] FileBytes)
        {
            FileBytes = File.ReadAllBytes(path);
            return patchCodes.Patch(ref FileBytes);
        }

        /****************************数据定义****************************/
        /// <summary>
        /// Patch
        /// </summary>
        public class PatchCode :IEquatable<PatchCode>,IEqualityComparer<PatchCode>
        {
            public byte[] SignatureCode; public byte[] Patch;
            private int hashCode;
            public int PatchPositionNum=1;
            public bool EnableAllPatchPosition=false;
            public override bool Equals(object obj) => Equals((PatchCode)obj);
            public bool Equals([AllowNull] PatchCode other)
            {
                if (!(other && this)) return false;
                if (!SignatureCode.EqualsEXD(other.SignatureCode))
                    return false;
                if (!Patch.EqualsEXD(other.Patch))
                    return false;
                return true;
            }
            public bool Equals([AllowNull] PatchCode x, [AllowNull] PatchCode y) => x.Equals(y);
            public override int GetHashCode()
            {
                if (hashCode != 0) return hashCode;
                if (!this) return 0;
                byte[] buffer = new byte[SignatureCode.Length + Patch.Length];
                Buffer.BlockCopy(SignatureCode, 0, buffer, 0, SignatureCode.Length);
                Buffer.BlockCopy(Patch, 0, buffer, SignatureCode.Length, Patch.Length);
                return hashCode = Convert.ToBase64String(buffer).GetHashCode();
            }
            public int GetHashCode([DisallowNull] PatchCode obj) => obj.GetHashCode();

            public static implicit operator bool(PatchCode value) => 
                value.SignatureCode != null && value.SignatureCode.Length > 0 && value.Patch != null && value.Patch.Length > 0;
        }
        /// <summary>
        /// 一个文件中需要用到的的全部Patch
        /// </summary>
        public class FilePatchCode : IEnumerable<PatchCode>,IEquatable<FilePatchCode>,IEqualityComparer<FilePatchCode>
        {
            public PatchCode[] patchCodes;
            public PatchCode this[long index] { get => patchCodes[index]; set => patchCodes[index] = value; }
            public long Count => patchCodes.Length;
            public ReadOnlySpan<PatchCode> PatchCodes => patchCodes;
            public bool IsAllValid => Array.TrueForAll(patchCodes, p => p);
            private int hashCode;
            public override bool Equals(object obj) => Equals((FilePatchCode)obj);
            public bool Equals([AllowNull] FilePatchCode other)
            {
                if (!this || !other) return false;
                if (Count != other.Count) return false;
                for (int i = 0; i < patchCodes.Length; i++)
                {
                    if (!patchCodes[i].Equals(other[i]))
                        return false;
                }
                return true;
            }
            public bool Equals([AllowNull] FilePatchCode x, [AllowNull] FilePatchCode y) => x.Equals(y);
            public override int GetHashCode()
            {
                if (hashCode != 0) return hashCode;
                if (patchCodes == null || patchCodes.Length < 1) return 0;
                List<byte[]> bytesList = new List<byte[]>(10);
                foreach ( ref readonly PatchCode item in (ReadOnlySpan<PatchCode>)patchCodes)
                { bytesList.Add(item.SignatureCode); bytesList.Add(item.Patch); }
                return hashCode = Convert.ToBase64String(bytesList.SelectMany(p => p).ToArray()).GetHashCode(); 
            }
            public int GetHashCode([DisallowNull] FilePatchCode obj) => obj.GetHashCode();
            public IEnumerator<PatchCode> GetEnumerator() => ((IEnumerable<PatchCode>)patchCodes).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator()=> patchCodes.GetEnumerator();

            public static implicit operator FilePatchCode(PatchCode[] value)=> new FilePatchCode { patchCodes=value };
            public static implicit operator PatchCode[](FilePatchCode value) => value.patchCodes;
            public static implicit operator ReadOnlySpan<PatchCode>(FilePatchCode value) => value.patchCodes;
            public static implicit operator bool(FilePatchCode value) => value.patchCodes != null && value.patchCodes.Length > 0;
        }

        [Serializable]
        public class BufferException : Exception
        {
            public BufferException() { }
            public BufferException(string message) : base(message) { }
            public BufferException(string message, Exception inner) : base(message, inner) { }
            protected BufferException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }


}
