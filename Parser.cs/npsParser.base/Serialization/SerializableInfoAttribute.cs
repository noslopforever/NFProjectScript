using System;
using System.Collections.Generic;
using System.Text;

namespace nf.protoscript.Serialization
{

    /// <summary>
    /// C# Attribute to describe if we need and how to serialize info's property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true)]
    public sealed class SerializableInfoAttribute
        : Attribute
    {
        public SerializableInfoAttribute()
        {
        }

    }

}