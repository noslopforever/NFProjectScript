using System;
using System.Collections.Generic;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Code-line.
    /// </summary>
    public class CodeLine
    {
        public CodeLine(int InIndex, string InContent)
        {
            LineNumber = InIndex;
            Content = InContent;
        }

        /// <summary>
        /// Current line's row index.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// Current line's code content.
        /// </summary>
        public string Content { get; }

    }


    /// <summary>
    /// Code content reader.
    /// </summary>
    public interface ICodeContentReader
    {

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