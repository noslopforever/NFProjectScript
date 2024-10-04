using nf.protoscript.syntaxtree;
using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.nps1
{
    /// <summary>
    /// Represents a type signature that describes an element's type, such as n, str, txt[], tile[,], or dictinoary{ key0: value0, key1:value1 ].
    /// </summary>
    [VirtualSTNode]
    class STNode_TypeSignature
        : ISyntaxTreeNode
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="STNode_TypeSignature"/> class with the specified type code.
        /// </summary>
        /// <param name="InTypeCode">The code representing the type.</param>
        public STNode_TypeSignature(string InTypeCode)
        {
            TypeCode = InTypeCode;
        }

        /// <summary>
        /// Gets the code representing the type.
        /// </summary>
        string TypeCode { get; }

        /// <summary>
        /// Gets a value indicating whether the type is a collection type (e.g., array, list).
        /// </summary>
        public bool IsCollectionType
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the keys for an n-dimensional collection or array.
        /// </summary>
        public IReadOnlyCollection<STNode_TypeSignature> CollectionKeys
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the type is a delegate type.
        /// </summary>
        public bool IsDelegateType 
        {
            get 
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the parameter types for a delegate type.
        /// </summary>
        public IReadOnlyCollection<STNode_TypeSignature> DelegateTypes
        {
            get;
        }

        /// <summary>
        /// Finds the <see cref="TypeInfo"/> in the given context.
        /// </summary>
        /// <param name="InProjectInfo">The project information.</param>
        /// <param name="InParentInfo">The parent information.</param>
        /// <returns>The located <see cref="TypeInfo"/>.</returns>
        public TypeInfo LocateTypeInfo(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            // Try to find the type in the project.
            TypeInfo typeInProj = InProjectInfo.FindTheFirstSubInfoWithName<TypeInfo>(TypeCode);
            if (typeInProj != null)
            {
                return typeInProj;
            }

            // TODO: Try to find types in the related projects of InProjectInfo.
            // throw new NotImplementedException();

            // Find the type in all root packages.
            TypeInfo typeInPak = CommonTypeInfos.Unknown;
            Info.Root.ForeachSubInfo<Info>(rootPackage =>
            {
                var typeFinding = rootPackage.FindTheFirstSubInfoWithName<TypeInfo>(TypeCode);
                if (typeFinding != null)
                {
                    typeInPak = typeFinding;
                    return false;
                }
                return true;
            });

            return typeInPak;
        }

        /// <inheritdoc />
        public void ForeachSubNodes(Func<string, ISyntaxTreeNode, bool> InActionFunc)
        {
            // No sub-nodes to iterate over.
        }

        /// <inheritdoc />
        public TypeInfo GetPredictType(ElementInfo InHostElemInfo)
        {
            throw new InvalidProgramException();
        }
    }

}