using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfformat
{
    public class ElfDocumentHeader
    {
        public string Head_Magic;
        public bool Head_Is64Bit;
        public bool Head_IsLittleEndian;
        public byte Head_Version = 1;
        public byte[] Head_Unuse9 = new byte[9];

        public FileType Head_Type;
        public MachineType Head_Machine;
        public UInt32 Head_FileVersion = 1;
        public UInt64 Head_EntryPoint;
        public UInt64 Head_SegmentHeaderOffset;
        public UInt64 Head_SectionHeaderOffset;
        public UInt32 Head_MachineFlags;
        public UInt16 Head_ELFHeadSize;
        public UInt16 Head_SegmentHeaderEntrySize;
        public UInt16 Head_SegmentHeaderEntryCount;
        public UInt16 Head_SectionHeaderEntrySize;
        public UInt16 Head_SectionHeaderEntryCount;
        public UInt16 Head_StringTableIndex;
        public void Read(System.IO.Stream stream)
        {
            byte[] buf = stream.ReadByte4();
            var str = System.Text.Encoding.ASCII.GetString(buf);
            var magiccheck = "\u007fELF";
            if (str != magiccheck)
                throw new Exception("Not a ELF file.");
            this.Head_Magic = str;

            var classtype = stream.ReadByte();
            if (classtype == 1)
            {
                Head_Is64Bit = false;
            }
            else if (classtype == 2)
            {
                Head_Is64Bit = true;
            }
            else
            {
                throw new Exception("Error ClassType =" + classtype);
            }
            var endianess = stream.ReadByte1();
            if (endianess == 1)
            {
                Head_IsLittleEndian = true;
            }
            else if (endianess == 2)
            {
                Head_IsLittleEndian = false;
            }
            else
            {
                throw new Exception("Error Endianess =" + endianess);
            }

            Head_Version = stream.ReadByte1();
            this.Head_Unuse9 = stream.ReadBytes(9);



            Head_Type = (FileType)stream.ReadUInt16(this);
            Head_Machine = (MachineType)stream.ReadUInt16(this);
            Head_FileVersion = stream.ReadUInt32(this);
            if (Head_FileVersion != 1)
            {
                throw new ArgumentException("unknown version" + Head_FileVersion);
            }
            Head_EntryPoint = stream.ReadUIntPtr(this);
            Head_SegmentHeaderOffset = stream.ReadUIntPtr(this);
            Head_SectionHeaderOffset = stream.ReadUIntPtr(this);
            Head_MachineFlags = stream.ReadUInt32(this);
            Head_ELFHeadSize = stream.ReadUInt16(this); // elf header size
            Head_SegmentHeaderEntrySize = stream.ReadUInt16(this);
            Head_SegmentHeaderEntryCount = stream.ReadUInt16(this);
            Head_SectionHeaderEntrySize = stream.ReadUInt16(this);
            Head_SectionHeaderEntryCount = stream.ReadUInt16(this);
            Head_StringTableIndex = stream.ReadUInt16(this);
        }

        public void Dump(Action<string> logfunc)
        {
            logfunc(":ElfDocumentHeader");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }
        }
    }
}
