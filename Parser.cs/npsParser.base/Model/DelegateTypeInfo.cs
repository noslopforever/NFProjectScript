using System;
using System.Collections.Generic;

namespace nf.protoscript
{
    /// <summary>
    /// Infos to describe a delegate type (or function type).
    /// -   Contains Input and Output parameters. (Sub MemberTypes of this type)
    /// </summary>
    public class DelegateTypeInfo : TypeInfo
    {
        public DelegateTypeInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InReturnType)
            : base(InParentInfo, InHeader, InName)
        {
            ReturnType = InReturnType;
        }

        /// <summary>
        /// Return type of the function (delegate)
        /// </summary>
        public TypeInfo ReturnType { get; private set; }

        /// <summary>
        /// Check if the delegate is same or compatible with the InElementType.
        /// </summary>
        /// <param name="InElementType"></param>
        /// <returns></returns>
        public override bool IsSameOrDerivedOf(TypeInfo InElementType)
        {
            if (InElementType is DelegateTypeInfo)
            {
                var diff = CheckDifferent(this, InElementType as DelegateTypeInfo);

                // TODO Support delegate Derivation rulers: M1(x) should be considered as a derived signature of M0(x, y)
                //throw new NotImplementedException();

                return diff.IsSame;
            }
            return false;
        }

        /// <summary>
        /// Show human-readable text of the delegate type.
        /// SignatureName(Params): ReturnType
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string typeName = base.ToString();

            // Gather params
            string paramsStr = "";
            ForeachSubInfoByHeader<ElementInfo>("param", param =>
            {
                paramsStr += $"{param.Name}, ";
            });
            // Try removing the last ', '
            if (paramsStr.Length > 2)
            {
                paramsStr = paramsStr.Remove(paramsStr.Length - 2);
            }

            // Gather return string.
            string retStr = "";
            if (ReturnType != null)
            {
                retStr = $": {ReturnType}";
            }

            return $"{typeName}({paramsStr}){retStr}";
        }


        /// <summary>
        /// Difference between delegates.
        /// </summary>
        public class DelegateDifferent
        {
            internal DelegateDifferent()
            {
            }
            internal DelegateDifferent(bool InIsSame)
            {
                IsSame = InIsSame;
            }

            /// <summary>
            /// The two delegates are the same.
            /// </summary>
            public bool IsSame { get; } = false;

        }

        enum EParamCheckResult
        {
            Equal,
            ADeriveB,
            BDeriveA,
            InOneTree,
            NoRelation,
        }

        /// <summary>
        /// Check if the delegates are the same.
        /// </summary>
        /// <param name="InA"></param>
        /// <param name="InB"></param>
        /// <returns>Different between the two signatures.</returns>
        public static DelegateDifferent CheckDifferent(DelegateTypeInfo InA, DelegateTypeInfo InB)
        {
            // Check result type
            EParamCheckResult result = EParamCheckResult.NoRelation;
            if (InA.ReturnType == InB.ReturnType)
            {
                result = EParamCheckResult.Equal;
            }
            else
            {
                // Check if one return type is derived from another.
                if (InB.ReturnType.IsSameOrDerivedOf(InA.ReturnType))
                {
                    result = EParamCheckResult.BDeriveA;
                }
                else if (InA.ReturnType.IsSameOrDerivedOf(InB.ReturnType))
                {
                    result = EParamCheckResult.ADeriveB;
                }
                // If not, Check if both return types derive from a common base type.
                else
                {
                    var commonBase = PredictCommonBaseTypeFromTypes(new TypeInfo[] { InA, InB });
                    if (commonBase != null)
                    {
                        result = EParamCheckResult.InOneTree;
                    }
                    else
                    {
                        result = EParamCheckResult.NoRelation;
                    }
                }
            }

            // Check parameters.
            IReadOnlyList<ElementInfo> paramsOfA = InA.GetParameters();
            IReadOnlyList<ElementInfo> paramsOfB = InB.GetParameters();

            // TODO Record various situitions like "SameName-DiffIndex", "SameIndex-DiffName", "SameName-SameIndex-DiffType"
            //throw new NotImplementedException();
            bool paramsDiff = false;
            if (paramsOfA.Count == paramsOfB.Count)
            {
                for (int i = 0; i < paramsOfA.Count; i++)
                {
                    var checkA = paramsOfA[i];  
                    var checkB = paramsOfB[i];
                    if (checkA.Name != checkB.Name
                        || checkA.ElementType != checkB.ElementType
                        )
                    {
                        paramsDiff = true;
                    }
                }
            }

            return new DelegateDifferent(result == EParamCheckResult.Equal && !paramsDiff);
        }

        public IReadOnlyList<ElementInfo> GetParameters()
        {
            List<ElementInfo> paramsList = new List<ElementInfo>();
            ForeachSubInfo<ElementInfo>(paramsList.Add);
            return paramsList;
        }


    }


}
