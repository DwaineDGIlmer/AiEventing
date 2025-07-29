using Core.Helpers;

namespace Core.UnitTests.Helpers;

public class FileSystemHelpersTest
{
    [Fact]
    public void SanitizeForFileSystem_ReturnsDefaultName_WhenInputIsNullOrWhitespace()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { DefaultName = "default" };
        Assert.Equal("default", FileSystemHelpers.SanitizeForFileSystem(null!, options));
        Assert.Equal("default", FileSystemHelpers.SanitizeForFileSystem("", options));
        Assert.Equal("default", FileSystemHelpers.SanitizeForFileSystem("   ", options));
    }

    [Fact]
    public void SanitizeForFileSystem_ConvertsToLowercase_WhenOptionIsSet()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { ConvertToLowercase = true };
        Assert.Equal("filename", FileSystemHelpers.SanitizeForFileSystem("FileName", options));
    }

    [Fact]
    public void SanitizeForFileSystem_ReplacesSpaces_WhenOptionIsSet()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { ReplaceSpaces = true, SpaceReplacement = '-' };
        Assert.Equal("file-name", FileSystemHelpers.SanitizeForFileSystem("file name", options));
    }

    [Fact]
    public void SanitizeForFileSystem_RemovesAccents_WhenOptionIsSet()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { RemoveAccents = true };
        Assert.Equal("resume", FileSystemHelpers.SanitizeForFileSystem("résumé", options));
    }

    [Fact]
    public void SanitizeForFileSystem_RemovesInvalidChars_WhenOptionIsSet()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { RemoveInvalidChars = true };
        string input = "file<name>?";
        string sanitized = FileSystemHelpers.SanitizeForFileSystem(input, options);
        Assert.DoesNotContain('<', sanitized);
        Assert.DoesNotContain('?', sanitized);
    }

    [Fact]
    public void SanitizeForFileSystem_HandlesReservedNames()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { HandleReservedNames = true, ReservedNamePrefix = "x_" };
        Assert.StartsWith("x_", FileSystemHelpers.SanitizeForFileSystem("CON", options));
        Assert.StartsWith("x_", FileSystemHelpers.SanitizeForFileSystem("com1", options));
    }

    [Fact]
    public void SanitizeForFileSystem_TrimsProblematicChars()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { TrimProblematicChars = true };
        Assert.Equal("filename", FileSystemHelpers.SanitizeForFileSystem(" filename. ", options));
    }

    [Fact]
    public void SanitizeForFileSystem_CollapsesReplacementChars()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { CollapseReplacements = true, ReplacementChar = '_' };
        string input = "file??name";
        string sanitized = FileSystemHelpers.SanitizeForFileSystem(input, options);
        Assert.DoesNotContain("__", sanitized);
    }

    [Fact]
    public void SanitizeForFileSystem_PreservesExtension_WhenTruncating()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { MaxLength = 10, PreserveExtension = true };
        string input = "verylongfilename.txt";
        string sanitized = FileSystemHelpers.SanitizeForFileSystem(input, options);
        Assert.EndsWith(".txt", sanitized);
        Assert.True(sanitized.Length <= 10);
    }

    [Fact]
    public void SanitizeForFileSystem_UsesAdditionalInvalidChars()
    {
        var options = new FileSystemHelpers.FileSystemSanitizeOptions { AdditionalInvalidChars = new[] { '@', '#' }, RemoveInvalidChars = true };
        string input = "file@name#test";
        string sanitized = FileSystemHelpers.SanitizeForFileSystem(input, options);
        Assert.DoesNotContain('@', sanitized);
        Assert.DoesNotContain('#', sanitized);
    }
}