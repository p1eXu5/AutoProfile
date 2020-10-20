using System;
using System.Reflection;
using AutoMapper;
using p1eXu5.AutoMapperAttributes.Contracts;

namespace p1eXu5.AutoMapperAttributes.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true, Inherited = false )]
    public class MapFromAttribute : MapAttribute
    {
        public MapFromAttribute( Type source )
        {
            Source = source;
        }

        public Type Source { get; }

        public override MemberList MemberList { get; set; } = MemberList.Destination;

        protected override Type DestinationType { get; set; } = default!;
        protected override Type SourceType
        {
            get => Source;
            set => throw new InvalidOperationException( $"Cannot set {nameof(DestinationType)} in the {nameof(MapToAttribute)}" );
        }

        protected override void SetType( Type type )
        {
            DestinationType = type;
        }

        protected override IMappingExpression CreateDefaultMap< TProfile >( TProfile profile, Type type ) 
        {
            var expr = profile.Instance.CreateMap( Source, type, MemberList );

            foreach ( var pair in FindOpposites( type, BindingFlags.SetProperty, Source, pi => pi.GetMethod != null ) ) {
                expr.ForMember( pair.pi.Name, opt => opt.MapFrom( ( s, d ) => pair.opi.GetMethod!.Invoke( s, null ) ) );
            }

            return expr;
        }

        protected override IMappingExpression CreateDefaultReverseMap< TProfile >( TProfile profile, Type type, IMappingExpression expr )
        {
            expr = expr.ReverseMap();

            var propertyPairs = FindOpposites( type, BindingFlags.GetProperty, Source, pi => pi.SetMethod != null );

            foreach ( var pair in propertyPairs ) {
                expr.ForMember( pair.opi.Name , opt => opt.MapFrom( ( s, d ) => pair.pi.GetMethod!.Invoke( s, null ) ) );
            }

            return expr;
        }
    }
}
