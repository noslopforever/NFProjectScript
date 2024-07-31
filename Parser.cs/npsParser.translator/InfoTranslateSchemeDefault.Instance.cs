using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator.DefaultScheme
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
                _scheme = InScheme;
                Context = InContext;
                ExtParams = InParams;
            }

            // Begin IInfoTranslateSchemeInstance interfaces


            /// <inheritdoc />
            public InfoTranslatorAbstract HostTranslator { get; }

            /// <inheritdoc />
            public IInfoTranslateScheme Scheme { get { return _scheme; } }

            /// <inheritdoc />
            public ITranslatingContext Context { get; }

            /// <inheritdoc />
            public object[] ExtParams { get; }

            /// <inheritdoc />
            public IReadOnlyList<string> GetResult()
            {
                return _scheme.Apply(this);
            }

            /// <inheritdoc />
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

            /// <summary>
            /// The scheme associated with this instance.
            /// </summary>
            InfoTranslateSchemeDefault _scheme = null;

        }

    }

}