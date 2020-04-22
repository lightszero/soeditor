using soeditor.elfformat;
using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfparse
{
    public class ParsedDocument
    {
        public soeditor.elfformat.ElfDocument document;
        public Dictionary<int, ParsedSymTable> Parsed_SymTables = new Dictionary<int, ParsedSymTable>();
        public Dictionary<int, Dictionary<int, String>> Parsed_StringTables = new Dictionary<int, Dictionary<int, string>>();
        public Dictionary<int, ParsedRelocationTable> Parsed_RelocationTables = new Dictionary<int, ParsedRelocationTable>();
        public void Parse(soeditor.elfformat.ElfDocument document)
        {
            this.document = document;
            //先处理所有的string表，由于可能有多个string表
            Parsed_StringTables.Clear();
            Parsed_StringTables[document.header.Head_StringTableIndex] = document.stringTable;
            for (var i = 0; i < document.sectionHeaders.Count; i++)
            {
                var sec = document.sectionHeaders[i];
                var data = document.sectionRaws[i];

                if (sec.Type == elfformat.SectionType.StringTable && Parsed_StringTables.ContainsKey(i) == false)
                {
                    Parsed_StringTables[i] = ElfIO.ReadStringTableFromData(data);
                    //readStringTable
                }
            }
            for (var i = 0; i < document.sectionHeaders.Count; i++)
            {
                var sec = document.sectionHeaders[i];
                var data = document.sectionRaws[i];
                //处理符号表
                if (sec.Type == elfformat.SectionType.DynamicSymbolTable
                    || sec.Type == elfformat.SectionType.SymbolTable)
                {
                    ParsedSymTable symTable = new ParsedSymTable();
                    symTable.Name = document.GetSctionName(sec);
                    if (document.header.Head_Is64Bit && sec.EntrySize != 24)
                        throw new Exception("Error EntrySize");
                    if (document.header.Head_Is64Bit == false && sec.EntrySize != 16)
                        throw new Exception("Error EntrySize");
                    symTable.Read(this, sec, data);
                    Parsed_SymTables.Add(i, symTable);
                }
                //处理重定向节
                if (sec.Type == SectionType.Relocation || sec.Type == SectionType.RelocationAddends)
                {
                    ParsedRelocationTable table = new ParsedRelocationTable(sec.Type == SectionType.RelocationAddends);
                    table.Name = document.GetSctionName(sec);
                    table.Read(this, sec, data);
                    Parsed_RelocationTables.Add(i, table);
                }

            }
        }
        public void Dump(Action<string> logfunc)
        {
            logfunc("==Dump ParsedDocument==");
            foreach (var i in Parsed_SymTables.Keys)
            {
                logfunc("[SymTable:" + i + "]");
                Parsed_SymTables[i].Dump(logfunc);
            }
            foreach (var i in Parsed_RelocationTables.Keys)
            {
                logfunc("[RelocationTable:" + i + "]");
                Parsed_RelocationTables[i].Dump(logfunc);
            }
        }
    }
}
