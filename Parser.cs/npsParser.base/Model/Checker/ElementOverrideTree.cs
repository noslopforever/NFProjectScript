using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace nf.protoscript.modelchecker
{

    /// <summary>
    /// Node of the tree.
    /// </summary>
    public sealed class ElementOverrideTreeNode
    {
        internal ElementOverrideTreeNode(ElementOverrideTreeNode InParentTreeNode, ElementInfo InElementInfo)
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

        internal static ElementOverrideTreeNode NewRoot(ElementInfo InElementInfo)
        {
            return new ElementOverrideTreeNode(null, InElementInfo);
        }

        /// <summary>
        /// Parent Node
        /// </summary>
        public ElementOverrideTreeNode ParentNode { get; }

        /// <summary>
        /// Element of the tree node.
        /// </summary>
        public ElementInfo Element { get; }

        /// <summary>
        /// Depth of a node.
        /// </summary>
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
        public List<ElementOverrideTreeNode> Children { get { return _subNodes; } }

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

        List<ElementOverrideTreeNode> _subNodes = new List<ElementOverrideTreeNode>();

    }




    /// <summary>
    /// Organize all elements overrided by elements of their derived-types.
    /// </summary>
    public class ElementOverrideCollector
    {
        public ElementOverrideCollector()
        {
        }

        /// <summary>
        /// Root nodes of the collected override-tree. The elements in these nodes are overrided by their host type's derived-types.
        /// </summary>
        public IEnumerable<ElementOverrideTreeNode> RootNodes { get { return _rootNodes; } }

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
            ElementOverrideTreeNode curParentOvrNode = null;
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

        private ElementOverrideTreeNode _EnsureOvrNode(ElementOverrideTreeNode InParentOvrNode, ElementInfo InOvrElem)
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
                var newRootNode = ElementOverrideTreeNode.NewRoot(InOvrElem);
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
            var newOvrNode = new ElementOverrideTreeNode(InParentOvrNode, InOvrElem);
            return newOvrNode;
        }

        /// <summary>
        /// Root element nodes.
        /// </summary>
        List<ElementOverrideTreeNode> _rootNodes = new List<ElementOverrideTreeNode>();

    }



    public class ElementOverrideAnalyzer
    {

        /// <summary>
        /// Analyze one node in a override tree. Log errors in OutCheckErrors.
        /// </summary>
        /// <param name="OutCheckLogs"></param>
        /// <param name="InOvrTreeNode"></param>
        public static void AnalyzeOverrideTree(List<ILog> OutCheckLogs, ElementOverrideTreeNode InOvrTreeNode)
        {
            if (InOvrTreeNode.Children.Count == 0)
            {
                return;
            }

            // Calculate common base type of child-types.
            List<TypeInfo> childTypes = new List<TypeInfo>(InOvrTreeNode.Children.Count);
            foreach (var ovrChild in InOvrTreeNode.Children)
            {
                // Recursive and collect child types.
                AnalyzeOverrideTree(OutCheckLogs, ovrChild);

                childTypes.Add(ovrChild.Element.ElementType);
            }
            TypeInfo commonBaseType = TypeInfo.PredictCommonBaseTypeFromTypes(childTypes);

            if (commonBaseType == null)
            {
                // log error: Cannot exact a valid common base type from its children.
                OutCheckLogs.Add(ModelCheckLogger.LogError_InvalidCommonBaseOfOverrideElements(InOvrTreeNode.Element));
                return;
            }

            // ElementType 
            if (InOvrTreeNode.Element.ElementType != CommonTypeInfos.Unknown)
            {
                // Success if the parent type is super class of, or equal to the common base type of all children.
                if (commonBaseType.IsSameOrDerivedOf(InOvrTreeNode.Element.ElementType))
                {
                    return;
                }
                else
                {
                    // log error: The element's type conflicts with the type it overrode from.
                    foreach (var ovrChild in InOvrTreeNode.Children)
                    {
                        if (!ovrChild.Element.ElementType.IsSameOrDerivedOf(InOvrTreeNode.Element.ElementType))
                        {
                            OutCheckLogs.Add(ModelCheckLogger.LogError_ElementTypeConflicts(ovrChild.Element, InOvrTreeNode.Element));
                        }
                    }
                }
            }
            else
            {
                // If the checking element's type is unset, mark it can comes from the common base of all override elements.
                OutCheckLogs.Add(ModelCheckLogger.LogWarning_UnknownElementType_DecidedByOverrides(InOvrTreeNode.Element, commonBaseType));
            }
        }


    }




}
