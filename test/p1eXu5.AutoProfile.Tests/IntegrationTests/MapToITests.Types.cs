using p1eXu5.AutoProfile.Attributes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests;

public partial class MapToITests
{
    public enum TestEnum
    {
        One,
        Two,
        Three
    }

    [MapTo(typeof(SlaveSimpleClassModel))]
    public class MasterSimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; }
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }

    public class SlaveSimpleClassModel
    {
        public int IntProp { get; init; }
        public string StringProp { get; init; }
        public bool BoolProp { get; init; }
        public double DoubleProp { get; init; }
        public TestEnum EnumProp { get; init; }
    }
}
    
