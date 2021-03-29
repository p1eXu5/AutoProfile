using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace p1eXu5.AutoProfile
{
    /// <summary>
    ///  To create <see cref="Mapper"/> call <see cref="SetMapperConfiguration"/>.
    /// </summary>
    /// <typeparam name="TProfile"></typeparam>
    public abstract class MapConfigurationTestsBase< TProfile > where TProfile : AutoProfile
    {
        public MapperConfiguration MapperConfiguration { get; private set; } = default!;
        public IMapper Mapper { get; private set; } = default!;
        protected abstract IEnumerable< Type > MappedTypes { get; }
        

        /// <summary>
        /// Instantiates <see cref="MapperConfiguration"/>, check if configuration is valid, creates <see cref="Mapper"/>.
        /// </summary>
        protected void SetMapperConfiguration()
        {
            AutoProfile profile = GetProfile();
            CreateTestMaps( profile );

            MapperConfiguration = new MapperConfiguration( cfg => {
                cfg.AddProfile( GetProfile() );
            } );


            MapperConfiguration.AssertConfigurationIsValid();

            Mapper = new Mapper( MapperConfiguration );
        }


        #region factories

        /// <summary>
        /// <see cref="AutoProfile"/> factory method. Is called in the <see cref="SetMapperConfiguration"/> method.
        /// <para>
        /// If you need to create application maps call <see cref="AutoProfile.Configure"/> method
        /// in the <see cref="CreateTestMaps"/> overload.
        /// </para>
        /// </summary>
        /// <returns></returns>
        protected abstract AutoProfile GetProfile();


        /// <summary>
        /// Enumerates <see cref="MappedTypes"/> and invokes <see cref="AutoProfile.CreateMaps"/> method.
        /// </summary>
        /// <param name="profile"></param>
        protected virtual void CreateTestMaps( AutoProfile profile )
        {
            foreach ( var mappedType in MappedTypes ) {
                profile.CreateMaps( mappedType );
            }
        }

        #endregion
    }
}
