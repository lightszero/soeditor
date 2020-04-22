using soeditor.elfformat;
using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfparse
{
    public class Relocation
    {
        public UInt64 r_offset;
        public UInt64 r_info;
        public UInt64 r_append;

        public UInt32 symAddr;
        public UInt32 relocType;
        public void Dump(Action<string> logfunc)
        {
            logfunc("  :Relocation");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }

        }
    }
    public class ParsedRelocationTable
    {
        public string Name;
        public bool haveAppend;
        public List<Relocation> Relocations = new List<Relocation>();
        public ParsedRelocationTable(bool haveAppend)
        {
            this.haveAppend = haveAppend;
        }

        public void Read(ParsedDocument doc, ElfSectionHeader section, byte[] data)
        {
            var header = doc.document.header;
            Relocations.Clear();
            using (var ms = new System.IO.MemoryStream(data))
            {
                int sylsize = header.Head_Is64Bit ? 8 : 4;
                if (haveAppend)
                    sylsize = sylsize * 3;
                else
                    sylsize = sylsize * 2;
                for (var i = 0; i < data.Length; i += sylsize)
                {
                    Relocation reloc = new Relocation();
                    reloc.r_offset = ms.ReadUIntPtr(header);
                    reloc.r_info = ms.ReadUIntPtr(header);
                    if (this.haveAppend)
                    {
                        reloc.r_append = ms.ReadUIntPtr(header);
                    }
                    if (header.Head_Is64Bit)
                    {
                        reloc.symAddr =(uint)( reloc.r_info >> 32);
                        reloc.relocType = (uint)(reloc.r_info);
                    }
                    else
                    {
                        reloc.symAddr = (uint)(reloc.r_info >> 8);
                        reloc.relocType = (byte)(reloc.r_info);
                    }
                    Relocations.Add(reloc);
                }
            }

        }

        public void Dump(Action<string> logfunc)
        {
            logfunc(":RelocationTable");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                logfunc("  " + f.Name + "=" + f.GetValue(this).ToString());
            }


            foreach (var s in Relocations)
            {
                s.Dump(logfunc);
            }
        }
    }
}
