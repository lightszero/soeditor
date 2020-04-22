using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
namespace soeditor.elfformat
{
    public static class ElfIO
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NeedTurn(this ElfDocumentHeader header)
        {
            return BitConverter.IsLittleEndian != header.Head_IsLittleEndian;

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ResortArray(this ElfDocumentHeader header, byte[] src)
        {
            if (BitConverter.IsLittleEndian == header.Head_IsLittleEndian)
                return src;

            byte[] dest = new byte[src.Length];
            unsafe
            {
                int last = src.Length - 1;
                int forlen = src.Length / 2;
                fixed (byte* _src = src)
                {
                    for (var i = 0; i < forlen; i++)
                    {
                        //a = i;
                        //b = last-i;
                        byte t = _src[i];
                        byte b = _src[last - i];
                        _src[i] = b;
                        _src[last - i] = t;
                    }
                }
            }
            return src;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte1(this System.IO.Stream stream)
        {
            byte[] buf = new byte[1];
            stream.Read(buf, 0, 1);
            return buf[0];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadByte4(this System.IO.Stream stream)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);
            return buf;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ReadBytes(this System.IO.Stream stream, int length)
        {
            byte[] buf = new byte[length];
            stream.Read(buf, 0, length);
            return buf;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int16 ReadInt16(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[2];
            stream.Read(buf, 0, 2);
            header.ResortArray(buf);
            return BitConverter.ToInt16(buf, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 ReadInt32(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);
            header.ResortArray(buf);
            return BitConverter.ToInt32(buf, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 ReadInt64(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[8];
            stream.Read(buf, 0, 8);
            header.ResortArray(buf);
            return BitConverter.ToInt64(buf, 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt16 ReadUInt16(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[2];
            stream.Read(buf, 0, 2);
            header.ResortArray(buf);
            return BitConverter.ToUInt16(buf, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt32 ReadUInt32(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[4];
            stream.Read(buf, 0, 4);
            header.ResortArray(buf);
            return BitConverter.ToUInt32(buf, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 ReadUInt64(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            byte[] buf = new byte[8];
            stream.Read(buf, 0, 8);
            header.ResortArray(buf);
            return BitConverter.ToUInt64(buf, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 ReadUIntPtr(this System.IO.Stream stream, ElfDocumentHeader header)
        {
            if (header.Head_Is64Bit)
                return ReadUInt64(stream, header);
            else
                return ReadUInt32(stream, header);
        }

        public static Dictionary<int, string> ReadStringTableFromData(byte[] data)
        {
            var table = new Dictionary<int, string>();


            int start = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    var str = System.Text.Encoding.ASCII.GetString(data, start, i - start);
                    table[start] = str;
                    start = i + 1;
                }
            }

            return table;
        }

    }
}
