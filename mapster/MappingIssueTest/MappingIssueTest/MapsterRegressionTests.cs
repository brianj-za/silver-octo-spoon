using Mapster;
using AwesomeAssertions;

namespace MappingIssueTest;

public class MapsterRegressionTests
{
    [Trait("Mapster", "Regression")]
    [Fact]
    public void RecordUID_Should_BeMappedCorrectly()
    {
        // Arrange
        var company = new Company(
            RecordUID: 42,
            Name: "Bob's Shop of Hardware",
            ShortName: "Bob's Hardware"
        );

        // Act
        var companyDto = company.Adapt<CompanyDto>();

        // Assert
        companyDto.RecordUID.Should().Be(42);
    }
}