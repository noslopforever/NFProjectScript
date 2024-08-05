using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using nf.protoscript.translator.SchemeSelectors;
using System;
using System.Text.Json;

namespace nf.protoscript.translator.DefaultScheme
{
    /// <summary>
    /// Provides functionality to load translation schemes from JSON strings.
    /// </summary>
    public static class SchemeJsonLoader
    {
        /// <summary>
        /// Represents the serialized data structure for a translation scheme.
        /// </summary>
        private struct SerializeData
        {
            /// <summary>
            /// Gets or sets the name of the scheme.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the parameters required by the scheme.
            /// </summary>
            public string[] Params { get; set; }

            /// <summary>
            /// Gets or sets the condition that triggers the scheme.
            /// </summary>
            public string Condition { get; set; }

            /// <summary>
            /// Gets or sets the code that defines the scheme's behavior.
            /// </summary>
            public string Code { get; set; }
        }

        /// <summary>
        /// Generates a default JSON string representing a serialization data structure.
        /// </summary>
        /// <returns>A JSON string containing default scheme data.</returns>
        public static string GenerateDefaultSerializeData()
        {
            SerializeData defaultData = new SerializeData
            {
                Name = "Name",
                Params = new[] { "Param0" },
                Condition = "${Condition}",
                Code = "${Code}"
            };
            string json = JsonSerializer.Serialize(defaultData);
            return json;
        }

        /// <summary>
        /// Loads a single scheme from a JSON string into the translator.
        /// </summary>
        /// <param name="InTranslator">The translator to add the scheme to.</param>
        /// <param name="InJsonCode">The JSON string containing the scheme data.</param>
        public static void LoadSchemeFromJson(InfoTranslatorDefault InTranslator, string InJsonCode)
        {
            var data = JsonSerializer.Deserialize<SerializeData>(InJsonCode);
            LoadSchemeFromData(InTranslator, data);
        }

        /// <summary>
        /// Loads multiple schemes from a JSON string into the translator.
        /// </summary>
        /// <param name="InTranslator">The translator to add the schemes to.</param>
        /// <param name="InCodes">The JSON string containing an array of scheme data.</param>
        public static void LoadSchemes(InfoTranslatorDefault InTranslator, string InCodes)
        {
            var dataList = JsonSerializer.Deserialize<SerializeData[]>(InCodes);
            foreach (var data in dataList)
            {
                LoadSchemeFromData(InTranslator, data);
            }
        }

        /// <summary>
        /// Loads a scheme into the translator from serialized data.
        /// </summary>
        /// <param name="InTranslator">The translator to add the scheme to.</param>
        /// <param name="InData">The serialized data of the scheme.</param>
        private static void LoadSchemeFromData(InfoTranslatorDefault InTranslator, SerializeData InData)
        {
            var elemArray = ElementParser.ParseElements(InData.Code);
            var scheme = new InfoTranslateSchemeDefault(InData.Params, elemArray);

            var selector = new TranslateSchemeSelector_Expr(1, InData.Condition, scheme);
            InTranslator.AddSelector(InData.Name, selector);
        }
    }
}