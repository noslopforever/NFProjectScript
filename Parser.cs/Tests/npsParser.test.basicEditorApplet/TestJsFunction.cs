﻿using System.Collections.Generic;

namespace nf.protoscript.test
{
    /// <summary>
    /// Helper for generating JS functions.
    /// </summary>
    class JsFunction
    {
        public JsFunction(Info InContextInfo)
        {
            ContextInfo = InContextInfo;
        }

        /// <summary>
        /// Context Info of the cpp. 
        /// Most of time, it will be the MethodInfo paired with the function.
        /// Sometimes may be the TypeInfo when describing a ctor/dtor or other special functions.
        /// </summary>
        public Info ContextInfo { get; }

        public string Name { get; set; }
        public string[] Params { get; set; }
        public string[] BodyLines { get; set; }

        /// <summary>
        /// Merge params to "param0, param1, param2 ... "
        /// </summary>
        public string ParamsCode
        {
            get
            {
                string paramsCode = "";
                if (Params.Length > 0)
                {
                    paramsCode = Params[0];
                }
                for (int i = 1; i < Params.Length; i++)
                {
                    paramsCode += $", {Params[i]}";
                }
                return paramsCode;
            }
        }

        public string FuncDeclCode
        {
            get
            {
                return $"{Name}({ParamsCode})";
            }
        }


        /// <summary>
        /// Temporary variables used in code-translating
        /// </summary>
        List<string> TempVars { get; } = new List<string>();

        /// <summary>
        /// Try register a temporary variable.
        /// </summary>
        public string TryRegTempVar()
        {
            string registeredTmpVar = $"___TMPV_{TempVars.Count}_";
            TempVars.Add(registeredTmpVar);
            return registeredTmpVar;
        }


    }



}
