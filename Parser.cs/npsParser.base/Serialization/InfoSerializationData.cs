using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// A serializable info which can help auto-serialization.
    /// </summary>
    public sealed class InfoSerializationData
    {
        public InfoSerializationData()
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
        public List<InfoSerializationData> SubInfos { get; } = new List<InfoSerializationData>();

        /// <summary>
        /// Append datas of info: Member's InitExpr, Method's Codebody, etc...
        /// </summary>
        public Dictionary<string, object> AppendData { get; } = new Dictionary<string, object>();

    }


}
