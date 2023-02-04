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
        public static TypeInfo Unknown { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "unknown");

        public static TypeInfo Any { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "any");

        public static TypeInfo Integer { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "integer");

        public static TypeInfo Float { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "float");

        public static TypeInfo String { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "string");

        public static TypeInfo AsciiString { get; } = new TypeInfo(Info.SystemTypePackage, "systype", "ascii");

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
