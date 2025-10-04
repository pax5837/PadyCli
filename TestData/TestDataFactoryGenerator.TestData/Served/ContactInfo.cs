namespace TestDataFactoryGenerator.TestData.Served;

internal abstract record ContactInfo(Person Person);

internal record EmailContactInfo(Person Person, string Email) : ContactInfo(Person);

internal record PhoneContactInfo(Person Person, string PhoneNumber) : ContactInfo(Person);