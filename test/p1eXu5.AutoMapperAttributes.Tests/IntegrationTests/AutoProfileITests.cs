using System;
using System.Linq;
using Microsoft.Extensions.Logging;

using NUnit.Framework;
using Moq;

using AutoMapper;

namespace p1eXu5.AutoMapperAttributes.Tests.IntegrationTests
{
    using Attributes;
    using Tests.Models;


    [TestFixture]
    public class AutoProfileITests
    {
        private IMapper Mapper { get; set; }


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
            var coll = Mapper.ConfigurationProvider.GetAllTypeMaps();

            // Assert:
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(Model), typeof(ModelDtoA) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(ModelDtoB), typeof(Model) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(Model), typeof(ModelDtoC) ) ) );
            Assert.IsTrue( coll.Any( m => m.Types == new TypePair( typeof(ModelDtoC), typeof(Model) ) ) );
        }

        [Test]
        public void ctor_ByDefault_CreatesMapFrom()
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
            Type type = null;

            // Action:
            new TestProfile( Mock.Of< ILogger >(), t => type = t ).Configure();

            // Assert:
            Assert.NotNull( type );
            Assert.AreEqual( type, typeof( TestProfile ) );
        }
    }


    #region fakes

    public class TestProfile : AutoProfile
    {
        public TestProfile(ILogger logger, Action<Type> setProfileType ) : base( typeof(TestModel), logger )
        {
            SetProfileType = setProfileType;
        }

        public Action<Type> SetProfileType { get; }
    }


    [ MapFrom( typeof( Model ), MapFactory = nameof(TestModel.CreateMap) )]
    public class TestModel
    { 
        public IMappingExpression< Model, TestModel > CreateMap( TestProfile profile )
        {
            profile.SetProfileType( profile.GetType() );

            return
                profile.Instance.CreateMap< Model, TestModel >( MemberList.Destination );
        }
    }


    #endregion ----------------------------------------------------- factories
}
