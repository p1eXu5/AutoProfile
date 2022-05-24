using System;
using System.Reflection;
using AutoMapper;

namespace p1eXu5.AutoProfile.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true, Inherited = false )]
    public class MapToAttribute: MapAttribute
    {
        #region ctor
        public MapToAttribute(Type target)
        {
            Target = target;
        }

        #endregion ----------------------------------------------------- ctor


        #region properties
        public Type Target { get; }

        #endregion ----------------------------------------------------- properties


        #region methods
        public override MemberList MemberList { get; set; } = MemberList.Source;
        protected override Type SourceType { get; set; } = default!;
        protected override Type DestinationType
        {
            get => Target;
            set => throw new InvalidOperationException($"Cannot set {nameof(DestinationType)} in the {nameof(MapToAttribute)}");
        }

        protected override void SetType(Type type)
        {
            SourceType = type;
        }

        protected override IMappingExpression CreateDefaultMap<TProfile>(TProfile profile)
        {
            var expr = profile.Instance.CreateMap(SourceType, DestinationType, MemberList);

            var propertyPairs = FindOpposites(SourceType, BindingFlags.GetProperty, DestinationType, pi => pi.SetMethod != null);

            if (MemberList.Destination == MemberList) 
            {
                foreach (var pair in propertyPairs)
                {
                    expr.ForMember(pair.opi.Name, opt => opt.MapFrom((s, d) => pair.pi.GetMethod!.Invoke(s, null)));
                }
            }
            else 
            {
                foreach (var pair in propertyPairs) {
                    expr.ForMember(pair.opi.Name, opt => opt.MapFrom((s, d) => pair.pi.GetMethod!.Invoke(s, null)));
                    expr.ForSourceMember(pair.pi.Name, opt => opt.DoNotValidate());
                }
            }

            return expr;
        }

        protected override IMappingExpression CreateDefaultReverseMap<TProfile>(TProfile profile, Type type, IMappingExpression expr)
        {
            expr = expr.ReverseMap();

            var propertyPairs = FindOpposites(type, BindingFlags.GetProperty, Target, pi => pi.SetMethod != null);

            foreach (var pair in propertyPairs)
            {
                expr.ForMember(pair.pi.Name, opt => opt.MapFrom((s, d) => pair.opi.GetMethod!.Invoke(s, null)));
            }

            return expr;
        }

        #endregion ----------------------------------------------------- methods
    }
}
