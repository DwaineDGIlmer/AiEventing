using Domain.Customers;

namespace Domain.UnitTests.Customers;

public class CustomerTest
{
    [Fact]
    public void Contact_SetProperties_ValuesAreSet()
    {
        var contact = new Customer
        {
            Name = "John Smith",
            Email = "john.smith@example.com",
            Title = "Developer",
            AccountId = "12345",
            Id = "67890"
        };

        Assert.Equal("John Smith", contact.Name);
        Assert.Equal("john.smith@example.com", contact.Email);
        Assert.Equal("Developer", contact.Title);
        Assert.Equal("12345", contact.AccountId);
        Assert.Equal("67890", contact.Id);
    }
}
