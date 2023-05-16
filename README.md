AutoProfile. 
============

<em>Custom [AutoMapper](https://github.com/AutoMapper "AutoMapper") attributes and profiler.</em>

| Package                     | Versions                                                                                                                |
| --------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-11.1.4-brightgreen)](https://www.nuget.org/packages/p1eXu5.AutoProfile/11.1.4)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-11.1.2-brightgreen)](https://www.nuget.org/packages/p1eXu5.AutoProfile/11.1.2)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-11.0.1-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/11.0.1)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-10.1.5.2-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/10.1.5.2)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-0.1.0-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/0.1.0)     |

<br/>

## Mapping configurations.

*see [test project](https://github.com/p1eXu5/AutoProfile/tree/master/test/p1eXu5.AutoProfile.Tests) for more examples*

### 1. Using map configuration factory method:

<br/>

```csharp
[MapTo(typeof(Model), 
    MapFactory = nameof(CreateMapMethod), 
    IncludeAllDerived = true, 
    ReverseMap = true,
    IncludeAllDerivedForReverse = true)]
[MapFrom(typeof(Model2), MapFactory = nameof(CreateMapMethod))]
public class Dto
{
    ...

    public IMappingExpression<Dto, Model> CreateMapMethod(IAutoProfile profile)
    {
        return
            profile.Instance.CreateMap<Dto, Model>(MemberList.Source);
    }

    public static IMappingExpression<Model2, Dto> StaticCreateMapMethod(IAutoProfile profile)
    {
        return
            profile.Instance.CreateMap<Model2, Dto>(MemberList.Source);
    }
}
```

If factory method is located in other type use `MapFactoryType` attribute property.

<br/>

### 2. Using reverse map factory configuration method:

<br/>

```csharp
[MapTo( typeof(IApplicationUser), 
    IncludeAllDerived = true, 
    IncludeAllDerivedForReverse = true,
    MapFactory = nameof(UserDtoBase.CreateMap), 
    ReverseMapFactory = nameof(UserDtoBase.CreateReverseMap))]
public class UserDtoBase
{
    public IMappingExpression<UserDtoBase, IApplicationUser> CreateMap(DerivedAutoProfile profile)
    {
        return
            profile.Instance.CreateMap<UserDtoBase, IApplicationUser>(MemberList.Source)
                .ForMember(au => au.FirstName, opt => opt.MapFrom((dto, au) => TextCipherSet.Encrypt( dto.FirstName, profile.PassPhrase)))
                .ForMember(au => au.LastName, opt => opt.MapFrom((dto, au) => TextCipherSet.Encrypt( dto.LastName, profile.PassPhrase)));
    }

    // expr and profile can have any order in parameters
    public IMappingExpression<IApplicationUser, UserDtoBase> CreateReverseMap( 
        IMappingExpression<UserDtoBase, IApplicationUser> expr, 
        DerivedAutoProfile profile)
    {
        return
            expr.ReverseMap()
                .ForMember(dto => dto.FirstName, opt => opt.MapFrom((au, dto) => TextCipherSet.Decrypt( au.FirstName, profile.PassPhrase)))
                .ForMember(dto => dto.LastName, opt => opt.MapFrom((au, dto) => TextCipherSet.Decrypt( au.LastName, profile.PassPhrase)));
    }
}
```

<br/>

### 3. Ignoring property.

<br/>

#### Ignoring destination property:

<br/>

```csharp
[MapFrom(typeof(IModelA))]
[MapFrom(typeof(Model) /*, ...*/)]
public class ViewModel
{
    [Ignore]
    public IList<string> RoleNames { get; set; } = default!;
}
```

<br/>

```csharp
[MapTo(typeof(ModelA))]
public class ViewModel...
{
    public IList<string> RoleNames { get; set; } = default!;
}

public class ModelA...
{
    [Ignore]
    public IList<string> RoleNames { get; set; } = default!;
}
```

<br/>

#### Ignoring source property:

<br/>

```csharp
[MapTo(typeof(IModelA))]
public class ViewModel
{
    [Ignore]
    public IList<string> RoleNames { get; set; } = default!;
}
```

<br/>

```csharp
[MapFrom(typeof(ModelA))]
public class ViewModel
{
    public IList<string> RoleNames { get; set; } = default!;
}

public class ModelA
{
    [Ignore]
    public IList<string> RoleNames { get; set; } = default!;
}
```

<br/>

### 4. OppositeAttribute usage:

<br/>

```csharp
[MapTo(typeof(IApplicationUser))]
[MapFrom(typeof(IApplicationUser))]
public class UserUpdateDto : UserDto, IEntityIdDto<string>
{
    [Opposite(nameof(IApplicationUser.UserTools))]
    public ICollection<EntityIdDto>? Tools { get; set; }
}
```

<br/>

#### <b>Warning! There is an workaround to use with immutable record:</b>

<br/>

1. Define private parameterless constructor
2. Add private init

    ```csharp
    public record RecordModel
    {
        private RecordModel()
        { }

        public RecordModel(string oppositeProp) : this()
        {
            this.OppositeProp = oppositeProp;
        }
        
        public string OppositeProp { get; private init; }
    }
    ```

<br/>

## Using

<br/>

### 1. Configure in the ASP.NET Core:

<br/>

Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 11.0.0

```csharp
builder.Services.AddAutoMapper(
    (serviceProvider, cfg) =>
    { 
        var logger = serviceProvider.GetRequiredService<ILogger<AutoProfile>>();
        var profile = new AutoProfile(typeof(Model), logger);
        cfg.AddProfile(profile.Configure());
    },
     new Type[0]);
```

<br/>

### 2. For testing of certain maps:

<br/>

```csharp
public abstract class AutoMapperTestsBase
{
    protected IMapper Mapper { get; private set; } = default!;

    protected virtual ICollection<Type> MappingTypes { get; } = Array.Empty<Type>();

    [OneTimeSetUp]
    public void Initialize()
    {
        AutoProfile autoProfile = new AutoProfile(
            MockLoggerFactories.GetMockILogger<AutoProfile>(TestContext.WriteLine).Object,
            new AutoProfileOptions(ProcessMapAttributesFromAssembly: false));

        foreach (var type in MappingTypes) {
            autoProfile.CreateMaps(type);
        }

        var conf = new MapperConfiguration(cfg => cfg.AddProfile(autoProfile.Configure()));
        conf.AssertConfigurationIsValid();

        Mapper = conf.CreateMapper();
    }
}
```

<br/>



## Other known issuers & workarounds.

<br/>

* When a collection is mapped with resolver additional configuration is needed or AssertConfigurationIsValid() is failed (Automapper 9.0.0 bug):

    ```csharp
    public IMappingExpression<ProjectDto, Project> CreateMap(IAutoProfile profile)
    {
        return
            profile.Instance.CreateMap<Dto, Model>(MemberList.Source)
                .ForMember(model => model.Collection, opt => opt.MapFrom(dto => dto.Collection))
                .ForMember(model => model.Collection,
                    opt => {
                        opt.MapFrom<FooResolver, ICollection<FooType>?>(dto => dto.Collection);
                    });
    }
    ```

* In some cases you'll have to use both MapTo and MapFrom attribute instead of using ReverseMap = true only or AssertConfigurationIsValid() is failed (Automapper 9.0.0 bug).
