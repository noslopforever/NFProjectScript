using System;
using System.Collections.Generic;
using System.IO;

namespace nf.protoscript.parser
{

    /// <summary>
    /// Default content reader which stores all codes in memory.
    /// </summary>
    public class StringCodeContentReader
        : ICodeContentReader
    {
        /// <summary>
        /// Construct a reader by string array.
        /// </summary>
        /// <param name="InFilelines"></param>
        public StringCodeContentReader(string InFilename, string[] InFilelines)
        {
            Filename = InFilename;
            _FileLines = InFilelines;
            Goto(0);
        }

        /// <summary>
        /// Construct a reader by reading a text-file.
        /// </summary>
        /// <param name="InFlag"></param>
        /// <param name="InFilename"></param>
        public static StringCodeContentReader LoadFromFile(string InFilename)
        {
            try
            {
                var fileLns = File.ReadAllLines(InFilename);

                return new StringCodeContentReader(InFilename, fileLns);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ELoggerType.Error, "Parser"
                    , ParserErrorType.Parser_LoadFileError.UniqueID
                    , $"{ex.TargetSite} : {ParserErrorType.Parser_LoadFileError.AsciiMessage} : {ex.Message}"
                    );
            }
            return null;
        }

        /// <summary>
        /// Construct a reader by reading a string with 'NewLine' characters.
        /// </summary>
        /// <param name="InFlag"></param>
        /// <param name="InFilename"></param>
        public static StringCodeContentReader LoadFromString(string InFilename, string InString)
        {
            try
            {
                StringReader sr = new StringReader(InString);
                List<string> result = new List<string>();
                string ln = null;
                while ((ln = sr.ReadLine()) != null)
                { result.Add(ln); }

                return new StringCodeContentReader(InFilename, result.ToArray());
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ELoggerType.Error, "Parser"
                    , ParserErrorType.Parser_LoadFileError.UniqueID
                    , $"{ex.TargetSite} : {ParserErrorType.Parser_LoadFileError.AsciiMessage} : {ex.Message}"
                    );
            }
            return null;
        }

        public string Filename { get; private set; }

        public CodeLine CurrentCodeLine
        {
            get
            {
                if (_ActualLineIndex >= _FileLines.Length)
                { return null; }
                return new CodeLine(this, _ActualLineIndex + 1, _FileLines[_ActualLineIndex]);
            }
        }

        public bool IsEndOfFile
        {
            get
            {
                return _ActualLineIndex == _FileLines.Length;
            }
        }

        public bool GoNextLine()
        {
            return Goto(_ActualLineIndex + 1);
        }

        public void GoStart()
        {
            Goto(0);
        }


        /// <summary>
        /// File contents.
        /// </summary>
        string[] _FileLines = new string[0];

        /// <summary>
        /// Index to the current line.
        /// </summary>
        int _ActualLineIndex = 0;

        /// <summary>
        /// Goto the targe line.
        /// </summary>
        /// <param name="InLineIndex"></param>
        /// <returns></returns>
        bool Goto(int InLineIndex)
        {
            _ActualLineIndex = InLineIndex;
            if (_ActualLineIndex >= _FileLines.Length)
            { return false; }

            return true;
        }
    }


}