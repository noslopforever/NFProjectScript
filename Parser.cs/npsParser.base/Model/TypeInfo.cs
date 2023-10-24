using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript
{

    /// <summary>
    /// Declare a type (like class/structure) which describes a conception and needs to be instantialized when using.
    /// </summary>
    public class TypeInfo : Info
    {
        public TypeInfo(Info InParentInfo, string InHeader, string InName)
            : base(InParentInfo, InHeader, InName)
        {
            BaseType = null;
        }

        public TypeInfo(Info InParentInfo, string InHeader, string InName, TypeInfo InBaseType)
            : base(InParentInfo, InHeader, InName)
        {
            BaseType = InBaseType;
        }

        /// <summary>
        /// Base types of the type.
        /// </summary>
        [Serialization.SerializableInfo]
        public TypeInfo BaseType { get; protected set; }

        /// <summary>
        /// Set Base Type internally.
        /// </summary>
        /// <param name="InBaseType"></param>
        public void __Internal_SetBaseType(TypeInfo InBaseType)
        {
            BaseType = InBaseType;
        }

        /// <summary>
        /// Evaluate element type of collection.
        /// </summary>
        /// <param name="paramTypes"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TypeInfo EvalCollectionElementType(TypeInfo[] InParamTypes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if the current type is same of or derived from the InElementType.
        /// </summary>
        /// <param name="InElementType"></param>
        /// <returns></returns>
        public virtual bool IsSameOrDerivedOf(TypeInfo InElementType)
        {
            // If any Base of the current Type matches the InElementType, return true, else return false.
            var checkingType = this;
            while(checkingType != null)
            {
                if (checkingType == InElementType)
                {
                    return true;
                }
                checkingType = checkingType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Predict the common base type from InTypes.
        /// </summary>
        /// <param name="InTypes"></param>
        /// <returns>May be null, which means there is no valid common base type of all InTypes/</returns>
        public static TypeInfo PredictCommonBaseTypeFromTypes(IEnumerable<TypeInfo> InTypes)
        {
            bool onlyOnce = false;
            TypeInfo checkingBaseType = null;
            foreach (var type in InTypes)
            {
                // Do once
                if (!onlyOnce)
                {
                    checkingBaseType = type;
                    onlyOnce = true;
                    continue;
                }

                // If the checking type is not a common-base of another type, try select a new common-base from upper levels.
                // e.g. B0 <- B1, B1 <- B10, B1 <- B11, checking: B10,
                // Because the B11 is not derived from B10, so we must select B1 (B10's base) as the new checking type.
                while (checkingBaseType != null)
                {
                    if (checkingBaseType.IsSameOrDerivedOf(type))
                    {
                        break;
                    }
                    checkingBaseType = checkingBaseType.BaseType;
                }
            }

            return checkingBaseType;
        }

    }

}
