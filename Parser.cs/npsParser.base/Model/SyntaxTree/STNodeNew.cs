using System;
using System.Collections.Generic;

namespace nf.protoscript.syntaxtree
{

    // TODO deprecated: New should be taken as a special call.
    /// <summary>
    /// Expr-node to create a object.
    /// </summary>
    public class STNodeNew
        : STNodeBase
    {
        internal STNodeNew()
        {
        }

        public STNodeNew(string InTypename)
        {
            Typename = InTypename;
            Params = new ISyntaxTreeNode[] { };
        }

        public STNodeNew(string InTypename, ISyntaxTreeNode[] InParams)
        {
            Typename = InTypename;
            Params = InParams;
        }

        public STNodeNew(TypeInfo InTypeInfo)
        {
            Type = InTypeInfo;
            Typename = InTypeInfo.Name;
            Params = new ISyntaxTreeNode[] { };
        }

        public STNodeNew(TypeInfo InTypeInfo, ISyntaxTreeNode[] InParams)
        {
            Type = InTypeInfo;
            Typename = InTypeInfo.Name;
            Params = InParams;
        }

        public override void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            for (int i = 0; i < Params.Length; i++)
            {
                var param = Params[i];
                var key = $"Param{i}";
                if (!InActionFunc(key, param)) { return; }
            }
        }

        public override TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            return Type;
        }

        /// <summary>
        /// Type to be constructed.
        /// </summary>
        [Serialization.SerializableInfo]
        public string Typename { get; private set; }

        /// <summary>
        /// Type to be constructed.
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo Type { get; private set; }

        /// <summary>
        /// Parameters
        /// </summary>
        [Serialization.SerializableInfo]
        public ISyntaxTreeNode[] Params { get; private set; }
    }

}
