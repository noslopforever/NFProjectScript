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
        /// Represents an instance of the default translation scheme.
        /// </summary>
        public class Instance
            : IInfoTranslateSchemeInstance
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Instance"/> class.
            /// </summary>
            /// <param name="InScheme">The scheme associated with this instance.</param>
            /// <param name="InTranslator">The translator that will use this instance.</param>
            /// <param name="InContext">The context in which the translation will occur.</param>
            /// <param name="InParams">External parameters passed to the instance.</param>
            public Instance(InfoTranslateSchemeDefault InScheme, InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, object[] InParams)
            {
                HostTranslator = InTranslator;
                Scheme = InScheme;
                Context = InContext;
                ExtParams = InParams;
            }

            // Begin IInfoTranslateSchemeInstance interfaces


            /// <see cref="IInfoTranslateSchemeInstance.HostTranslator"/>
            public InfoTranslatorAbstract HostTranslator { get; }

            /// <see cref="IInfoTranslateSchemeInstance.Scheme"/>
            public InfoTranslateSchemeDefault Scheme { get; }

            /// <see cref="IInfoTranslateSchemeInstance.Context"/>
            public ITranslatingContext Context { get; }

            /// <see cref="IInfoTranslateSchemeInstance.ExtParams"/>
            public object[] ExtParams { get; }

            /// <see cref="IInfoTranslateSchemeInstance.GetResult"/>
            public IReadOnlyList<string> GetResult()
            {
                return Scheme.Apply(this);
            }

            /// <see cref="IInfoTranslateSchemeInstance.TryGetParamValue"/>
            public bool TryGetParamValue(string InName, out object OutValue)
            {
                OutValue = null;

                int paramIndex = Scheme.GetParamIndex(InName);
                if (paramIndex < 0 || paramIndex >= ExtParams.Length)
                {
                    return false;
                }
                OutValue = ExtParams[paramIndex];
                return true;
            }

            // ~ End IInfoTranslateSchemeInstance interfaces

            /// <summary>
            /// Stores additional scheme instances by their names.
            /// </summary>
            private Dictionary<string, IInfoTranslateSchemeInstance> _params = new Dictionary<string, IInfoTranslateSchemeInstance>();

        }

    }

}