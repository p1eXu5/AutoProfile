using Microsoft.Extensions.Logging;
using Moq;
using p1eXu5.AutoProfile.Contracts;
using p1eXu5.AutoProfile.Tests.Fakes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests
{
    public class StaticMapFactoryITests
    {
        [Test]
        public void ctor_MapFactoryTypeAttribute_StaticCreateMapMethod_IsCalled()
        {
            // Arrange:
            Type? type = null;
            var profile = new TestProfile( Mock.Of< ILogger >(), setIAutoProfileInstanceType: t => type = t );

            // Action:
            profile.CreateMaps( typeof(TestModelWithStaticFactoryMethod) );

            // Assert:
            Assert.NotNull( type );
            Assert.AreEqual( type, typeof( TestProfile ) );
        }


        #region fakes

        public class TestProfile : AutoProfile
        {
            public TestProfile(ILogger logger, Action<Type> setIAutoProfileInstanceType ) : base( typeof(TestModelWithStaticFactoryMethod), logger )
            {
                SetIAutoProfileInstanceType = setIAutoProfileInstanceType;
            }

            /// <summary>
            /// Getter calls in the <see cref="p1eXu5.AutoProfile.Tests.IntegrationTests.TestModel.CreateMap"/> method.
            /// </summary>
            public Action<Type> SetIAutoProfileInstanceType { get; }
        }


        [MapFrom( typeof(Model), MapFactory = nameof(TestModelWithStaticFactoryMethod.CreateMap), MapFactoryType = typeof(TestModelWithStaticFactoryMethod))]
        public class TestModelWithStaticFactoryMethod
        { 
            public static IMappingExpression< Model, TestModelWithStaticFactoryMethod > CreateMap( IAutoProfile profile )
            {
                if ( profile is TestProfile testProfile) {
                    testProfile.SetIAutoProfileInstanceType( profile.GetType() );
                }

                return
                    profile.Instance.CreateMap< Model, TestModelWithStaticFactoryMethod >( MemberList.Destination );
            }
        }


        #endregion ----------------------------------------------------- factories
    }

}
