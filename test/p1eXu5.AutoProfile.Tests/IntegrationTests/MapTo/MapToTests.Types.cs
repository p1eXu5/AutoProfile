namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapTo;

using IgnoreAttribute = AutoMapper.Configuration.Annotations.IgnoreAttribute;


public class MapToTestsTypes
{
    public enum TestEnum
    {
        One,
        Two,
        Three
    }


    public interface ISimpleClassModel
    {
        int IntProp { get; }
        string StringProp { get; }
        bool BoolProp { get; }
        double DoubleProp { get; }
        TestEnum EnumProp { get; }
    }


    [MapTo(typeof(SlaveSimpleClassModel))]
    [MapTo(typeof(Ignored.SlaveSimpleClassModel))]
    public class MasterSimpleClassModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; } = default!;
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }

    [MapTo(typeof(SlaveSimpleClassModel))]
    public struct MasterSimpleStructModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; }
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }

    [MapTo(typeof(SlaveSimpleClassModel))]
    public record MasterSimpleRecordModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;

    [MapTo(typeof(SlaveSimpleClassModel))]
    public record struct MasterSimpleRecordStructModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;





    public class SlaveSimpleClassModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; } = default!;
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }


    public static class Ignored
    {
        public class SlaveSimpleClassModel : ISimpleClassModel
        {
            public int IntProp { get; init; }

            [Ignore]
            public string StringProp { get; init; } = default!;

            public bool BoolProp { get; init; }
            public double DoubleProp { get; init; }
            public TestEnum EnumProp { get; init; }
        }


        [MapTo(typeof(MapToTestsTypes.SlaveSimpleClassModel))]
        public class MasterSimpleClassModel : ISimpleClassModel
        {
            public int IntProp { get; init; }

            [Ignore]
            public string StringProp { get; init; } = default!;

            public bool BoolProp { get; init; }
            public double DoubleProp { get; init; }
            public TestEnum EnumProp { get; init; }
        }
    }
}

