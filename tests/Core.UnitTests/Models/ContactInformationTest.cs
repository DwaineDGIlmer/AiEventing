using Core.Models;

namespace Core.UnitTests.Models;

public class ContactInformationTest
{
    [Fact]
    public void Default_Properties_Should_Be_Empty_Strings()
    {
        var contactInfo = new ContactInformation();
        Assert.Equal(string.Empty, contactInfo.Email);
        Assert.Equal(string.Empty, contactInfo.Phone);
    }

    [Fact]
    public void Can_Set_And_Get_Email()
    {
        var contactInfo = new ContactInformation
        {
            Email = "test@example.com"
        };
        Assert.Equal("test@example.com", contactInfo.Email);
    }

    [Fact]
    public void Can_Set_And_Get_Phone()
    {
        var contactInfo = new ContactInformation
        {
            Phone = "123-456-7890"
        };
        Assert.Equal("123-456-7890", contactInfo.Phone);
    }

    [Fact]
    public void Can_Set_Both_Email_And_Phone()
    {
        var contactInfo = new ContactInformation
        {
            Email = "user@domain.com",
            Phone = "555-1234"
        };
        Assert.Equal("user@domain.com", contactInfo.Email);
        Assert.Equal("555-1234", contactInfo.Phone);
    }
}