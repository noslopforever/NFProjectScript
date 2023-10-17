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
        public bool IsSameOrDerivedOf(TypeInfo InElementType)
        {
            // If any Base of the current Type matches the InElementType, return true, else return false.
            var checkingType = this;
            while(checkingType != null)
            {
                if (checkingType == InElementType)
                {
                    return true;
                }
            }
            return false;
        }

    }

}
