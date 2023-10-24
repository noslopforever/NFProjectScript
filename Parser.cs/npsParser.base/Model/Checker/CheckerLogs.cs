using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nf.protoscript.modelchecker
{


    /// <summary>
    /// Logger for model check only
    /// </summary>
    internal class ModelCheckLogger
    {
        internal static ILogger Instance { get; } = new LoggerDefault();

        /// <summary>
        /// Error 1001: 
        /// </summary>
        /// <example>
        /// model ModelBase
        ///     - Property # Warning: the type of the Property should be integer.
        /// 
        /// ModelBase ModelDerived1
        ///     -n Property = 100
        /// 
        /// </example>
        internal static ILogTemplate Warning_UnknownElementType_DecidedByOverrides { get; }
            = new LogTemplateDefault(ELoggerType.Warning, "ModelCheck", 1001, "{0}'s type should be {1} which is the common base type of override elements."
                , ("Element", typeof(ElementInfo))
                , ("Type", typeof(TypeInfo))
                );
        internal static ILog LogWarning_UnknownElementType_DecidedByOverrides(ElementInfo InElementInfo, TypeInfo InTypeInfo)
        {
            var logSource = LogHelper.ExactLogSourceFromInfo(InElementInfo);

            return new LogDefault(Warning_UnknownElementType_DecidedByOverrides, logSource
                , InElementInfo
                , InTypeInfo
                );
        }


        /// <summary>
        /// Error 1002: 
        /// </summary>
        /// <example>
        /// model ModelBase
        ///     - Property
        /// 
        /// ModelBase ModelDerived1
        ///     -n Property = 100
        /// 
        /// ModelBase ModelDerived2
        ///     -float Property = 100
        /// 
        /// </example>
        internal static ILogTemplate Error_InvalidCommonBaseOfOverrideElements { get; }
            = new LogTemplateDefault(ELoggerType.Error, "ModelCheck", 1002, "{0}: Cannot exact a valid common base type from its children"
                , ("Element", typeof(ElementInfo))
                );
        internal static ILog LogError_InvalidCommonBaseOfOverrideElements(ElementInfo InElementInfo)
        {
            var logSource = LogHelper.ExactLogSourceFromInfo(InElementInfo);

            return new LogDefault(Error_InvalidCommonBaseOfOverrideElements, logSource
                , InElementInfo
                );
        }


        /// <summary>
        /// Error 1003: 
        /// </summary>
        /// <example>
        /// model ModelBase
        ///     -s Property = "a string"
        /// 
        /// ModelBase ModelDerived
        ///     -n Property = 100
        /// 
        /// </example>
        internal static ILogTemplate Error_ElementTypeConflicts { get; }
            = new LogTemplateDefault(ELoggerType.Error, "ModelCheck", 1003, "The element({0})'s type {1} conflicts with the type({2} from {3}) it overrode from."
                , ("ChildElement", typeof(ElementInfo))
                , ("ChildType", typeof(TypeInfo))
                , ("ParentElement", typeof(ElementInfo))
                , ("ParentType", typeof(TypeInfo))
                );
        internal static ILog LogError_ElementTypeConflicts(ElementInfo InChildElement, ElementInfo InParentElement)
        {
            var logSource = LogHelper.ExactLogSourceFromInfo(InChildElement);

            return new LogDefault(Error_ElementTypeConflicts, logSource
                , InChildElement
                , InChildElement.ElementType
                , InParentElement
                , InParentElement.ElementType
                );
        }
    }



}
