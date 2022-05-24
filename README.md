AutoProfile. 
============

<em>Custom [AutoMapper](https://github.com/AutoMapper "AutoMapper") attributes and profiler.</em>

| Package                     | Versions                                                                                                                |
| --------------------------- | ----------------------------------------------------------------------------------------------------------------------- |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-11.1.1-brightgreen)](https://www.nuget.org/packages/p1eXu5.AutoProfile/11.1.1)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-11.0.1-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/11.0.1)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-10.1.5.2-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/10.1.5.2)     |
| p1eXu5.AutoProfile          | [![NuGet](https://img.shields.io/badge/nuget-0.1.0-green)](https://www.nuget.org/packages/p1eXu5.AutoProfile/0.1.0)     |


<br/>

## Using
<br/>

1. Configure in the ASP.NET Core:

    Install-Package AutoMapper.Extensions.Microsoft.DependencyInjection -Version 11.0.0

    ```csharp
    services.AddAutoMapper( (serviceProvider, cfg) => {
        var logger = serviceProvider.GetRequiredService<ILogger<AutoProfile>>();
        var profile = new AutoProfile( logger );
        cfg.AddProfile( profile.Configure() );
    }, new Type[0] );
    ```
    
    Blazor:
    ```csharp
    builder.Services.AddAutoMapper((serviceProvider, cfg) => { 
        var logger = serviceProvider.GetRequiredService<ILogger<AutoProfile>>();
        var profile = new AutoProfile(typeof(Model), logger);
        cfg.AddProfile( profile.Configure() );
    }, new Type[0]);
    ```

2. Plain creation:

    ```csharp
    AutoProfile autoProfile = new AutoProfile( typeof(Model), Mock.Of< ILogger >() );

    var conf = new MapperConfiguration( cfg => cfg.AddProfile( autoProfile.Configure() ) );
    conf.AssertConfigurationIsValid();

    Mapper = conf.CreateMapper();
    ```

3. For testing of certain maps:

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


## Map configuration examples.
<br/>

1. Using map configuration factory method:

    ```csharp
    [MapTo( typeof(Model), 
        MapFactory = nameof(Dto.CreateMapMethod), 
        IncludeAllDerived = true, 
        ReverseMap = true, IncludeAllDerivedForReverse = true )]
    public class Dto
    {
        ...

        public IMappingExpression< ProjectDto, Project > CreateMapMethod( IAutoProfile profile )
        {
            return
                profile.Instance.CreateMap< Dto, Model >( MemberList.Source )...
        }
    }
    ```
    <br/>

2. Using reverse map factory configuration method:

    ```csharp
    [MapTo( typeof(IApplicationUser), 
        IncludeAllDerived = true, 
        IncludeAllDerivedForReverse = true,
        MapFactory = nameof( UserDtoBase.CreateMap ), 
        ReverseMapFactory = nameof( UserDtoBase.CreateReverseMap ) )]
    public class UserDtoBase
    {
        ...

        public IMappingExpression< UserDtoBase, IApplicationUser > CreateMap( DerivedAutoProfile profile )
        {
            return
                profile.Instance.CreateMap< UserDtoBase, IApplicationUser >( MemberList.Source )
                    
                    .ForMember( au => au.FirstName, opt => opt.MapFrom( ( dto, au ) => TextCipherSet.Encrypt( dto.FirstName, profile.PassPhrase ) ) )
                    .ForMember( au => au.LastName, opt => opt.MapFrom( ( dto, au ) => TextCipherSet.Encrypt( dto.LastName, profile.PassPhrase ) ) )
                ;
        }


        // expr and profile can have any order in parameters
        public IMappingExpression< IApplicationUser, UserDtoBase > CreateReverseMap( 
            IMappingExpression< UserDtoBase, IApplicationUser > expr, 
            DerivedAutoProfile profile )
        {
            return
                expr.ReverseMap()
                    .ForMember( dto => dto.FirstName, opt => opt.MapFrom( (au, dto) => TextCipherSet.Decrypt( au.FirstName, profile.PassPhrase ) ) )
                    .ForMember( dto => dto.LastName, opt => opt.MapFrom( (au, dto) => TextCipherSet.Decrypt( au.LastName, profile.PassPhrase) ) )
                ;
        }
    }
    ```
    <br/>

3. Ignoring property.

    - ignoring destination property:

        ```csharp
        [MapFrom( typeof(IModelA) )]
        [MapFrom( typeof(Model), ... )]
        public class ViewModel...
        {
            ...

            [Ignore]
            public IList< string > RoleNames { get; set; } = default!;
        }
        ```
        <br/>

        ```csharp
        [MapTo( typeof(ModelA) )]
        public class ViewModel...
        {
            ...

            public IList< string > RoleNames { get; set; } = default!;
        }

        public class ModelA...
        {
            ...

            [Ignore]
            public IList< string > RoleNames { get; set; } = default!;
        }
        ```
        <br/>

    - ignoring source property:
        ```csharp
        [MapTo( typeof(IModelA) )]
        public class ViewModel...
        {
            ...

            [Ignore]
            public IList< string > RoleNames { get; set; } = default!;
        }
        ```
        <br/>

        ```csharp
        [MapFrom( typeof(ModelA) )]
        public class ViewModel...
        {
            ...

            public IList< string > RoleNames { get; set; } = default!;
        }

        public class ModelA...
        {
            ...

            [Ignore]
            public IList< string > RoleNames { get; set; } = default!;
        }
        ```
        <br/>



4. OppositeAttribute usage:

    ```csharp
    [MapTo( typeof(IApplicationUser ), ... )]
    [MapFrom( typeof(IApplicationUser) )]
    public class UserUpdateDto : UserDto, IEntityIdDto< string >
    {
        ...

        [Opposite( nameof(IApplicationUser.UserTools) )]
        public ICollection< EntityIdDto >? Tools { get; set; }

        ...
    }
    ```
    <br/>

## Workarounds.
<br/>

* When a collection is mapped with resolver additional configuration is needed or AssertConfigurationIsValid() is failed (Automapper 9.0.0 bug):

    ```csharp
    ...
    public IMappingExpression< ProjectDto, Project > CreateMap( IAutoProfile profile )
    {
        return
            profile.Instance.CreateMap< Dto, Model >( MemberList.Source )
                .ForMember( model => model.Collection, opt => opt.MapFrom( dto => dto.Collection ) )
                .ForMember( model => model.Collection,
                    opt => {
                        opt.MapFrom< FooResolver, ICollection< FooType >? >( dto => dto.Collection );
                    })
            ;
    }
    ```

* In some cases you'll have to use both MapTo and MapFrom attribute instead of using ReverseMap = true only or AssertConfigurationIsValid() is failed (Automapper 9.0.0 bug).
