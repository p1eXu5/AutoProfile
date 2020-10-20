using System;

namespace p1eXu5.AutoMapperAttributes.Attributes
{
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = true, Inherited = false )]
    public class OppositeAttribute : Attribute
    {
        public OppositeAttribute( string property )
        {
            Property = property;
        }

        public string Property { get; }
    }
}
