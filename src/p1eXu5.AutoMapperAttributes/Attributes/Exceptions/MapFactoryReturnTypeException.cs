using System;

namespace p1eXu5.AutoMapperAttributes.Attributes.Exceptions
{
    class MapFactoryReturnTypeException : MissingMethodException
    {
        public MapFactoryReturnTypeException()
            : base()
        { }

        public MapFactoryReturnTypeException( string? message )
            : base( message )
        { }

        public MapFactoryReturnTypeException(string? message, Exception? inner)
            :base( message, inner)
        { }

        public MapFactoryReturnTypeException(string? className, string? methodName)
            :base( className, methodName )
        { }
    }
}
