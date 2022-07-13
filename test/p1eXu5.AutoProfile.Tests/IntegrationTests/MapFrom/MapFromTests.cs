using System.Collections;
using static p1eXu5.AutoProfile.Tests.IntegrationTests.MapFrom.MapFromTestsTypes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapFrom;

[TestFixture]
public class MapFromTests : AutoMapperTestsBase
{
    protected override ICollection<Type> MappingTypes { get; } =
        new Type[] {
            typeof(SlaveSimpleClassModel),
            typeof(SlaveSimpleStructModel),
            typeof(SlaveSimpleRecordModel),
            typeof(SlaveSimpleRecordStructModel),
            typeof(Ignored.SlaveSimpleClassModel),
            typeof(Ignored.WithNotExistedInMasterPropSlaveSimpleClassModel),
            typeof(Opposite.DestinationSlaveSimpleClassModel),
            typeof(Opposite.SourceSlaveSimpleClassModel),
        };


    [TestCaseSource(typeof(TestCases), nameof(TestCases.SimpleTypeCases))]
    public void MapToAttributeWithNoAdditionalSettings_DestinationTypeWithTheSameProperties_MapsAllSourceProperties(
        object masterModel, Type sourceType, Type destinationType)
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
    public void SlaveModelWithIgnoredNotExistedInMasterProperty_ByDefault_NotMappingProperty()
    {
        // Arrange:
        ISimpleClassModel master = AutoFaker.Generate<MasterSimpleClassModel>();

        // Action:
        ISimpleClassModel slave = Mapper.Map<Ignored.WithNotExistedInMasterPropSlaveSimpleClassModel>(master);

        // Assert:
        slave.Should().BeEquivalentTo(master);
    }

    [Test]
    public void SlaveModelWithOppositeProperty_MemberListIsDestination_MappingProperty()
    {
        // Arrange:
        var master = AutoFaker.Generate<MasterSimpleClassModel>();

        // Action:
        Opposite.DestinationSlaveSimpleClassModel slave = Mapper.Map<Opposite.DestinationSlaveSimpleClassModel>(master);

        // Assert:
        slave.StringAnotherProp.Should().BeEquivalentTo(master.StringProp);
    }

    [Test]
    public void SlaveModelWithOppositeProperty_MemberListIsSource_MappingProperty()
    {
        // Arrange:
        var master = AutoFaker.Generate<MasterSimpleClassModel>();

        // Action:
        Opposite.SourceSlaveSimpleClassModel slave = Mapper.Map<Opposite.SourceSlaveSimpleClassModel>(master);

        // Assert:
        slave.StringAnotherProp.Should().BeEquivalentTo(master.StringProp);
    }
}

