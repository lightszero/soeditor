using System;
using System.Collections.Generic;
using System.Text;

namespace soeditor.elfparse
{
    public enum SpecialSectionIndex : ushort
    {
        Absolute = 0,
        Common = 0xFFF1,
        Undefined = 0xFFF2
    }
    public enum SymbolBinding
    {
        Local,
        Global,
        Weak,
        ProcessorSpecific
    }
    public enum SymbolType
    {
        NotSpecified,
        Object,
        Function,
        Section,
        File,
        ProcessorSpecific
    }
}
