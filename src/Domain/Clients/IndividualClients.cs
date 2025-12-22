using SharedKernel;

namespace Domain.Clients;

public sealed class IndividualClient : Entity
{
    private IndividualClient(
        Guid id,
        string firstName,
        string lastName) : base(id)
	{ 
        FirstName = firstName;
        LastName = lastName;
	}
	public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public static IndividualClient Create(
        string firstName,
        string lastName)
    => new(Guid.NewGuid(), firstName, lastName);
    
}
