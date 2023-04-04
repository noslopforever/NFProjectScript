using System;
using System.Collections.Generic;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Code-line.
    /// </summary>
    public class CodeLine
    {
        public CodeLine(ICodeContentReader InReader, int InIndex, string InContent)
        {
            Reader = InReader;
            LineNumber = InIndex;
            Content = InContent;
        }

        /// <summary>
        /// Reader of the code.
        /// </summary>
        public ICodeContentReader Reader { get; }

        /// <summary>
        /// Current line's row index.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Current line's code content.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// The site string which indicates the place of the codeline.
        /// </summary>
        public string SiteString
        {
            get
            {
                return $"{Reader.Filename}[{LineNumber}]";
            }
        }

    }

    /// <summary>
    /// Code content reader.
    /// </summary>
    public interface ICodeContentReader
    {
        /// <summary>
        /// Name of the file readed by the reader.
        /// </summary>
        public string Filename { get; }

        /// <summary>
        /// Code line
        /// </summary>
        public CodeLine CurrentCodeLine { get; }

        /// <summary>
        /// Is EOF
        /// </summary>
        bool IsEndOfFile { get; }

        /// <summary>
        /// Step to the head of the file.
        /// </summary>
        void GoStart();

        /// <summary>
        /// Step to the next line.
        /// </summary>
        /// <returns>
        /// If reaches EOF, return false. Otherwise, return true.
        /// </returns>
        bool GoNextLine();

    }


}