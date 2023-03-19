namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Element-Define statement (member/child-elements/globals/parameters). 
    /// </summary>
    class STNode_ElementDef
        : STNode_DefBase
    {
        public STNode_ElementDef(string InElementName)
            : base(InElementName)
        {
        }

    }

}