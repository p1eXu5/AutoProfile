using p1eXu5.AutoProfile.Tests.Factories;

namespace p1eXu5.AutoProfile.Tests;

public abstract class AutoMapperTestsBase
{
    protected IMapper Mapper { get; private set; } = default!;

    protected virtual ICollection<Type> MappingTypes { get; } = Array.Empty<Type>();

    [OneTimeSetUp]
    public void Initialize()
    {
        AutoProfile autoProfile = new AutoProfile(
            MockLoggerFactories.GetMockILogger<AutoProfile>(TestContext.WriteLine).Object,
            new AutoProfileOptions(NotProcessMapAttributesFromAssembly: true));

        foreach (var type in MappingTypes)
        {
            autoProfile.CreateMaps(type);
        }

        var conf = new MapperConfiguration(cfg => cfg.AddProfile(autoProfile.Configure()));
        conf.AssertConfigurationIsValid();

        Mapper = conf.CreateMapper();
    }
}
