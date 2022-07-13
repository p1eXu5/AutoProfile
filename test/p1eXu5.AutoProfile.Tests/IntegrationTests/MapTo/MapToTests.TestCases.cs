using System.Collections;
using static p1eXu5.AutoProfile.Tests.IntegrationTests.MapTo.MapToTestsTypes;

namespace p1eXu5.AutoProfile.Tests.IntegrationTests.MapTo;

/// <summary>
/// <see cref="MapToTests"/> cases.
/// </summary>
public static class TestCases
{
    public static IEnumerable SimpleTypeCases
    {
        get {
            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleClassModel>(), typeof(MasterSimpleClassModel), typeof(SlaveSimpleClassModel) }
                ).SetName("class map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleStructModel>(), typeof(MasterSimpleStructModel), typeof(SlaveSimpleStructModel) }
                ).SetName("struct map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleRecordModel>(), typeof(MasterSimpleRecordModel), typeof(SlaveSimpleRecordModel) }
                ).SetName("record map");

            yield return
                new TestCaseData(
                    new object[] { AutoFaker.Generate<MasterSimpleRecordStructModel>(), typeof(MasterSimpleRecordStructModel), typeof(SlaveSimpleRecordStructModel) }
                ).SetName("record struct map");
        }
    }
}
