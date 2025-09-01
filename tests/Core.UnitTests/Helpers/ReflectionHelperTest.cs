using Core.Helpers;
using System.Reflection;

namespace Core.UnitTests.Helpers;

public sealed class ReflectionHelperTest
{
    private class TestClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsActive { get; set; }
        public decimal Salary { get; set; }
        public TestEnum Status { get; set; }
        public int? NullableInt { get; set; }
    }

    public enum TestEnum
    {
        None,
        Active,
        Inactive
    }

    private PropertyInfo GetProperty(string name) =>
        typeof(TestClass).GetProperty(name)!;

    [Fact]
    public void ShouldIgnoreProperty_NullValue_ReturnsTrue()
    {
        var prop = GetProperty(nameof(TestClass.Name));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void ShouldIgnoreProperty_EmptyOrWhitespaceString_ReturnsTrue(string value)
    {
        var prop = GetProperty(nameof(TestClass.Name));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, value));
    }

    [Fact]
    public void ShouldIgnoreProperty_NonEmptyString_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.Name));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, "John"));
    }

    [Fact]
    public void ShouldIgnoreProperty_DefaultDateTime_ReturnsTrue()
    {
        var prop = GetProperty(nameof(TestClass.BirthDate));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, default(DateTime)));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, DateTime.MinValue));
    }

    [Fact]
    public void ShouldIgnoreProperty_ValidDateTime_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.BirthDate));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, DateTime.Now));
    }

    [Fact]
    public void ShouldIgnoreProperty_EmptyList_ReturnsTrue()
    {
        var prop = GetProperty(nameof(TestClass.Tags));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, new List<string>()));
    }

    [Fact]
    public void ShouldIgnoreProperty_NonEmptyList_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.Tags));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, new List<string> { "tag1" }));
    }

    [Fact]
    public void ShouldIgnoreProperty_EmptyArray_ReturnsTrue()
    {
        var prop = GetProperty(nameof(TestClass.Tags));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, Array.Empty<string>()));
    }

    [Fact]
    public void ShouldIgnoreProperty_NumericType_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.Age));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, 0));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, 42));
    }

    [Fact]
    public void ShouldIgnoreProperty_DecimalType_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.Salary));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, 0m));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, 100.5m));
    }

    [Fact]
    public void ShouldIgnoreProperty_BooleanType_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.IsActive));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, false));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, true));
    }

    [Fact]
    public void ShouldIgnoreProperty_EnumType_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.Status));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, TestEnum.None));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, TestEnum.Active));
    }

    [Fact]
    public void ShouldIgnoreProperty_NullableInt_Null_ReturnsTrue()
    {
        var prop = GetProperty(nameof(TestClass.NullableInt));
        Assert.True(ReflectionHelper.ShouldIgnoreProperty(prop, null));
    }

    [Fact]
    public void ShouldIgnoreProperty_NullableInt_Value_ReturnsFalse()
    {
        var prop = GetProperty(nameof(TestClass.NullableInt));
        Assert.False(ReflectionHelper.ShouldIgnoreProperty(prop, 5));
    }
}