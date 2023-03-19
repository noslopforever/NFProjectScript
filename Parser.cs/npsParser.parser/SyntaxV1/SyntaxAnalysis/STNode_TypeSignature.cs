using System;
using System.Collections.Generic;

namespace nf.protoscript.parser.syntax1.analysis
{
    /// <summary>
    /// Type signature which describes a element's type, like n/ str/ txt[]/ tile[,]/ object[str].
    /// </summary>
    class STNode_TypeSignature
        : syntaxtree.ISyntaxTreeNode
    {
        public STNode_TypeSignature(string InTypeCode)
        {
            TypeCode = InTypeCode;
        }

        /// <summary>
        /// Code of the type.
        /// </summary>
        string TypeCode { get; }

        /// <summary>
        /// Is the type a collection type?
        /// </summary>
        public bool IsCollectionType
        {
            get
            {
                return false;
            }
        }

        // Keys for a n-D collection/array
        public IReadOnlyCollection<STNode_TypeSignature> CollectionKeys
        {
            get;
        }

        /// <summary>
        /// Is the type a delegate type?
        /// </summary>
        public bool IsDelegateType 
        {
            get 
            {
                return false;
            }
        }

        // Parameters for a delegate type
        public IReadOnlyCollection<STNode_TypeSignature> DelegateTypes
        {
            get;
        }

        /// <summary>
        /// Find TypeInfo in the context.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InParentInfo"></param>
        /// <returns></returns>
        public TypeInfo LocateTypeInfo(ProjectInfo InProjectInfo, Info InParentInfo)
        {
            // Try find type in project.
            TypeInfo typeInProj = InProjectInfo.FindTheFirstSubInfoWithName<TypeInfo>(TypeCode);

            // TODO Try find types in the InProjectInfo's related projects.
            //throw new NotImplementedException();

            // Find type in all root packages.
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

    }

}