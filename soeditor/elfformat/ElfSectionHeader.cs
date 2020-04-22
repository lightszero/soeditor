using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfformat
{
    public class ElfSectionHeader
    {
        public UInt32 NameIndex;
        public SectionType Type;
        public SectionFlags Flags;
        bool Flag_Writable;
        bool Flag_Allocatable;
        bool Flag_Executable;
        public UInt64 LoadAddress;
        public UInt64 Offset;
        public UInt64 Size;
        public UInt32 Link;
        public UInt32 Info;
        public UInt64 Alignment;
        public UInt64 EntrySize;

        public void Read(System.IO.Stream stream, ElfDocumentHeader header)
        {
            NameIndex = stream.ReadUInt32(header);
            Type = (SectionType)stream.ReadUInt32(header);
            var RawFlags = stream.ReadUIntPtr(header);
            Flags = unchecked((SectionFlags)RawFlags);
            Flag_Writable = (Flags & SectionFlags.Writable) > 0;
            Flag_Allocatable = (Flags & SectionFlags.Allocatable) > 0;
            Flag_Executable = (Flags & SectionFlags.Executable) > 0;

            LoadAddress = stream.ReadUIntPtr(header);
            Offset = stream.ReadUIntPtr(header);
            Size = stream.ReadUIntPtr(header);
            Link = stream.ReadUInt32(header);
            Info = stream.ReadUInt32(header);
            Alignment = stream.ReadUIntPtr(header);
            EntrySize = stream.ReadUIntPtr(header);
        }

        public void Dump(Action<string> logfunc)
        {
            logfunc(":ElfSectionHeader");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }
        }
    }
}
