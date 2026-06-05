using System;
using Xunit;

namespace XamlFormatting.Vsix.Tests
{
    /// <summary>
    /// Guards the hand-maintained GUID/ID table in <c>VSCommandTable.cs</c> against the
    /// symbols declared in <c>VSCommandTable.vsct</c>. These run without a live VS instance.
    /// </summary>
    public class CommandTableTests
    {
        [Fact]
        public void PackageGuid_MatchesVsctSymbol()
        {
            Assert.Equal(
                new Guid("b8f5a9e2-3c1d-4f6a-9e21-7d4c8a1b2f30"),
                PackageGuids.XamlFormattingPackage);
        }

        [Fact]
        public void CommandSetGuid_MatchesVsctSymbol()
        {
            Assert.Equal(
                new Guid("c2e4f7a1-5b3d-4e8c-a1f9-6d2b7c9e4a50"),
                PackageGuids.XamlFormattingCmdSet);
        }

        [Fact]
        public void PackageAndCommandSetGuids_AreDistinct()
        {
            Assert.NotEqual(PackageGuids.XamlFormattingPackage, PackageGuids.XamlFormattingCmdSet);
        }

        [Fact]
        public void FormatXamlCommandId_MatchesVsctSymbol()
        {
            Assert.Equal(0x0100, PackageIds.FormatXamlCommand);
        }
    }
}
