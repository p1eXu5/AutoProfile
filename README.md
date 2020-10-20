AutoMapper Attributes
=====================


- ASP.NET Core:

    ```csharp
    services.AddAutoMapper( (serviceProvider, cfg) => { 
        var profile = new AutoProfile( serviceProvider );
        cfg.AddProfile( profile.Configure() );
    }, new Type[0] );
    ```


- Setup in tests:

    ```csharp
    AutoProfile autoProfile = new AutoProfile( typeof(Model), Mock.Of< ILogger >() );

    var conf = new MapperConfiguration( cfg => cfg.AddProfile( autoProfile.Configure() ) );
    conf.AssertConfigurationIsValid();

    Mapper = conf.CreateMapper();
    ```



* When a collection is mapped with resolver additional configuration is needed (Automapper 9.0.0 bug):

    ```csharp
    public IMappingExpression< ProjectDto, Project > CreateMap( IAutoProfile profile )
    {
        return
            profile.Instance.CreateMap< Dto, Model >( MemberList.Source )
                .ForMember( model => model.Collection, opt => opt.MapFrom( dto => dto.Collection ) )
                .ForMember( model => model.Collection,
                    opt => {
                        opt.MapFrom< Resolver, ICollection< Type >? >( dto => dto.Collection );     // for example
                    })
            ;
    }
    ```


* MapToAttribute and MapFromAttribute usage examples:

    ```csharp
    [MapTo( typeof(Model), 
        MapFactory = nameof(Dto.CreateMapMethod), 
        IncludeAllDerived = true, 
        ReverseMap = true, IncludeAllDerivedForReverse = true )]
    public class Dto
    {
        ...

        public IMappingExpression< ProjectDto, Project > CreateMap( IAutoProfile profile )
        {
            return
                profile.Instance.CreateMap< Dto, Model >( MemberList.Source )...
        }
    }
    ```

    ```csharp
    [MapTo( typeof(IApplicationUser), 
        IncludeAllDerived = true, 
        IncludeAllDerivedForReverse = true,
        MapFactory = nameof( UserDtoBase.CreateMap ), 
        ReverseMapFactory = nameof( UserDtoBase.CreateReverseMap ) )]
    public class UserDtoBase
    {
        ...

        public IMappingExpression< UserDtoBase, IApplicationUser > CreateMap( IAutoProfile profile )
        {
            return
                profile.Instance.CreateMap< UserDtoBase, IApplicationUser >( MemberList.Source )
                    
                    .ForMember( au => au.FirstName, opt => opt.MapFrom( ( dto, au ) => TextCipherSet.Encrypt( dto.FirstName, profile.PassPhrase ) ) )
                    .ForMember( au => au.LastName, opt => opt.MapFrom( ( dto, au ) => TextCipherSet.Encrypt( dto.LastName, profile.PassPhrase ) ) )
                ;
        }

        public IMappingExpression< IApplicationUser, UserDtoBase > CreateReverseMap( IMappingExpression< UserDtoBase, IApplicationUser > expr, IAutoProfile profile )
        {
            return
                expr.ReverseMap()
                    .ForMember( dto => dto.FirstName, opt => opt.MapFrom( (au, dto) => TextCipherSet.Decrypt( au.FirstName, profile.PassPhrase ) ) )
                    .ForMember( dto => dto.LastName, opt => opt.MapFrom( (au, dto) => TextCipherSet.Decrypt( au.LastName, profile.PassPhrase) ) )
                ;
        }
    }
    ```


* IgnoreAttribute attribute example:

    ```csharp
    [ MapFrom( typeof(IModelA) )]
    [ MapFrom( typeof(Model), MapFactory = nameof( ViewModel.CreateMap )) ]
    public class ViewModel...
    {
        ...

        [Ignore]
        public IList< string > RoleNames { get; set; } = default!;
    }
    ```



* OppositeAttribute attribute example:

    ```csharp
    [ MapTo( typeof(IApplicationUser ), MapFactory = nameof( UserUpdateDto.CreateMap ) )]
    [ MapFrom( typeof(IApplicationUser)) ]
    public class UserUpdateDto : UserDto, IEntityIdDto< string >
    {
        ...

        [ Opposite( nameof(IApplicationUser.UserTools)) ]
        public ICollection< EntityIdDto >? Tools { get; set; }

        ...
    }
    ```

