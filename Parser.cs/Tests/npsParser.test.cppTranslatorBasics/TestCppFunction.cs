using System.Collections.Generic;

namespace nf.protoscript.test
{
    class CppFunction
    {

        public CppFunction(Info InContextInfo)
        {
            ContextInfo = InContextInfo;
        }

        public struct FuncParam
        {
            public FuncParam(string InType, string InName)
            {
                Typecode = InType;
                Name = InName;
                Default = "";
            }

            public string Typecode;

            public string Name;

            public string Default;

        }

        /// <summary>
        /// Context Info of the cpp. 
        /// Most of time, it will be the MethodInfo paired with the function.
        /// Sometimes may be the TypeInfo when describing a ctor/dtor or other special functions.
        /// </summary>
        public Info ContextInfo { get; }

        /// <summary>
        /// Name of the function
        /// </summary>
        public string FuncName { get; set; } = "";

        /// <summary>
        /// Return typecode of the function
        /// </summary>
        public string FuncReturn { get; set; } = "";

        /// <summary>
        /// Parameters of the function
        /// </summary>
        public List<FuncParam> FuncParams { get; } = new List<FuncParam>();

        /// <summary>
        /// Codes translated from the function body.
        /// </summary>
        public List<string> FuncBodyCodes { get; } = new List<string>();

        /// <summary>
        /// Is function constant?
        /// </summary>
        public bool FuncConst { get; internal set; }


        /// <summary>
        /// Temporary variables used in code-translating
        /// </summary>
        List<FuncParam> TempVars { get; } = new List<FuncParam>();

        /// <summary>
        /// Try register a temporary variable.
        /// </summary>
        public FuncParam TryRegTempVar(string InTypeCode)
        {
            FuncParam registeredTmpVar = new FuncParam()
            {
                Typecode = "FAny",
                Name = $"___TMPV_{TempVars.Count}_",
            };

            TempVars.Add(registeredTmpVar);
            return registeredTmpVar;
        }

        /// <summary>
        /// Try register a temporary variable.
        /// </summary>
        public FuncParam TryRegTempVar(string InVarName, string InTypeCode, string InDefault = "")
        {
            FuncParam registeredTmpVar = new FuncParam()
            {
                Typecode = InTypeCode,
                Name = InVarName,
                Default = InDefault,
            };

            TempVars.Add(registeredTmpVar);
            return registeredTmpVar;
        }

    }

}
