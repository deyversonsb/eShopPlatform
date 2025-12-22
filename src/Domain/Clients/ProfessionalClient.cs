using SharedKernel;

namespace Domain.Clients;

public sealed class ProfessionalClient : Entity
{
    private ProfessionalClient(
        Guid id,
        string companyName,
        string? vatNumber,
        string businessRegistrationNumber,
        decimal annualRevenue): base(id)
    {
        CompanyName = companyName;
        VatNumber = vatNumber;
        BusinessRegistrationNumber = businessRegistrationNumber;
        AnnualRevenue = annualRevenue;
	}
    public string CompanyName { get; private set; }
    public string? VatNumber { get; private set; }
    public string BusinessRegistrationNumber { get; private set; }
    public decimal AnnualRevenue { get; private set; }
    public static ProfessionalClient Create(
        string companyName,
        string? vatNumber,
        string businessRegistrationNumber,
        decimal annualRevenue)
        => new(Guid.NewGuid(), companyName, vatNumber, businessRegistrationNumber, annualRevenue);

}
