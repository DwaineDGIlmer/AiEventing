using Core.Helpers;

namespace Core.UnitTests.Helpers;

sealed public class ErrorFactoryTest
{
    [Fact]
    public void CreateWorkflowError_ReturnsCorrectError()
    {
        var error = ErrorFactory.CreateWorkflowError("OrderWorkflow", "404", "Not found", new List<string> { "Step 1 failed" });

        Assert.Equal("WF_404", error.ErrorCode);
        Assert.Equal("[Workflow:OrderWorkflow] Not found", error.ErrorMessage);
        Assert.Contains("Step 1 failed", error.ErrorDetails);
    }

    [Fact]
    public void CreateWorkflowError_NullDetails_ReturnsEmptyList()
    {
        var error = ErrorFactory.CreateWorkflowError("OrderWorkflow", "404", "Not found");

        Assert.NotNull(error.ErrorDetails);
        Assert.Empty(error.ErrorDetails);
    }

    [Fact]
    public void CreateServiceError_ReturnsCorrectError()
    {
        var error = ErrorFactory.CreateServiceError("PaymentService", "500", "Internal error", new List<string> { "Timeout" });

        Assert.Equal("SVC_500", error.ErrorCode);
        Assert.Equal("[Service:PaymentService] Internal error", error.ErrorMessage);
        Assert.Contains("Timeout", error.ErrorDetails);
    }

    [Fact]
    public void CreateServiceError_NullDetails_ReturnsEmptyList()
    {
        var error = ErrorFactory.CreateServiceError("PaymentService", "500", "Internal error");

        Assert.NotNull(error.ErrorDetails);
        Assert.Empty(error.ErrorDetails);
    }

    [Fact]
    public void CreateApplicationError_ReturnsCorrectError()
    {
        var error = ErrorFactory.CreateApplicationError("UI", "401", "Unauthorized", new List<string> { "Missing token" });

        Assert.Equal("APP_401", error.ErrorCode);
        Assert.Equal("[Application:UI] Unauthorized", error.ErrorMessage);
        Assert.Contains("Missing token", error.ErrorDetails);
    }

    [Fact]
    public void CreateApplicationError_NullDetails_ReturnsEmptyList()
    {
        var error = ErrorFactory.CreateApplicationError("UI", "401", "Unauthorized");

        Assert.NotNull(error.ErrorDetails);
        Assert.Empty(error.ErrorDetails);
    }

    [Fact]
    public void CreateValidationError_ReturnsCorrectError()
    {
        var error = ErrorFactory.CreateValidationError("UserInput", "email", "Invalid email", new List<string> { "Format incorrect" });

        Assert.Equal("VAL_EMAIL_INVALID", error.ErrorCode);
        Assert.Equal("[Validation:UserInput] Invalid email", error.ErrorMessage);
        Assert.Contains("Format incorrect", error.ErrorDetails);
    }

    [Fact]
    public void CreateValidationError_NullDetails_ReturnsEmptyList()
    {
        var error = ErrorFactory.CreateValidationError("UserInput", "email", "Invalid email");

        Assert.NotNull(error.ErrorDetails);
        Assert.Empty(error.ErrorDetails);
    }

    [Fact]
    public void CreateExceptionError_ReturnsCorrectError()
    {
        var ex = new InvalidOperationException("Operation failed", new Exception("Inner"));
        var error = ErrorFactory.CreateExceptionError("Processor", ex, new List<string> { "Extra detail" });

        Assert.StartsWith("EXC_INVALIDOPERATIONEXCEPTION", error.ErrorCode);
        Assert.StartsWith("[Exception:Processor] Operation failed", error.ErrorMessage);
        Assert.Contains("Exception Type: InvalidOperationException", error.ErrorDetails);
        Assert.Contains("Stack Trace:", error.ErrorDetails[2]);
        Assert.Contains("Inner Exception: Inner", error.ErrorDetails);
        Assert.Contains("Extra detail", error.ErrorDetails);
    }

    [Fact]
    public void CreateExceptionError_NullDetails_InitializesList()
    {
        var ex = new Exception("Fail");
        var error = ErrorFactory.CreateExceptionError("Processor", ex);

        Assert.Contains("Exception Type: Exception", error.ErrorDetails);
        Assert.Contains("Stack Trace:", error.ErrorDetails[1]);
    }
}