namespace nf.protoscript.test
{
    /// <summary>
    /// Helper for generating JS functions.
    /// </summary>
    class JsFunction
    {
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
    }



}
