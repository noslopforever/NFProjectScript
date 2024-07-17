﻿using System;
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
            public Instance(InfoTranslateSchemeDefault InScheme, InfoTranslatorAbstract InTranslator, ITranslatingContext InContext)
            {
                HostTranslator = InTranslator;
                Scheme = InScheme;
                Context = InContext;
            }
            public static Instance CreateProxyInstance(InfoTranslateSchemeDefault InScheme, IInfoTranslateSchemeInstance InProxySI)
            {
                Instance inst = new Instance(InScheme, InProxySI.HostTranslator, InProxySI.Context)
                {
                    ProxyInstance = InProxySI
                };
                return inst;
            }

            /// <summary>
            /// The Proxy SI if this instance is a proxy instance.
            /// </summary>
            public IInfoTranslateSchemeInstance ProxyInstance { get; private set; }

            // Begin IInfoTranslateSchemeInstance interfaces
            public InfoTranslatorAbstract HostTranslator { get; }
            public InfoTranslateSchemeDefault Scheme { get; }
            public ITranslatingContext Context { get; }

            public void AddParam(string InKey, IInfoTranslateSchemeInstance InParamSI)
            {
                // TODO exceptions and logs
                _params[InKey] = InParamSI;
            }

            public IReadOnlyList<string> GetParamValue(string InKey)
            {
                // TODO exceptions and logs
                return _params[InKey].GetResult();
            }

            public IReadOnlyList<string> GetResult()
            {
                return Scheme.Snippet.Apply(this);
            }
            // ~ End IInfoTranslateSchemeInstance interfaces

            Dictionary<string, IInfoTranslateSchemeInstance> _params = new Dictionary<string, IInfoTranslateSchemeInstance>();

        }

    }

}
