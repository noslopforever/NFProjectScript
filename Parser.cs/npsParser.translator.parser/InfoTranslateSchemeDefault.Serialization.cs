using nf.protoscript.parser.SyntaxV1;
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
        /// The higher the priority, the earlier the scheme is applied during the translation process.
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
            // Parse the code into elements.
            var elemArray = ElementParser.ParseElements(InData.Code);

            // Create a scheme/selector by the elements/condition.
            var scheme = new InfoTranslateSchemeDefault(InData.Params, elemArray);
            if (!string.IsNullOrWhiteSpace(InData.Condition))
            {
                var conditionExpr = ExpressionParser_CommonNps.ParseExpression(InData.Condition);
                var selector = new TranslateSchemeSelector_Expr(InData.Priority, conditionExpr, scheme);

                // Add the selector to the translator.
                InTranslator.AddSelector(InData.Name, selector);
            }
            else
            {
                InTranslator.AddScheme(InData.Name, scheme);
            }
        }

    }

}