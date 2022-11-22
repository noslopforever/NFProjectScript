using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{

    /// <summary>
    /// Common TypeInfos like integer, 
    /// </summary>
    public static class CommonTypeInfos
    {

        public static TypeInfo Unknown { get; } = new TypeInfo(null, "__sys__", "unknown");

        public static TypeInfo Any { get; } = new TypeInfo(null, "__sys__", "any");

        public static TypeInfo Integer { get; } = new TypeInfo(null, "__sys__", "integer");

        public static TypeInfo Float { get; } = new TypeInfo(null, "__sys__", "float");

        public static TypeInfo String { get; } = new TypeInfo(null, "__sys__", "string");

        public static TypeInfo AsciiString { get; } = new TypeInfo(null, "__sys__", "ascii");

        public static bool IsInteger32Type(TypeInfo InType)
        {
            if (InType == Integer
                ) { return true; }
            return false;
        }

        public static bool IsStringType(TypeInfo InType)
        {
            if (InType == String
                || InType == AsciiString
                ) { return true; }
            return false;
        }
    }

}
