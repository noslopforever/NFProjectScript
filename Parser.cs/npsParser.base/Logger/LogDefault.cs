using System;

namespace nf.protoscript
{

    /// <summary>
    /// Default log.
    /// </summary>
    public class LogDefault
        : ILog
    {

        public LogDefault(ILogTemplate InTemplate, ILogSource InLogSource, params object[] InAppendData)
        {
            Template = InTemplate;
            LogSource = InLogSource;
            Appends = InAppendData;
        }

        // Begin ILog interfaces
        public ILogTemplate Template { get; }

        public ELoggerType LoggerType
        {
            get
            {
                if (Template != null)
                {
                    return Template.DefaultLoggerType;
                }
                throw new NotImplementedException();
            }
        }

        public int LogCodeID
        {
            get
            {
                if (Template != null)
                {
                    return Template.LogCodeID;
                }
                throw new NotImplementedException();
            }
        }

        public string Group
        {
            get
            {
                if (Template != null)
                {
                    return Template.Group;
                }
                throw new NotImplementedException();
            }
        }

        public ILogSource LogSource { get; }

        public string Message
        {
            get
            {
                if (Template != null)
                {
                    return string.Format(Template.MessageTemplate, Appends);
                }
                throw new NotImplementedException();
            }
        }

        public object[] Appends { get; }
        // ~ End ILog interfaces

    }


}