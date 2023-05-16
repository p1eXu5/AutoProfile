using Microsoft.Extensions.Logging;
using Moq;
using p1eXu5.AutoProfile.Contracts;
using p1eXu5.AutoProfile.Tests.Fakes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests
{
    public class StaticMapFactoryITests
    {
        [Test]
        public void StaticFactoryMethod_MapFactoryIsSet_MapFactoryTypeIsSet__FactoryMethodIsCalledWithExpectedProfile()
        {
            // Arrange:
            IAutoProfile? usingInFactoryAutoProfileInForward = null;
            IAutoProfile? usingInFactoryAutoProfileInReverse = null;
            var profile = new TestProfile(
                Mock.Of<ILogger>(),
                setUsingAutoProfileInForward: t => usingInFactoryAutoProfileInForward = t,
                setUsingAutoProfileInReverse: t => usingInFactoryAutoProfileInReverse = t
            );

            // Action:
            profile.CreateMaps(typeof(StaticFactoryMethodWithFactoryTypeModel));

            // Assert:
            Assert.NotNull(usingInFactoryAutoProfileInForward);
            Assert.AreSame(usingInFactoryAutoProfileInForward, profile);

            Assert.NotNull(usingInFactoryAutoProfileInReverse);
            Assert.AreSame(usingInFactoryAutoProfileInReverse, profile);
        }

        [Test]
        public void StaticFactoryMethod_MapFactoryIsSet_MapFactoryTypeIsNotSet__FactoryMethodIsCalledWithExpectedProfile()
        {
            // Arrange:
            IAutoProfile? usingInFactoryAutoProfileInForward = null;
            IAutoProfile? usingInFactoryAutoProfileInReverse = null;
            var profile = new TestProfile(
                Mock.Of<ILogger>(),
                setUsingAutoProfileInForward: t => usingInFactoryAutoProfileInForward = t,
                setUsingAutoProfileInReverse: t => usingInFactoryAutoProfileInReverse = t
            );
            // Action:
            profile.CreateMaps(typeof(StaticFactoryMethodWithNoFactoryTypeModel));

            // Assert:
            Assert.NotNull(usingInFactoryAutoProfileInForward);
            Assert.AreSame(usingInFactoryAutoProfileInForward, profile);

            Assert.NotNull(usingInFactoryAutoProfileInReverse);
            Assert.AreSame(usingInFactoryAutoProfileInReverse, profile);
        }


        // ==========
        //   Types
        // ==========

        public class TestProfile : AutoProfile
        {
            public TestProfile(ILogger logger, Action<IAutoProfile> setUsingAutoProfileInForward, Action<IAutoProfile> setUsingAutoProfileInReverse)
                : base(typeof(StaticFactoryMethodWithFactoryTypeModel), logger)
            {
                SetUsingAutoProfileInForward = setUsingAutoProfileInForward;
                SetUsingAutoProfileInReverse = setUsingAutoProfileInReverse;
            }

            /// <summary>
            /// Getter calls in the <see cref="p1eXu5.AutoProfile.Tests.IntegrationTests.TestModel.CreateMap"/> method.
            /// </summary>
            public Action<IAutoProfile> SetUsingAutoProfileInForward { get; }

            public Action<IAutoProfile> SetUsingAutoProfileInReverse { get; }
        }


        [MapFrom(typeof(Model),
            MapFactory = nameof(CreateMap),
            MapFactoryType = typeof(StaticFactoryMethodWithFactoryTypeModel),
            ReverseMap = true,
            ReverseMapFactory = nameof(CreateReverseMap))]
        public class StaticFactoryMethodWithFactoryTypeModel
        {
            public static IMappingExpression<Model, StaticFactoryMethodWithFactoryTypeModel> CreateMap(IAutoProfile profile)
            {
                if (profile is TestProfile testProfile)
                {
                    testProfile.SetUsingAutoProfileInForward(profile);
                }

                return
                    profile.Instance.CreateMap<Model, StaticFactoryMethodWithFactoryTypeModel>(MemberList.Destination);
            }


            public static IMappingExpression<StaticFactoryMethodWithFactoryTypeModel, Model> CreateReverseMap(
                IMappingExpression<Model, StaticFactoryMethodWithFactoryTypeModel> expr,
                IAutoProfile profile)
            {
                if (profile is TestProfile testProfile)
                {
                    testProfile.SetUsingAutoProfileInReverse(profile);
                }

                return
                    expr.ReverseMap();
            }
        }

        [MapFrom(typeof(Model),
            MapFactory = nameof(CreateMap),
            ReverseMap = true,
            ReverseMapFactory = nameof(CreateReverseMap))]
        public class StaticFactoryMethodWithNoFactoryTypeModel
        {
            public static IMappingExpression<Model, StaticFactoryMethodWithNoFactoryTypeModel> CreateMap(IAutoProfile profile)
            {
                if (profile is TestProfile testProfile)
                {
                    testProfile.SetUsingAutoProfileInForward(profile);
                }

                return
                    profile.Instance.CreateMap<Model, StaticFactoryMethodWithNoFactoryTypeModel>(MemberList.Destination);
            }

            public static IMappingExpression<StaticFactoryMethodWithNoFactoryTypeModel, Model> CreateReverseMap(
                IMappingExpression<Model, StaticFactoryMethodWithNoFactoryTypeModel> expr,
                IAutoProfile profile)
            {
                if (profile is TestProfile testProfile)
                {
                    testProfile.SetUsingAutoProfileInReverse(profile);
                }

                return
                    expr.ReverseMap();
            }
        }
    }
}
