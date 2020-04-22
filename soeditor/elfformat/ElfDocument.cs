using System;
using System.Collections.Generic;
using System.Text;
using soeditor.elfformat;
namespace soeditor.elfformat
{

    public class ElfDocument
    {
        public ElfDocumentHeader header;
        public List<ElfSectionHeader> sectionHeaders = new List<ElfSectionHeader>();
        public List<byte[]> sectionRaws = new List<byte[]>();
        public Dictionary<int, string> stringTable = new Dictionary<int, string>();
        public List<ElfSegmentHeader> segments = new List<ElfSegmentHeader>();

        public void Read(System.IO.Stream stream)
        {
            //read header;
            this.header = new ElfDocumentHeader();
            header.Read(stream);
            //read sectionheaders
            sectionHeaders.Clear();
            for (var i = 0; i < header.Head_SectionHeaderEntryCount; i++)
            {
                long offset = (long)header.Head_SectionHeaderOffset + (long)i * header.Head_SectionHeaderEntrySize;
                stream.Seek(offset, System.IO.SeekOrigin.Begin);
                ElfSectionHeader _header = new ElfSectionHeader();
                _header.Read(stream, header);
                long offsetend = stream.Position;
                var size = offsetend - offset;
                if (size != header.Head_SectionHeaderEntrySize)
                {
                    throw new Exception("Error Section Read");
                }
                sectionHeaders.Add(_header);
            }
            //read sectiondata
            sectionRaws.Clear();
            for (var i = 0; i < header.Head_SectionHeaderEntryCount; i++)
            {
                byte[] data = new byte[this.sectionHeaders[i].Size];
                var offset = (int)this.sectionHeaders[i].Offset;
                stream.Seek(offset, System.IO.SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                sectionRaws.Add(data);
            }
            //read segmentheaders
            for (var i = 0; i < header.Head_SegmentHeaderEntryCount; i++)
            {
                long offset = (long)header.Head_SegmentHeaderOffset + (long)i * header.Head_SegmentHeaderEntrySize;
                stream.Seek(offset, System.IO.SeekOrigin.Begin);
                ElfSegmentHeader segment = new ElfSegmentHeader();
                segment.Read(stream, header);
                segments.Add(segment);
            }
            Read_StringTable();
        }

        public string GetSctionName(ElfSectionHeader section)
        {
            var ni = (int)section.NameIndex;
            if (stringTable.ContainsKey(ni) == false)
                return "";
            return stringTable[(int)section.NameIndex];
        }
        public void Dump(Action<string> logfunc)
        {
            logfunc("Dump ElfDocument");
            header.Dump(logfunc);
            for (var i = 0; i < sectionHeaders.Count; i++)
            {
                var sec = sectionHeaders[i];
                var name = GetSctionName(sec);
                logfunc("[Section:" + name + "," + i + "]");
                sec.Dump(logfunc);
            }
            for (var i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                logfunc("[Segment:" + i + "]");
                seg.Dump(logfunc);
            }
        }

        private void Read_StringTable()
        {
            this.stringTable.Clear();

            if (this.header.Head_StringTableIndex == 0)
                return;
            ////string表是其中一个Section
            var stringsection = this.sectionRaws[this.header.Head_StringTableIndex];
            this.stringTable = ElfIO.ReadStringTableFromData(stringsection);
        }
        public void Write(System.IO.Stream stream)
        {

        }
    }
}
