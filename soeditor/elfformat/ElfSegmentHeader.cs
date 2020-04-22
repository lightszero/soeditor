using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfformat
{
    public class ElfSegmentHeader
    {
        public SegmentType Type;
        public SegmentFlags Flags;
        public UInt64 Offset;
        public UInt64 Address;
        public UInt64 PhysicalAddress;
        public UInt64 FileSize;
        public UInt64 Size;
        public UInt64 Alignment;

        public void Read(System.IO.Stream stream, ElfDocumentHeader header)
        {
            Type = (SegmentType)stream.ReadUInt32(header);

            if (header.Head_Is64Bit)
            {
                Flags = (SegmentFlags)stream.ReadUInt32(header);
            }
            // TODO: some functions?s
            Offset = stream.ReadUIntPtr(header);
            Address = stream.ReadUIntPtr(header);
            PhysicalAddress = stream.ReadUIntPtr(header);
            FileSize = stream.ReadUIntPtr(header);
            Size = stream.ReadUIntPtr(header);
            if (!header.Head_Is64Bit)
            {
                Flags = (SegmentFlags)stream.ReadUInt32(header);
            }

            Alignment = stream.ReadUIntPtr(header);

        }


        public void Dump(Action<string> logfunc)
        {
            logfunc(":ElfSegmentHeader");

            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }
        }
    }
}
