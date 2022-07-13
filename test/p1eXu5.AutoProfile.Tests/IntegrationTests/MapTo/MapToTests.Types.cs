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

    [MapTo(typeof(SlaveSimpleStructModel))]
    public struct MasterSimpleStructModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; }
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }

    [MapTo(typeof(SlaveSimpleRecordModel))]
    public record MasterSimpleRecordModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;



    [MapTo(typeof(SlaveSimpleRecordStructModel))]
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


    public struct SlaveSimpleStructModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; }
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }
    
    public record SlaveSimpleRecordModel : ISimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; } = default!;
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }



    public record struct SlaveSimpleRecordStructModel(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;


    // Error: no available constructor!
    public record SlaveSimpleRecordModelWithCtor(
        int IntProp,
        string StringProp,
        bool BoolProp,
        double DoubleProp,
        TestEnum EnumProp
    ) : ISimpleClassModel;


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

    public static class Opposite
    {
        [MapTo(typeof(MapToTestsTypes.SlaveSimpleClassModel), MemberList = MemberList.Destination)]
        [MapTo(typeof(MapToTestsTypes.SlaveSimpleRecordModel), MemberList = MemberList.Destination)]
        public class MasterSimpleClassModelWithDestinationMemberList : ISimpleClassModelBase
        {
            public int IntProp { get; init; }

            [Opposite(nameof(MapToTestsTypes.SlaveSimpleClassModel.StringProp))]
            public string StringAnotherProp { get; init; } = default!;

            public bool BoolProp { get; init; }
            public double DoubleProp { get; init; }
            public TestEnum EnumProp { get; init; }
        }

        [MapTo(typeof(MapToTestsTypes.SlaveSimpleClassModel), MemberList = MemberList.Source)]
        public class MasterSimpleClassModelWithSourceMemberList : ISimpleClassModelBase
        {
            public int IntProp { get; init; }
        
            [Opposite(nameof(MapToTestsTypes.SlaveSimpleClassModel.StringProp))]
            public string StringAnotherProp { get; init; } = default!;
        
            public bool BoolProp { get; init; }
            public double DoubleProp { get; init; }
            public TestEnum EnumProp { get; init; }
        }
    }
}

