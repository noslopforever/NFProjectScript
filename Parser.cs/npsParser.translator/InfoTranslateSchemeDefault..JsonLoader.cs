using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using nf.protoscript.translator.SchemeSelectors;
using System;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace nf.protoscript.translator.DefaultScheme
{

    /// <summary>
    /// Load scheme from json string.
    /// </summary>
    public static class SchemeJsonLoader
    {

        /// <summary>
        /// Scheme data to serialize
        /// </summary>
        struct SerializeData
        {
            /// <summary>
            /// Name of the scheme.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Parameters of the scheme.
            /// </summary>
            public string[] Params { get; set; }

            /// <summary>
            /// Condition to trigger this scheme.
            /// </summary>
            public string Condition { get; set; }

            /// <summary>
            /// Code of the scheme.
            /// </summary>
            public string Code { get; set; }
        }

        public static string GenerateDefaultSerializeData()
        {
            SerializeData defaultData = new SerializeData();
            defaultData.Name = "Name";
            defaultData.Params = new string[1] { "Param0" };
            defaultData.Condition = "${Condition}";
            defaultData.Code = "${Code}";
            string json = JsonSerializer.Serialize(defaultData);
            return json;
        }

        public static void LoadSchemeFromJson(InfoTranslatorDefault InTranslator, string InJsonCode)
        {
            var data = JsonSerializer.Deserialize<SerializeData>(InJsonCode);
            LoadSchemeFromData(InTranslator, data);
        }

        public static void LoadSchemes(InfoTranslatorDefault InTranslator, string InCodes)
        {
            var dataList = JsonSerializer.Deserialize<SerializeData[]>(InCodes);
            foreach (var data in dataList)
            {
                LoadSchemeFromData(InTranslator, data);
            }
        }

        static void LoadSchemeFromData(InfoTranslatorDefault InTranslator, SerializeData InData)
        {
            var elemArray = ElementParser.ParseElements(InData.Code);
            var scheme = new InfoTranslateSchemeDefault(InData.Params, elemArray);

            var selector = new TranslateSchemeSelector_Expr(1, InData.Condition, scheme);
            InTranslator.AddSelector(InData.Name, selector);
        }

    }

}