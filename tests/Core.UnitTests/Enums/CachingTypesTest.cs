using Core.Enums;

namespace Core.UnitTests.Enums;

public sealed class CachingTypesTest
{
    [Fact]
    public void CachingTypes_None_HasExpectedValue()
    {
        Assert.Equal(0, (int)CachingTypes.None);
    }

    [Fact]
    public void CachingTypes_InMemory_HasExpectedValue()
    {
        Assert.Equal(1, (int)CachingTypes.InMemory);
    }

    [Fact]
    public void CachingTypes_FileSystem_HasExpectedValue()
    {
        Assert.Equal(2, (int)CachingTypes.FileSystem);
    }

    [Fact]
    public void CachingTypes_Enum_HasExpectedNames()
    {
        var names = Enum.GetNames(typeof(CachingTypes));
        Assert.Contains("None", names);
        Assert.Contains("InMemory", names);
        Assert.Contains("FileSystem", names);
    }
}