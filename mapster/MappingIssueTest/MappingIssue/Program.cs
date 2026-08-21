using Mapster;

// ReSharper disable InconsistentNaming

// The following code does NOT map the RecordUID across the two records. You need Mapster 7.4.0 or lower to reproduce
// the issue. This is fixed in Mapster 10.0.0 and later.

var company = new Company(123, "Big Bob's Hardware", "Bob's");

var dto = company.Adapt<CompanyDto>();

if (dto.RecordUID is null)
{
    Console.WriteLine("UID is null");
}
else
{
    Console.WriteLine("UID is " + dto.RecordUID);
}

public sealed record Company(int? RecordUID, string Name, string? ShortName);

// ReSharper disable once ClassNeverInstantiated.Global
public sealed record CompanyDto(int? RecordUID, string Name, string? ShortName);