using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests;
public abstract class TestsBase
{
    protected IMapper Mapper { get; private set; } = default!;

    [OneTimeSetUp]
    public void Initialize()
    {
        AutoProfile autoProfile = new AutoProfile(typeof(Model), Mock.Of<ILogger>());

        var conf = new MapperConfiguration(cfg => cfg.AddProfile(autoProfile.Configure()));
        conf.AssertConfigurationIsValid();

        Mapper = conf.CreateMapper();
    }
}
