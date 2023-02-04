using nf.protoscript.syntaxtree;
using System;
using System.Dynamic;
using System.Collections.Generic;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// Serialization helper functions.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Convert a TypeInfo to a TypeReferenceData
        /// </summary>
        /// <param name="InTypeInfo"></param>
        /// <returns></returns>
        public static TypeReferenceData Convert(TypeInfo InTypeInfo)
        {
            string infoFullname = InfoHelper.GetFullnameOfInfo(InTypeInfo);
            return new TypeReferenceData(infoFullname);
        }

        /// <summary>
        /// Restore a TypeInfo from a TypeReferenceData.
        /// </summary>
        /// <param name="InTypeRefData"></param>
        /// <returns></returns>
        public static TypeInfo Restore(TypeReferenceData InTypeRefData)
        {
            Info foundInfo = InfoHelper.FindInfoByFullname(InTypeRefData.TypeFullname);
            return foundInfo as TypeInfo;
        }

        /// <summary>
        /// Convert SyntaxTree into SyntaxData.
        /// </summary>
        /// <param name="InSyntaxTree"></param>
        /// <returns></returns>
        public static SyntaxData Convert(ISyntaxTreeNode InSyntaxTree)
        {
            // TODO Support ISyntaxTreeNode provided by 3rd-library.

            SyntaxData targetData = new SyntaxData();

            var srcNode = InSyntaxTree as STNodeBase;
            var treeType = srcNode.GetType();
            targetData.Class = treeType.FullName;
            var nodeType = srcNode.GetType();

            // Write property values.
            var props = nodeType.GetProperties();
            foreach (var prop in props)
            {
                object val = prop.GetValue(srcNode);
                STNodeBase nodeVal = val as STNodeBase;
                if (nodeVal != null)
                {
                    // if sub-node, try convert it to SyntaxData.
                    SyntaxData subNode = Convert(nodeVal);
                    targetData.Extra.TryAdd(prop.Name, subNode);
                }
                else
                {
                    targetData.Extra.TryAdd(prop.Name, nodeVal);
                }
            }

            return targetData;
        }

        /// <summary>
        /// Restore SyntaxTree from SyntaxData.
        /// </summary>
        /// <param name="InSyntaxData"></param>
        /// <returns></returns>
        public static syntaxtree.ISyntaxTreeNode Restore(SyntaxData InSyntaxData)
        {
            // Restore node instance by SyntaxData.Class.
            Type nodeType = Type.GetType(InSyntaxData.Class);
            STNodeBase targetNode = Activator.CreateInstance(nodeType) as STNodeBase;

            // Read properties.
            var dict = (IDictionary<string, object>)InSyntaxData.Extra;
            var props = nodeType.GetProperties();
            foreach (var prop in props)
            {
                // TODO Support ISyntaxTreeNode provided by 3rd-library.
                object readVal = null;
                if (!dict.TryGetValue(prop.Name, out readVal))
                {
                    continue;
                }

                // If property is a STNode
                if (prop.PropertyType == typeof(STNodeBase) || prop.PropertyType.IsSubclassOf(typeof(STNodeBase)))
                {
                    // The saved value must be a SyntaxData.
                    SyntaxData synVal = readVal as SyntaxData;
                    if (synVal != null)
                    {
                        // try restore it as a STNodeBase.
                        object nodeVal = Restore(synVal) as object;
                        prop.SetValue(targetNode, nodeVal);
                    }
                    else
                    {
                        throw new InvalidCastException();
                    }
                }
                else
                {
                    // common properties.
                    prop.SetValue(targetNode, readVal);
                }
            }

            return targetNode;
        }

        //public static MethodDelegateTypeData Convert(DelegateTypeInfo InDelegateInfo)
        //{
        //    throw new NotImplementedException();
        //}

        //public static DelegateTypeInfo Restore()
        //{
        //    throw new NotImplementedException();
        //}

    }


}
