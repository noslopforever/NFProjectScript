using nf.protoscript.translator.DefaultScheme.Elements.Internal;
using nf.protoscript.translator.SchemeSelectors;
using System;

namespace nf.protoscript.translator.DefaultScheme
{


    /// <summary>
    /// Represents the serialized data structure for a translation scheme.
    /// </summary>
    public struct SerializeData
    {
        /// <summary>
        /// Gets or sets the name of the scheme.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the priority of the scheme.
        /// </summary>
        public int Priority { get; set; }

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

        /// <summary>
        /// Loads a scheme into the translator from serialized data.
        /// </summary>
        /// <param name="InTranslator">The translator to add the scheme to.</param>
        /// <param name="InData">The serialized data of the scheme.</param>
        public static void LoadSchemeFromData(InfoTranslatorDefault InTranslator, SerializeData InData)
        {
            var elemArray = ElementParser.ParseElements(InData.Code);
            var scheme = new InfoTranslateSchemeDefault(InData.Params, elemArray);

            var selector = new TranslateSchemeSelector_Expr(InData.Priority, InData.Condition, scheme);
            InTranslator.AddSelector(InData.Name, selector);
        }

    }
}