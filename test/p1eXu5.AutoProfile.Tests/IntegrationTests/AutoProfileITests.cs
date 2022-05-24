using System;
using System.Linq;
using Microsoft.Extensions.Logging;

using NUnit.Framework;
using Moq;

using AutoMapper;
using p1eXu5.AutoProfile.Contracts;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests
{
    using Attributes;
    using AutoMapper.Internal;
    using Fakes;


    [TestFixture]
    public class AutoProfileTests
    {
        private IMapper Mapper { get; set; } = default!;


        [OneTimeSetUp]
        public void CreateMapper()
        {
            AutoProfile autoProfile = new AutoProfile( typeof(Model), Mock.Of< ILogger >() );

            var conf = new MapperConfiguration( cfg => cfg.AddProfile( autoProfile.Configure() ) );
            conf.AssertConfigurationIsValid();

            Mapper = conf.CreateMapper();
        }

        [Test]
        public void ctor_ByDefault_ScansAndAddMapsFromAssembly()
        {
            // Action:
            var coll = Mapper.ConfigurationProvider.Internal().GetAllTypeMaps();

            // Assert:
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(Model), typeof(ModelDtoA) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(ModelDtoB), typeof(Model) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(Model), typeof(ModelDtoC) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(ModelDtoC), typeof(Model) ) ) );
        }

        [Test]
        public void ctor_DtoModelWithMapFromAttribute_MapsFromModelToDtoModel()
        {
            // Arrange:
            var date = DateTimeOffset.UtcNow;
            var model = new Model { Id = 2342, Name = "asdasd", Date = date };

            // Action:
            var dto = Mapper.Map<ModelDtoA>( model );

            // Assert:
            Assert.AreEqual( model.Id, dto.Id );
            Assert.AreEqual( model.Name, dto.Name );
            Assert.AreEqual( model.Date.ToUnixTimeMilliseconds(), dto.Date );
        }

        [Test]
        public void ctor_TestProfile_CallsFactoryMethodWithTestProfile()
        {
            // Arrange:
            Type type = null!;

            // Action:
            new TestProfile( Mock.Of< ILogger >(), setIAutoProfileInstanceType: t => type = t ).Configure();

            // Assert:
            Assert.NotNull( type );
            Assert.AreEqual( type, typeof( TestProfile ) );
        }


        #region fakes

        public class TestProfile : AutoProfile
        {
            public TestProfile(ILogger logger, Action<Type> setIAutoProfileInstanceType ) : base( typeof(TestModel), logger )
            {
                SetIAutoProfileInstanceType = setIAutoProfileInstanceType;
            }

            /// <summary>
            /// Getter calls in the <see cref="TestModel.CreateMap"/> method.
            /// </summary>
            public Action<Type> SetIAutoProfileInstanceType { get; }
        }


        [ MapFrom( typeof( Model ), MapFactory = nameof(TestModel.CreateMap) )]
        public class TestModel
        { 
            public IMappingExpression< Model, TestModel > CreateMap( IAutoProfile profile )
            {
                if ( profile is TestProfile testProfile) {
                    testProfile.SetIAutoProfileInstanceType( profile.GetType() );
                }

                return
                    profile.Instance.CreateMap< Model, TestModel >( MemberList.Destination );
            }
        }


        #endregion ----------------------------------------------------- factories
    }


}
