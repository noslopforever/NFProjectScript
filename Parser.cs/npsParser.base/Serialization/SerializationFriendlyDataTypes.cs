using System;
using System.Collections.Generic;
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
        public Dictionary<string, object> AppendData { get; } = new Dictionary<string, object>();

    }



    /// <summary>
    /// Type reference
    /// </summary>
    public sealed class TypeReferenceData
    {

        /// <summary>
        /// Path of the type: 
        /// -   Package.Type.SubType
        /// -   Or __sys__.Typename
        /// </summary>
        public string PathOfTheType { get; set; }

    }


    public sealed class MethodDelegateTypeData
    {
        // TODO ...
    }

    /// <summary>
    /// Data to save values in Syntaxes
    /// </summary>
    public sealed class SyntaxData
    {
        // TODO ...
    }


}
