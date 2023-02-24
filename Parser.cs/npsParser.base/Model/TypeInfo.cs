﻿using System;
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
        public TypeInfo BaseType { get; private set; }

    }

}
