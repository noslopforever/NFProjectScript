using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{
    /// <summary>
    /// A special info that stores all system-types.
    /// </summary>
    public class SystemTypePackageInfo
        : Info
    {
        private SystemTypePackageInfo()
            : base(null, "__sys__", "system")
        {
        }

        public static Info Instance { get; } = new SystemTypePackageInfo();

    }



    /// <summary>
    /// Common TypeInfos like integer, 
    /// </summary>
    public static class CommonTypeInfos
    {

        public static TypeInfo Unknown { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "unknown");

        /// <summary>
        /// The 'Any' type which can store any values and objects.
        /// </summary>
        public static TypeInfo Any { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "any");

        /// <summary>
        /// Integer numbers.
        /// </summary>
        public static TypeInfo Integer { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "integer");

        /// <summary>
        /// Floating numbers.
        /// </summary>
        public static TypeInfo Float { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "float");

        /// <summary>
        /// Strings with encodes.
        /// </summary>
        public static TypeInfo String { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "string");

        /// <summary>
        /// Strings which only store ascii-codes.
        /// </summary>
        public static TypeInfo AsciiString { get; } = new TypeInfo(SystemTypePackageInfo.Instance, "systype", "ascii");

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
