using static p1eXu5.AutoProfile.Tests.IntegrationTests.MapTo.MapToTestsTypes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapTo;

[TestFixture]
public class MapToTests : AutoMapperTestsBase
{
    protected override ICollection<Type> MappingTypes { get; } =
        new Type[] {
            typeof(MasterSimpleClassModel),
            typeof(MasterSimpleStructModel),
            typeof(MasterSimpleRecordModel),
            typeof(MasterSimpleRecordStructModel),
            typeof(Ignored.MasterSimpleClassModel),
            typeof(Opposite.MasterSimpleClassModelWithDestinationMemberList),
            typeof(Opposite.MasterSimpleClassModelWithSourceMemberList),
        };


    [TestCaseSource(typeof(TestCases), nameof(TestCases.SimpleTypeCases))]
    public void MapToAttributeWithNoAdditionalSettings_DestinationTypeWithTheSameProperties_MapsAllSourceProperties(
        object masterModel,
        Type sourceType,
        Type destinationType)
    {
        // Arrange:
        // Action:
        var resultModel = (ISimpleClassModel)Mapper.Map(masterModel, sourceType, destinationType);

        // Assert:
        resultModel.Should().BeEquivalentTo((ISimpleClassModel)masterModel);
    }

    [Test]
    public void MasterModelWithIgnoredProperty_ByDefault_NotMappingProperty()
    {
        // Arrange:
        ISimpleClassModel master = AutoFaker.Generate<Ignored.MasterSimpleClassModel>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<SlaveSimpleClassModel>(master);

        // Assert:
        slave.Should().NotBeEquivalentTo(master);
    }

    [Test]
    public void SlaveModelWithIgnoredProperty_ByDefault_NotMappingProperty()
    {
        // Arrange:
        ISimpleClassModel master = AutoFaker.Generate<MasterSimpleClassModel>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<Ignored.SlaveSimpleClassModel>(master);

        // Assert:
        slave.Should().NotBeEquivalentTo(master);
    }

    [Test]
    public void MasterModelWithOppositeProperty_MemberListIsDestination_MappingProperty()
    {
        // Arrange:
        var master = AutoFaker.Generate<Opposite.MasterSimpleClassModelWithDestinationMemberList>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<SlaveSimpleClassModel>(master);

        // Assert:
        slave.StringProp.Should().BeEquivalentTo(master.StringAnotherProp);
    }

    [Test]
    public void MasterModelWithOppositeProperty_MemberListIsSource_MappingProperty()
    {
        // Arrange:
        var master = AutoFaker.Generate<Opposite.MasterSimpleClassModelWithSourceMemberList>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<SlaveSimpleClassModel>(master);

        // Assert:
        slave.StringProp.Should().BeEquivalentTo(master.StringAnotherProp);
    }

    [Test]
    public void MasterModelWithOppositeProperty_MemberListIsDestination_ToRecord_MappingProperty()
    {
        // Arrange:
        var master = AutoFaker.Generate<Opposite.MasterSimpleClassModelWithDestinationMemberList>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<SlaveSimpleRecordModel>(master);

        // Assert:
        slave.StringProp.Should().BeEquivalentTo(master.StringAnotherProp);
    }
}
