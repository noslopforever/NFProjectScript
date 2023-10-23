using System.Collections.Generic;
using System;

namespace nf.protoscript
{
    /// <summary>
    /// Default log template.
    /// </summary>
    public class LogTemplateDefault
        : ILogTemplate
    {
        public LogTemplateDefault(ELoggerType InDefaultLoggerType
            , string InGroup
            , int InLogCodeID
            , string InMessageTemplate
            , params (string, Type)[] InParamDecls
            )
        {
            DefaultLoggerType = InDefaultLoggerType;
            Group = InGroup;
            LogCodeID = InLogCodeID;
            MessageTemplate = InMessageTemplate;
            ParamDecls = InParamDecls;
        }

        // Begin ILogTemplate interfaces
        public ELoggerType DefaultLoggerType { get; }
        public string Group { get; }
        public int LogCodeID { get; }
        public string MessageTemplate { get; }
        public IList<(string, Type)> ParamDecls { get; }
        // ~ End ILogTemplate interfaces

    }


}