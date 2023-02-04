using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Hold all Info's necessary datas which can be used to reconstruct the Info itself in the future.
    /// InfoData is Serializer-friendly.
    /// </summary>
    public sealed class InfoData
    {
        public InfoData()
        {
        }

        /// <summary>
        /// Class of the info: Type/Member/Method ...
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Info's header
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Info's Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sub-infos
        /// </summary>
        public List<InfoData> SubInfos { get; } = new List<InfoData>();

        /// <summary>
        /// Append datas of info: Member's InitExpr, Method's Codebody, etc...
        /// </summary>
        public ExpandoObject AppendData { get; private set; } = new ExpandoObject();

    }



    /// <summary>
    /// Type reference
    /// </summary>
    public sealed class TypeReferenceData
    {
        public TypeReferenceData()
        {
        }

        public TypeReferenceData(string InTypeFullname)
        {
            TypeFullname = InTypeFullname;
        }

        /// <summary>
        /// Path of the type: 
        /// -   Package.Type.SubType
        /// -   Or __sys__.Typename
        /// </summary>
        public string TypeFullname { get; set; }

    }


    /// <summary>
    /// Data to save values in Syntaxes
    /// </summary>
    public sealed class SyntaxData
    {
        public SyntaxData()
        {
        }

        /// <summary>
        /// Class of the syntax.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Extras of a syntax node.
        /// </summary>
        public ExpandoObject Extra { get; private set; } = new ExpandoObject();

    }


    public sealed class MethodDelegateTypeData
    {
        // TODO ...
    }


}
