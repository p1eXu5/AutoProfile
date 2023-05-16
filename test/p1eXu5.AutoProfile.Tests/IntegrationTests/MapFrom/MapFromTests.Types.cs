namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapFrom;

using IgnoreAttribute = AutoMapper.Configuration.Annotations.IgnoreAttribute;

public class MapFromTestsTypes
{
    public enum TestEnum
    {
        One,
        Two,
        Three
    }

    public interface ISimpleClassModelBase
    {
        int IntProp { get; }

        bool BoolProp { get; }

        double DoubleProp { get; }

        TestEnum EnumProp { get; }
    }

    public interface ISimpleClassModel : ISimpleClassModelBase
    {
        string StringProp { get; }
    }

    [MapFrom(typeof(MasterSimpleClassModel))]
    [MapFrom(typeof(Ignored.MasterSimpleClassModel))]
    public class SlaveSimpleClassModel : ISimpleClassModel
    {
        public int IntProp { get; init; }

        public string StringProp { get; init; } = default!;

        public bool BoolProp { get; init; }

        public double DoubleProp { get; init; }

        public TestEnum EnumProp { get; init; }
    }

    [MapFrom(typeof(MasterSimpleClassModel))]
    public struct SlaveSimpleStructModel : ISimpleClassModel
    {
        public int IntProp { get; init; }

        public string StringProp { get; init; }

        public bool BoolProp { get; init; }

        public double DoubleProp { get; init; }

        public TestEnum EnumProp { get; init; }
    }

    [MapFrom(typeof(MasterSimpleClassModel))]
    public record SlaveSimpleRecordModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;

    [MapFrom(typeof(MasterSimpleClassModel))]
    public record struct SlaveSimpleRecordStructModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;

    public class MasterSimpleClassModel : ISimpleClassModel
    {
        public int IntProp { get; init; }

        public string StringProp { get; init; } = default!;

        public bool BoolProp { get; init; }

        public double DoubleProp { get; init; }

        public TestEnum EnumProp { get; init; }
    }


    public static class Ignored
    {
        public class MasterSimpleClassModel : ISimpleClassModel
        {
            public int IntProp { get; init; }

            [Ignore]
            public string StringProp { get; init; } = default!;

            public bool BoolProp { get; init; }

            public double DoubleProp { get; init; }

            public TestEnum EnumProp { get; init; }
        }

        [MapFrom(typeof(MapFromTestsTypes.MasterSimpleClassModel))]
        public class SlaveSimpleClassModel : ISimpleClassModel
        {
            public int IntProp { get; init; }

            [Ignore]
            public string StringProp { get; init; } = default!;

            public bool BoolProp { get; init; }

            public double DoubleProp { get; init; }

            public TestEnum EnumProp { get; init; }

        }

        [MapFrom(typeof(MapFromTestsTypes.MasterSimpleClassModel))]
        public class WithNotExistedInMasterPropSlaveSimpleClassModel : ISimpleClassModel
        {
            public int IntProp { get; init; }

            public string StringProp { get; init; } = default!;

            public bool BoolProp { get; init; }

            public double DoubleProp { get; init; }

            public TestEnum EnumProp { get; init; }

            [Ignore]
            public string NotExistedStringProp { get; init; } = default!;
        }
    }

    public static class Opposite
    {
        [MapFrom(typeof(MapFromTestsTypes.MasterSimpleClassModel), MemberList = MemberList.Destination)]
        public class DestinationSlaveSimpleClassModel : ISimpleClassModelBase
        {
            public int IntProp { get; init; }

            [Opposite(nameof(MapFromTestsTypes.MasterSimpleClassModel.StringProp))]
            public string StringAnotherProp { get; init; } = default!;

            public bool BoolProp { get; init; }

            public double DoubleProp { get; init; }

            public TestEnum EnumProp { get; init; }
        }

        [MapFrom(typeof(MapFromTestsTypes.MasterSimpleClassModel), MemberList = MemberList.Source)]
        public class SourceSlaveSimpleClassModel : ISimpleClassModelBase
        {
            public int IntProp { get; init; }

            [Opposite(nameof(MapFromTestsTypes.MasterSimpleClassModel.StringProp))]
            public string StringAnotherProp { get; init; } = default!;

            public bool BoolProp { get; init; }

            public double DoubleProp { get; init; }

            public TestEnum EnumProp { get; init; }
        }
    }
}

