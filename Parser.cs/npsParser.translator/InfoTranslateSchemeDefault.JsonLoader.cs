using System.Text.Json;

namespace nf.protoscript.translator.DefaultScheme
{
    /// <summary>
    /// Provides functionality to load translation schemes from JSON strings.
    /// </summary>
    public static class SchemeJsonLoader
    {
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
            SerializeData.LoadSchemeFromData(InTranslator, data);
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
                SerializeData.LoadSchemeFromData(InTranslator, data);
            }
        }

    }
}