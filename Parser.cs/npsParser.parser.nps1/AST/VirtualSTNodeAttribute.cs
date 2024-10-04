using System;

namespace nf.protoscript.parser.nps1
{

    /// <summary>
    /// Attribute used to mark a Syntax Tree Node (STNode) class as a virtual syntax tree node.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class VirtualSTNodeAttribute:Attribute
    {
    }

}