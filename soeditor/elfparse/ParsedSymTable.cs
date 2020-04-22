using System;
using System.Collections.Generic;
using System.Text;
using soeditor.elfformat;

namespace soeditor.elfparse
{
    public class Symbol
    {
        public UInt32 st_name;
        public UInt64 st_value;
        public UInt64 st_size;
        public byte st_info;
        public byte st_other;
        public UInt16 st_shndx;

        public string Name;
        public SymbolType SymbolType;
        public SymbolBinding SymbolBinding;
        public SpecialSectionIndex? SpecialSectionIndex;
        public void Dump(Action<string> logfunc)
        {
            logfunc("  :Symbol");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }
        }
    }
    public class ParsedSymTable
    {
        public string Name;
        public List<Symbol> symbols = new List<Symbol>();
        public void Read(ParsedDocument doc, ElfSectionHeader section, byte[] data)
        {
            var header = doc.document.header;
            symbols.Clear();
            using (var ms = new System.IO.MemoryStream(data))
            {
                int sylsize = header.Head_Is64Bit ? 24 : 16;
                for (var i = 0; i < data.Length; i += sylsize)
                {
                    Symbol symbol = new Symbol();
                    if (header.Head_Is64Bit)//64位和32位格式，顺序不太一样
                    {
                        symbol.st_name = ms.ReadUInt32(header);
                        symbol.st_info = ms.ReadByte1();
                        symbol.st_other = ms.ReadByte1();
                        symbol.st_shndx = ms.ReadUInt16(header);
                        symbol.st_value = ms.ReadUIntPtr(header);
                        symbol.st_size = ms.ReadUIntPtr(header);
                    }
                    else
                    {
                        symbol.st_name = ms.ReadUInt32(header);
                        symbol.st_value = ms.ReadUIntPtr(header);
                        symbol.st_size = ms.ReadUIntPtr(header);
                        symbol.st_info = ms.ReadByte1();
                        symbol.st_other = ms.ReadByte1();
                        symbol.st_shndx = ms.ReadUInt16(header);
                    }
                    if (doc.Parsed_StringTables.ContainsKey((int)section.Link))
                    {
                        var stable = doc.Parsed_StringTables[(int)section.Link];
                        stable.TryGetValue((int)symbol.st_name, out symbol.Name);
                    }
                    symbol.SymbolBinding = (SymbolBinding)(symbol.st_info >> 4);
                    symbol.SymbolType = (SymbolType)(symbol.st_info & 0x0F);
                    if (Enum.IsDefined(typeof(SpecialSectionIndex), symbol.st_shndx))
                    {
                        symbol.SpecialSectionIndex = (SpecialSectionIndex)symbol.st_shndx;
                    }
                    else
                    {
                        symbol.SpecialSectionIndex = null;
                    }


                    symbols.Add(symbol);
                }
            }
        }
        public void Dump(Action<string> logfunc)
        {
            logfunc(":SymTable");
            var fields = this.GetType().GetFields();
            foreach (var f in fields)
            {
                var obj = f.GetValue(this);
                logfunc("     " + f.Name + "=" + Util.GetDebugString(obj));
            }

            foreach (var s in symbols)
            {
                logfunc("sym:" + symbols.IndexOf(s));
                s.Dump(logfunc);
            }
        }
    }
}
