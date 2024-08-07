using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using nf.protoscript.translator.SchemeSelectors;
using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace nf.protoscript.translator.DefaultScheme
{
    /// <summary>
    /// Provides functionality to load translation schemes from YAML strings.
    /// </summary>
    public static class SchemeYamlLoader
    {
        /// <summary>
        /// Generates a default YAML string representing a serialization data structure.
        /// </summary>
        /// <returns>A YAML string containing default scheme data.</returns>
        public static string GenerateDefaultSerializeData()
        {
            SerializeData defaultData = new SerializeData
            {
                Name = "Name",
                Params = new[] { "Param0" },
                Condition = "${Condition}",
                Code = "${Code}"
            };

            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(defaultData);
            return yaml;
        }

        /// <summary>
        /// Loads a single scheme from a YAML string into the translator.
        /// </summary>
        /// <param name="InTranslator">The translator to add the scheme to.</param>
        /// <param name="InYamlCode">The YAML string containing the scheme data.</param>
        public static void LoadSchemeFromYaml(InfoTranslatorDefault InTranslator, string InYamlCode)
        {
            var deserializer = new DeserializerBuilder().Build();
            var data = deserializer.Deserialize<SerializeData>(InYamlCode);
            SerializeData.LoadSchemeFromData(InTranslator, data);
        }

        /// <summary>
        /// Loads multiple schemes from a YAML string into the translator.
        /// </summary>
        /// <param name="InTranslator">The translator to add the schemes to.</param>
        /// <param name="InCodes">The YAML string containing an array of scheme data.</param>
        public static void LoadSchemes(InfoTranslatorDefault InTranslator, string InCodes)
        {
            var deserializer = new DeserializerBuilder().Build();
            var dataList = deserializer.Deserialize<SerializeData[]>(InCodes);
            foreach (var data in dataList)
            {
                SerializeData.LoadSchemeFromData(InTranslator, data);
            }
        }


    }
}