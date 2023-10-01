using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    public partial class InfoTranslateSchemeDefault
    {

        /// <summary>
        /// Default scheme instance.
        /// </summary>
        public class Instance
            : IInfoTranslateSchemeInstance
        {
            public Instance(InfoTranslatorAbstract InTranslator, InfoTranslateSchemeDefault InScheme, Info InContextInfo)
            {
                HostTranslator = InTranslator;
                Scheme = InScheme;
                ContextInfo = InContextInfo;
            }

            // Begin IInfoTranslateSchemeInstance interfaces
            public InfoTranslatorAbstract HostTranslator { get; }
            public InfoTranslateSchemeDefault Scheme { get; }
            public Info ContextInfo { get; }

            public IReadOnlyList<string> GetResult()
            {
                return Scheme.Snippet.Apply(this);
            }

            public string GetContextVarValueString(string InContextVarName)
            {
                try
                {
                    var prop = ContextInfo.GetType().GetProperty(InContextVarName);
                    if (prop != null)
                    {
                        return prop.GetValue(ContextInfo).ToString();
                    }
                }
                catch
                {
                    // TODO log error.
                }

                return "<<Invalid Context Var>>";
            }
            // ~ End IInfoTranslateSchemeInstance interfaces

        }

    }

}
