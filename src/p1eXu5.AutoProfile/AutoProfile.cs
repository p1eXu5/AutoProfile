using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using AutoMapper;

// ReSharper disable once IdentifierTypo
namespace p1eXu5.AutoProfile
{
    using Attributes;
    using Contracts;

    /// <summary>
    /// Scans types with <see cref="MapAttribute"/> in the executing assembly
    /// and invokes map factory methods in those types is they set
    /// or applies default configurations.
    /// Sets default <see cref="DateTimeOffset"/> to <see cref="uint"/> maps.
    /// </summary>
    public class AutoProfile : Profile, IAutoProfile
    {
        #region fields

        private readonly Assembly _assembly;
        private bool _areMapsScanned;

        #endregion ----------------------------------------------------- fields


        #region ctor

        /// <summary>
        /// Sets executing assembly to scanned assembly.
        /// Do not forget to call <see cref="Configure"/>.
        /// </summary>
        /// <param name="serviceProvider"> <see cref="IServiceProvider"/> </param>
        public AutoProfile(IServiceProvider serviceProvider)
            : this(serviceProvider.GetRequiredService<ILogger>())
        { }


        /// <summary>
        /// Sets executing assembly to scanned assembly.
        /// Do not forget to call <see cref="Configure"/>.
        /// </summary>
        /// <param name="logger"></param>
        public AutoProfile(ILogger logger)
            : this(Assembly.GetExecutingAssembly(), logger)
        { }

        /// <summary>
        /// Do not forget to call <see cref="Configure"/>.
        /// </summary>
        /// <param name="assemblyType"> Type in a scanned assembly. </param>
        /// <param name="logger"> Logger. </param>
        public AutoProfile(Type assemblyType, ILogger logger)
            : this(Assembly.GetAssembly(assemblyType), logger)
        { }


        /// <summary>
        /// Do not forget to call <see cref="Configure"/>.
        /// </summary>
        /// <param name="assembly"> Scanned assembly. </param>
        /// <param name="logger"> Logger </param>
        public AutoProfile(Assembly assembly, ILogger logger)
        {
            this._assembly = assembly;
            Logger = logger;
        }

        #endregion ----------------------------------------------------- ctor


        #region IAutoProfile 

        public Profile Instance => this;

        public ILogger Logger { get; }

        #endregion ----------------------------------------------------- IAutoProfile


        #region methods

        /// <summary>
        /// Applies profile settings, create common maps, scan assembly and create attributed maps.
        /// </summary>
        /// <returns></returns>
        public virtual Profile Configure()
        {
            if (!_areMapsScanned)
            {
                Setup();
                CreateCommonMaps();

                ProcessMapAttributesFrom(_assembly);
                _areMapsScanned = true;
            }

            return this;
        }

        protected virtual void Setup()
        {
            this.AllowNullCollections = true;
            this.AllowNullDestinationValues = true;
        }

        protected virtual void CreateCommonMaps()
        {
            this.CreateMap<Int64, DateTimeOffset>().ConvertUsing(s => DateTimeOffset.FromUnixTimeMilliseconds(s));
            this.CreateMap<DateTimeOffset, Int64>().ConvertUsing(dt => dt.ToUnixTimeMilliseconds());
        }

        protected void ProcessMapAttributesFrom(Assembly assembly)
        {
            var types = assembly.GetExportedTypes().Where(t => t.GetCustomAttributes<MapAttribute>().Any()).ToArray();
            foreach (var type in types)
            {
                CreateMaps(type);
            }
        }

        /// <summary>
        /// Checks <see cref="MapAttribute"/>s on <paramref name="type"/> and calls <see cref="Profile.CreateMap(System.Type,System.Type)"/>.
        /// <para>
        /// Is used in tests.
        /// </para>
        /// </summary>
        /// <param name="type"></param>
        internal void CreateMaps(Type type)
        {
            var attributes = type.GetCustomAttributes<MapAttribute>().ToArray();
            foreach (MapAttribute mapAttribute in attributes)
            {
                mapAttribute.CreateMap(this, type);
            }
        }

        #endregion ----------------------------------------------------- methods
    }
}
