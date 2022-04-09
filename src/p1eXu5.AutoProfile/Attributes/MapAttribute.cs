using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration.Annotations;

namespace p1eXu5.AutoProfile.Attributes
{
    using AutoMapper.Configuration;
    using Contracts;

    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true, Inherited = false )]
    public abstract class MapAttribute : Attribute
    {
        #region properties 

        /// <summary>
        /// Includes all maps of this type to derives types.
        /// </summary>
        public bool IncludeAllDerived { get; set; }

        /// <summary>
        /// Includes all maps of this type to derives types.
        /// </summary>
        public bool IncludeAllDerivedForReverse { get; set; }

        /// <summary>
        /// Set true to create a reverse mapping configuration that includes unflattening.
        /// If that behavior is undesirable consider using <see cref="MapFromAttribute"/>
        /// like additional attribute to your class.
        /// <br/>
        /// For more information see <see href="http://docs.automapper.org/en/stable/Reverse-Mapping-and-Unflattening.html">here</see>
        /// </summary>
        public bool ReverseMap { get; set; }
        public Type? IncludeBase { get; set; }
        public Type? Include { get; set; }

        /// <summary>
        /// Type where map is configured.
        /// </summary>
        public Type? MapFactoryType { get; set; }
        public string? MapFactory { get; set; }
        public string? ReverseMapFactory { get; set; }
        public abstract MemberList MemberList { get; set; }
        protected abstract Type SourceType { get; set; }
        protected abstract Type DestinationType { get; set; }

        #endregion ----------------------------------------------------- properties


        #region methods
        public void CreateMap<TProfile>(TProfile profile, Type type) where TProfile : IAutoProfile
        {
            SetType(type);

            var (expr, instance, mapFactory) = CreateForwardMap(profile, type);

            if (expr == null) return;

            IgnoreProperties(type, expr);

            CreateReverseMap(profile, type, expr, instance);
        }
        protected abstract void SetType(Type type);
        protected abstract IMappingExpression CreateDefaultMap<TProfile>(TProfile profile, Type type) where TProfile : IAutoProfile;
        protected abstract IMappingExpression CreateDefaultReverseMap<TProfile>(TProfile profile, Type type, IMappingExpression expr) where TProfile : IAutoProfile;

        /// <summary>
        /// Collects opposite properties.
        /// </summary>
        /// <param name="type">Annotated <see cref="Type"/>.</param>
        /// <param name="exprType"><see cref="Type"/> of created expression.</param>
        /// <returns></returns>
        protected MethodInfo? GetReverseMapFactoryMethodInfo(Type type, Type exprType)
        {
            Func<Type, Type, bool> thereIs =
                (pt, et) =>
                    pt == et
                    || (pt.IsGenericType &&
                        et.IsGenericType &&
                        pt.GetGenericArguments()[0] == et.GetGenericArguments()[0] &&
                        pt.GetGenericArguments()[1] == et.GetGenericArguments()[1]);


            MethodInfo? resultMethodInfo = null;

            var methods =
                type.GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);


            foreach (var mi in methods)
            {
                if (!mi.Name.Equals(ReverseMapFactory)) continue;
                var parameters = mi.GetParameters();

                foreach (var parType in parameters.Select(p => p.ParameterType))
                {

                    if (thereIs(parType, exprType))
                    {
                        resultMethodInfo = mi;
                        break;
                    }
                }
            }

            return resultMethodInfo;
        }

        /// <summary>
        /// Returns pair array of a name annotated property and property info of a property in an opposite type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="flags"><see cref="BindingFlags.GetProperty"/> or <see cref="BindingFlags.SetProperty"/> for annotated type properties.</param>
        /// <param name="oppositeType">Type in which map to or map from does.</param>
        /// <param name="filter">For filter getters or setters for property in an opposite type.</param>
        /// <returns></returns>
        protected (PropertyInfo pi, PropertyInfo opi)[] FindOpposites(Type type, BindingFlags flags, Type oppositeType, Predicate<PropertyInfo> filter)
        {
            return type.GetProperties(flags | BindingFlags.Instance | BindingFlags.Public)
                .Select(pi => (pi, pi.GetCustomAttributes<OppositeAttribute>()))
                .Select(pair =>
                {

                    if (pair.Item2.Any())
                    {
                        var opi = oppositeType
                            .GetProperties()
                            .SingleOrDefault(pi => pair.Item2.Select(attr => attr.Property)
                               .Contains(pi.Name));

                        if (opi != null && filter(opi))
                        {
                            return (pair.pi, opi);
                        }
                    }

                    return (null!, null!);
                })
                .Where(r => r != (null, null))
                .ToArray();
        }

        /// <summary>
        /// Creates map.
        /// </summary>
        /// <param name="profile"><see cref="IAutoProfile"/></param>
        /// <param name="type">Annotated <see cref="Type"/>.</param>
        /// <returns></returns>
        private (object? expr, object? instance, MethodInfo? mapFactoryMethodInfo) CreateForwardMap<TProfile>(TProfile profile, Type type) where TProfile : IAutoProfile
        {
            object? expr = null;
            object? instance = null;
            MethodInfo? mapFactory = null;

            if (!String.IsNullOrWhiteSpace(MapFactory))
            {
                if (MapFactoryType != null)
                {
                    mapFactory = MapFactoryType.GetMethod(MapFactory!, BindingFlags.Static | BindingFlags.Public);
                }
                else if (!type.IsInterface)
                {
                    // if there are several methods with the same name then CS0108 compiler warning will be
                    mapFactory = type.GetMethod(MapFactory!,
                        BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
                }

                if (mapFactory != null)
                {

                    if (!type.IsInterface && MapFactoryType == null)
                    {
                        instance = Activator.CreateInstance(type);
                    }

                    expr = mapFactory.Invoke(instance, new object[] { profile });

                    if (expr != null)
                    {
                        if (IncludeAllDerived)
                        {
                            MethodInfo? mi = expr.GetType().GetMethod("IncludeAllDerived");
                            expr = mi?.Invoke(expr, null);
                        }

                        if (IncludeBase != null)
                        {
                            MethodInfo? mi =
                                expr?.GetType()
                                    .GetMethods(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public)
                                    .First(m => !m.IsGenericMethod && m.Name.Equals("IncludeBase"));

                            expr = mi?.Invoke(expr, new object[] { IncludeBase, DestinationType });
                        }
                    }

                }
            }

            if (mapFactory == null)
            {
                expr = CreateDefaultMap(profile, type);

                if (IncludeAllDerived)
                {
                    expr = ((IMappingExpression)expr).IncludeAllDerived();
                }

                if (IncludeBase != null)
                {
                    expr = ((IMappingExpression)expr).IncludeBase(IncludeBase, DestinationType);
                }
            }

            return (expr, instance, mapFactory);
        }
        private void CreateReverseMap<TProfile>(TProfile profile, Type type, object expression, object? instance) where TProfile : IAutoProfile
        {
            object? expr = expression;
            MethodInfo? reverseMapFactory = null;

            if (!String.IsNullOrWhiteSpace(ReverseMapFactory))
            {
                if (!type.IsInterface)
                {
                    // if there are several methods with the same name then CS0108 compiler warning will be
                    reverseMapFactory = GetReverseMapFactoryMethodInfo(type, expr!.GetType());
                }
                else if (MapFactoryType != null)
                {
                    reverseMapFactory = GetReverseMapFactoryMethodInfo(MapFactoryType, expr!.GetType());
                }


                if (instance == null)
                {
                    if (!type.IsInterface)
                    {
                        instance = Activator.CreateInstance(type);
                    }

                    if (instance != null && reverseMapFactory != null)
                    {
                        expr = reverseMapFactory.Invoke(instance, new object[] { profile })!;
                    }
                    else if (reverseMapFactory == null)
                    {
                        expr = CreateDefaultReverseMap(profile, type, (IMappingExpression)expr);
                    }
                    else
                    {
                        ParameterInfo[] @params = reverseMapFactory.GetParameters();
                        expr =
                            @params[0].ParameterType == typeof(IAutoProfile)
                                ? reverseMapFactory.Invoke(instance, @params.Length == 2 ? new object[] { profile, expr } : new object[] { profile })
                                : reverseMapFactory.Invoke(instance, @params.Length == 2 ? new object[] { expr, profile } : new object[] { expr });
                    }
                }
                else if (reverseMapFactory != null)
                {
                    ParameterInfo[] @params = reverseMapFactory.GetParameters();
                    expr =
                        @params[0].ParameterType == typeof(IAutoProfile)
                            ? reverseMapFactory.Invoke(instance, @params.Length == 2 ? new object[] { profile, expr } : new object[] { profile })
                            : reverseMapFactory.Invoke(instance, @params.Length == 2 ? new object[] { expr, profile } : new object[] { expr });
                }
                else
                {
                    MethodInfo? mi = expr!.GetType().GetMethod("ReverseMap");
                    expr = mi?.Invoke(expr, new object?[] { profile, type, expr });
                }

                if (IncludeAllDerivedForReverse)
                {
                    MethodInfo? mi = expr!.GetType().GetMethod("IncludeAllDerived");
                    mi?.Invoke(expr, null);
                }
            }
            else if (ReverseMap)
            {
                if (expr is IMappingExpression e)
                {
                    e = CreateDefaultReverseMap(profile, type, e);

                    if (IncludeAllDerivedForReverse)
                    {
                        e.IncludeAllDerived();
                    }
                }
                else
                {
                    MethodInfo? mi = expr!.GetType().GetMethod("ReverseMap");
                    mi?.Invoke(expr, null);

                    if (IncludeAllDerivedForReverse)
                    {
                        mi = expr!.GetType().GetMethod("IncludeAllDerived");
                        mi?.Invoke(expr, null /*new object?[] { profile, type, expr }*/ );
                    }
                }
            }
        }
        private string[] IgnoredProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(pi => pi.GetCustomAttributes<IgnoreAttribute>().Any()).Select(pi => pi.Name).ToArray();
        }
        private void IgnoreProperties(Type type, object expr)
        {
            var mi = expr.GetType().GetMethod("ForMember",
                new Type[] { typeof(string), typeof(Action<IMemberConfigurationExpression>) });

            var smi = expr.GetType().GetMethod("ForSourceMember",
                new Type[] { typeof(string), typeof(Action<ISourceMemberConfigurationExpression>) });

            if (mi != null)
            {

                foreach (var propertyName in IgnoredProperties(type))
                {
                    if (DestinationType.GetProperty(propertyName) != null)
                    {
                        mi.Invoke(expr, new object[] { propertyName, new Action<IMemberConfigurationExpression>(opt => opt.Ignore()) });
                    }
                    else
                    {
                        smi?.Invoke(expr, new object[] { propertyName, new Action<ISourceMemberConfigurationExpression>(opt => opt.DoNotValidate()) });
                    }
                }
            }

        }

        #endregion ----------------------------------------------------- methods
    }
}
