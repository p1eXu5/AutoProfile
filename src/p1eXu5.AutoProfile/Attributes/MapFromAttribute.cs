using System;
using System.Reflection;
using AutoMapper;

namespace p1eXu5.AutoProfile.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true, Inherited = false )]
    public class MapFromAttribute : MapAttribute
    {
        #region ctor

        public MapFromAttribute(Type source)
        {
            Source = source;
        }

        #endregion ----------------------------------------------------- ctor


        #region properties 

        public Type Source { get; }

        #endregion ----------------------------------------------------- properties


        #region methods
        public override MemberList MemberList { get; set; } = MemberList.Destination;
        protected override Type DestinationType { get; set; } = default!;
        protected override Type SourceType
        {
            get => Source;
            set => throw new InvalidOperationException($"Cannot set {nameof(DestinationType)} in the {nameof(MapToAttribute)}");
        }
        protected override void SetType(Type type)
        {
            DestinationType = type;
        }
        protected override IMappingExpression CreateDefaultMap<TProfile>(TProfile profile)
        {
            var expr = profile.Instance.CreateMap(SourceType, DestinationType, MemberList);

            var propertyPairs = FindOpposites(DestinationType, BindingFlags.SetProperty, SourceType, pi => pi.GetMethod != null);


            if (MemberList.Destination == MemberList) 
            {
                foreach (var pair in propertyPairs)
                {
                    expr.ForMember(pair.pi.Name, opt => opt.MapFrom((s, d) => pair.opi.GetMethod!.Invoke(s, null)));
                }
            }
            else {
                foreach (var pair in propertyPairs) 
                {
                    expr.ForMember(pair.pi.Name, opt => opt.MapFrom((s, d) => pair.opi.GetMethod!.Invoke(s, null)));
                    expr.ForSourceMember(pair.opi.Name, opt => opt.DoNotValidate());
                }
            }

            return expr;
        }
        protected override IMappingExpression CreateDefaultReverseMap<TProfile>(TProfile profile, Type type, IMappingExpression expr)
        {
            expr = expr.ReverseMap();

            var propertyPairs = FindOpposites(type, BindingFlags.GetProperty, Source, pi => pi.SetMethod != null);

            foreach (var pair in propertyPairs)
            {
                expr.ForMember(pair.opi.Name, opt => opt.MapFrom((s, d) => pair.pi.GetMethod!.Invoke(s, null)));
            }

            return expr;
        }

        #endregion ----------------------------------------------------- methods
    }
}
