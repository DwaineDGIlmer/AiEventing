using Core.Models;

namespace Core.UnitTests.Models;

public class CustomerContextTest
{
    [Fact]
    public void DefaultConstructor_InitializesPropertiesWithDefaults()
    {
        var context = new CustomerContext();

        Assert.Equal(string.Empty, context.Id);
        Assert.Equal(string.Empty, context.CustomerId);
        Assert.Equal(string.Empty, context.AccountId);
        Assert.Equal(string.Empty, context.CustomerName);
        Assert.NotNull(context.ContactInformation);
        Assert.Equal(string.Empty, context.CustomerTier);
        Assert.Equal(string.Empty, context.CustomerLocation);
        Assert.Equal(string.Empty, context.CustomerTimeZone);
        Assert.NotNull(context.ExecutionOrder);
        Assert.Empty(context.ExecutionOrder);
        Assert.NotNull(context.CustomerAttributes);
        Assert.Empty(context.CustomerAttributes);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        var contactInfo = new ContactInformation { Email = "test@example.com" };
        var context = new CustomerContext
        {
            Id = "INC123",
            CustomerId = "CUST456",
            AccountId = "ACC789",
            CustomerName = "John Doe",
            ContactInformation = contactInfo,
            CustomerTier = "Gold",
            CustomerLocation = "New York",
            CustomerTimeZone = "EST",
            ExecutionOrder = ["Rule1", "Rule2"],
            CustomerAttributes = new Dictionary<string, object> { { "Age", 30 } }
        };

        Assert.Equal("INC123", context.Id);
        Assert.Equal("CUST456", context.CustomerId);
        Assert.Equal("ACC789", context.AccountId);
        Assert.Equal("John Doe", context.CustomerName);
        Assert.Equal(contactInfo, context.ContactInformation);
        Assert.Equal("Gold", context.CustomerTier);
        Assert.Equal("New York", context.CustomerLocation);
        Assert.Equal("EST", context.CustomerTimeZone);
        Assert.Equal(["Rule1", "Rule2"], context.ExecutionOrder);
        Assert.Equal(30, context.CustomerAttributes["Age"]);
    }

    [Fact]
    public void CustomerAttributes_CanAddAndRetrieveValues()
    {
        var context = new CustomerContext();
        context.CustomerAttributes["VIP"] = true;
        context.CustomerAttributes["Discount"] = 0.15;

        Assert.True((bool)context.CustomerAttributes["VIP"]);
        Assert.Equal(0.15, (double)context.CustomerAttributes["Discount"]);
    }

    [Fact]
    public void ExecutionOrder_CanAddRules()
    {
        var context = new CustomerContext();
        context.ExecutionOrder.Add("RuleA");
        context.ExecutionOrder.Add("RuleB");

        Assert.Contains("RuleA", context.ExecutionOrder);
        Assert.Contains("RuleB", context.ExecutionOrder);
        Assert.Equal(2, context.ExecutionOrder.Count);
    }
}