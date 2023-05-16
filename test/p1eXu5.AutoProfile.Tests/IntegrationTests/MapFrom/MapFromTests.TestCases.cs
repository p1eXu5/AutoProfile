using System.Collections;
using static p1eXu5.AutoProfile.Tests.IntegrationTests.MapFrom.MapFromTestsTypes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapFrom;

/// <summary>
/// <see cref="MapFromTests"/> cases.
/// </summary>
public static class TestCases
{
    public static IEnumerable SimpleTypeCases
    {
        get
        {
            yield return
                new TestCaseData(
                    new object[] {
                        AutoFaker.Generate<MasterSimpleClassModel>(), typeof(MasterSimpleClassModel), typeof(SlaveSimpleClassModel) }
                ).SetName("class map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleClassModel>(), typeof(MasterSimpleClassModel), typeof(SlaveSimpleStructModel) }
                ).SetName("struct map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleClassModel>(), typeof(MasterSimpleClassModel), typeof(SlaveSimpleRecordModel) }
                ).SetName("record map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleClassModel>(), typeof(MasterSimpleClassModel), typeof(SlaveSimpleRecordStructModel) }
                ).SetName("record struct map");
        }
    }
}
