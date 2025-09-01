using Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Core.Configuration.Tests
{
    sealed public class ResilientHttpPolicyTest
    {
        [Fact]
        public void Default_Constructor_Sets_Default_Values()
        {
            var policy = new ResilientHttpPolicy();

            Assert.Equal(string.Empty, policy.HttpClientName);
            Assert.Equal(string.Empty, policy.PolicyName);
            Assert.True(policy.Enabled);
            Assert.Equal(0, policy.PolicyOrder);
            Assert.Equal(Defaults.HttpTimeout, policy.HttpTimeout);
            Assert.True(policy.UseStandardResilience);
            Assert.NotNull(policy.RetryPolicy);
            Assert.NotNull(policy.CircuitBreakerPolicy);
            Assert.NotNull(policy.BulkheadPolicy);
        }

        [Fact]
        public void HttpClientName_Required_Validation()
        {
            var policy = new ResilientHttpPolicy { HttpClientName = "" };
            var context = new ValidationContext(policy);
            var results = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(policy, context, results, true);

            Assert.Contains(results, r => r.ErrorMessage == "HttpClientName is required.");
            Assert.False(valid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void PolicyOrder_OutOfRange_Validation(int invalidOrder)
        {
            var policy = new ResilientHttpPolicy { HttpClientName = "Test", PolicyOrder = invalidOrder };
            var context = new ValidationContext(policy);
            var results = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(policy, context, results, true);

            Assert.Contains(results, r => r.ErrorMessage == "PolicyOrder must be between 0 and 3.");
            Assert.False(valid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void PolicyOrder_Valid_Validation(int validOrder)
        {
            var policy = new ResilientHttpPolicy { HttpClientName = "Test", PolicyOrder = validOrder };
            var context = new ValidationContext(policy);
            var results = new List<ValidationResult>();

            bool valid = Validator.TryValidateObject(policy, context, results, true);

            Assert.True(valid);
        }

        [Fact]
        public void Can_Set_Properties()
        {
            var retry = new RetrySettings();
            var circuitBreaker = new CircuitBreakerSettings();
            var bulkhead = new BulkheadSettings();

            var policy = new ResilientHttpPolicy
            {
                HttpClientName = "MyClient",
                PolicyName = "MyPolicy",
                Enabled = false,
                PolicyOrder = 2,
                HttpTimeout = 60,
                UseStandardResilience = false,
                RetryPolicy = retry,
                CircuitBreakerPolicy = circuitBreaker,
                BulkheadPolicy = bulkhead
            };

            Assert.Equal("MyClient", policy.HttpClientName);
            Assert.Equal("MyPolicy", policy.PolicyName);
            Assert.False(policy.Enabled);
            Assert.Equal(2, policy.PolicyOrder);
            Assert.Equal(60, policy.HttpTimeout);
            Assert.False(policy.UseStandardResilience);
            Assert.Equal(retry, policy.RetryPolicy);
            Assert.Equal(circuitBreaker, policy.CircuitBreakerPolicy);
            Assert.Equal(bulkhead, policy.BulkheadPolicy);
        }
    }
}