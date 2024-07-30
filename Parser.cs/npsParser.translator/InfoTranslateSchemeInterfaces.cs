using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.translator
{

    /// <summary>
    /// Represents a concrete translation scheme that provides instructions on how to translate a target object.
    /// </summary>
    public interface IInfoTranslateScheme
    {

        /// <summary>
        /// Gets the number of parameters used in this scheme.
        /// </summary>
        int ParamNum { get; }

        /// <summary>
        /// Gets the names of the parameters used in this scheme.
        /// </summary>
        string[] ParamNames { get; }

        /// <summary>
        /// Retrieves the index of a parameter by its name.
        /// </summary>
        /// <param name="InName">The name of the parameter to find.</param>
        /// <returns>The index of the parameter, or -1 if not found.</returns>
        int GetParamIndex(string InName);

        /// <summary>
        /// Creates a new instance of the scheme for a given translator, context, and parameters.
        /// </summary>
        /// <param name="InTranslator">The translator that will use this scheme.</param>
        /// <param name="InContext">The context in which the translation will occur.</param>
        /// <param name="InParams">Parameters to pass to the scheme instance.</param>
        /// <returns>A new instance of the scheme.</returns>
        IInfoTranslateSchemeInstance CreateInstance(InfoTranslatorAbstract InTranslator, ITranslatingContext InContext, params object[] InParams);

    }

    /// <summary>
    /// Represents an instance of a translation scheme bound to a specific context and translator.
    /// Each scheme can create multiple instances to handle different translating Info nodes.
    /// </summary>
    public interface IInfoTranslateSchemeInstance
    {
        /// <summary>
        /// Gets the host translator associated with this instance.
        /// </summary>
        InfoTranslatorAbstract HostTranslator { get; }

        /// <summary>
        /// Gets the scheme associated with this instance.
        /// </summary>
        IInfoTranslateScheme Scheme { get; }

        /// <summary>
        /// Gets the context in which the translation will occur.
        /// </summary>
        ITranslatingContext Context { get; }

        /// <summary>
        /// Gets the external parameters passed to the instance.
        /// </summary>
        object[] ExtParams { get; }

        /// <summary>
        /// Applies the scheme's snippet to the instance and returns the result.
        /// </summary>
        /// <returns>A read-only list of strings representing the translation result.</returns>
        IReadOnlyList<string> GetResult();

        /// <summary>
        /// Attempts to retrieve the value of a parameter by its name.
        /// </summary>
        /// <param name="InName">The name of the parameter to retrieve.</param>
        /// <param name="OutValue">The retrieved parameter value.</param>
        /// <returns><c>true</c> if the parameter exists and its value is retrieved; otherwise, <c>false</c>.</returns>
        bool TryGetParamValue(string InName, out object OutValue);

    }


    /// <summary>
    /// Defines a scheme selector responsible for choosing the most suitable translation scheme for a given translating context.
    /// </summary>
    public interface IInfoTranslateSchemeSelector
    {
        /// <summary>
        /// Gets the priority of the selector. Selectors with higher priorities run first. The default priority is 0.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Gets the scheme selected by this selector.
        /// </summary>
        IInfoTranslateScheme Scheme { get; }

        /// <summary>
        /// Determines whether the conditions for this selector are met for a given translating context.
        /// </summary>
        /// <param name="InContext">The translating context to evaluate.</param>
        /// <returns><c>true</c> if the conditions are met; otherwise, <c>false</c>.</returns>
        bool IsMatch(ITranslatingContext InContext);

    }

}
