using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace nf.protoscript.modelchecker
{

    /// <summary>
    /// Organize all elements overrided by elements of their derived-types.
    /// </summary>
    public class ElementOverrideChecker
    {
        public ElementOverrideChecker()
        {
        }

        /// <summary>
        /// Node of the tree.
        /// </summary>
        public sealed class TreeNode
        {
            internal TreeNode(TreeNode InParentTreeNode, ElementInfo InElementInfo)
            {
                ParentNode = InParentTreeNode;
                Element = InElementInfo;
                if (ParentNode != null)
                {
                    ParentNode._subNodes.Add(this);

                    // Ensure the override info is saved in ElementInfo
                    Element.OverrideElement = InParentTreeNode.Element.OverrideElement;
                }
            }

            internal static TreeNode NewRoot(ElementInfo InElementInfo)
            {
                return new TreeNode(null, InElementInfo);
            }

            /// <summary>
            /// Parent Node
            /// </summary>
            public TreeNode ParentNode { get; }

            /// <summary>
            /// Element of the tree node.
            /// </summary>
            public ElementInfo Element { get; }

            public int Depth
            {
                get
                {
                    int depth = 0;
                    var checkingParent = ParentNode;
                    while (checkingParent != null)
                    {
                        depth++;
                        checkingParent = checkingParent.ParentNode;
                    }
                    return depth;
                }
            }

            /// <summary>
            /// Sub nodes of this node.
            /// </summary>
            public List<TreeNode> Children { get { return _subNodes; } }

            /// <summary>
            /// Iterate all children recursively.
            /// </summary>
            /// <param name="InAct"></param>
            public void IterateRecursively(Action<ElementInfo /*InParent*/, ElementInfo /*InThis*/, int> InAct)
            {
                var parentElem = ParentNode != null ? ParentNode.Element : null;
                InAct(parentElem, Element, Depth);

                // recursively iterate children.
                foreach (var node in Children)
                {
                    node.IterateRecursively(InAct);
                }
            }

            List<TreeNode> _subNodes = new List<TreeNode>();

        }


        /// <summary>
        /// Gather all override elements from a project
        /// </summary>
        /// <param name="InProjInfo"></param>
        public void GatherOverrideElementsInProject(ProjectInfo InProjInfo)
        {
            // Check the Type which the source element really belongs to.
            InProjInfo.ForeachSubInfo<Info>(info =>
            {
                if (info is TypeInfo)
                {
                    var typeInfo = (TypeInfo)info;
                    typeInfo.ForeachSubInfo<ElementInfo>(typeElem =>
                        GatherOverrideTypeElements(typeInfo, typeElem)
                        );
                }
            }
            );

        }

        /// <summary>
        /// Gather all override elements from a type.
        /// </summary>
        /// <param name="InSourceType"></param>
        /// <param name="InElemInfo"></param>
        public void GatherOverrideTypeElements(TypeInfo InSourceType, ElementInfo InElemInfo)
        {
            Debug.Assert(InElemInfo.ParentInfo == InSourceType);
            Debug.Assert(InSourceType.SubContains(InElemInfo));

            // Gather super types from derived to base.
            TypeInfo baseType = InSourceType.BaseType;
            List<TypeInfo> inheritTypesFromDerivedToBase = new List<TypeInfo>();
            while (baseType != null)
            {
                inheritTypesFromDerivedToBase.Add(baseType);
                baseType = baseType.BaseType;
            }


            // Gather all 'same name' elements from the inherit tree.
            // Use reverse-foreach to iterate override elements from root to leaf.
            List<ElementInfo> sameNameElemList = new List<ElementInfo>();
            TreeNode curParentOvrNode = null;
            for (int i = inheritTypesFromDerivedToBase.Count - 1; i >= 0; --i)
            {
                var checkType = inheritTypesFromDerivedToBase[i];

                // If found the element with the same name, register it in the 'same name' element list.
                var sameNameElem = checkType.FindTheFirstSubInfoWithName<ElementInfo>(InElemInfo.Name);
                if (sameNameElem != null)
                {
                    curParentOvrNode = _EnsureOvrNode(curParentOvrNode, sameNameElem);
                }
            }

            // Has valid ovr element, register 'this element' into the ovr tree.
            if (curParentOvrNode != null)
            {
                _EnsureOvrNode(curParentOvrNode, InElemInfo);
            }

        }

        /// <summary>
        /// Dump elements tree
        /// </summary>
        /// <param name="InOut"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Dump(TextWriter InOut)
        {
            InOut.WriteLine("<<Dump Override Elements>>");

            foreach (var root in _rootNodes)
            {
                root.IterateRecursively((parent, elem, depth) =>
                {
                    string indentStr = "";
                    for (int i = 0; i < depth; i++)
                    {
                        indentStr += "    ";
                    }
                    InOut.WriteLine($"{indentStr}Elem = {elem}, Type = {elem.ElementType}, SetType = {elem.SettedElementType}, InitType = {elem.InitSyntaxPredictType}");
                });
            }
        }

        private TreeNode _EnsureOvrNode(TreeNode InParentOvrNode, ElementInfo InOvrElem)
        {
            // No parent ovr node, try ensure root.
            if (InParentOvrNode == null)
            {
                // Try find exist first.
                var existRoot = _rootNodes.Find(n => n.Element == InOvrElem);
                if (existRoot != null)
                {
                    return existRoot;
                }

                // Not exist, new root and return.
                var newRootNode = TreeNode.NewRoot(InOvrElem);
                _rootNodes.Add(newRootNode);
                return newRootNode;
            }

            // With parent ovr node, try ensure a non-root node.

            // Try find first. 
            var existNode = InParentOvrNode.Children.Find(n => n.Element == InOvrElem);
            if (existNode != null)
            {
                return existNode;
            }

            // Not exist, new node and return.
            var newOvrNode = new TreeNode(InParentOvrNode, InOvrElem);
            return newOvrNode;
        }

        /// <summary>
        /// Root element nodes.
        /// </summary>
        List<TreeNode> _rootNodes = new List<TreeNode>();

    }




}
