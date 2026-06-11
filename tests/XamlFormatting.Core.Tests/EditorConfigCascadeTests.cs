using System;
using System.IO;
using XamlFormatting.Core;
using XamlFormatting.Core.EditorConfig;
using Xunit;

namespace XamlFormatting.Core.Tests
{
    /// <summary>
    /// End-to-end tests that exercise real on-disk <c>.editorconfig</c> discovery, the
    /// <c>root = true</c> stop, and nearest-file-wins cascade through the library.
    /// </summary>
    public sealed class EditorConfigCascadeTests : IDisposable
    {
        private readonly string _root;

        public EditorConfigCascadeTests()
        {
            _root = Path.Combine(Path.GetTempPath(), "XamlFormattingTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);

            // root = true isolates the test from any .editorconfig higher up the machine.
            Write(_root, ".editorconfig",
                "root = true",
                "",
                "[*]",
                "indent_style = space",
                "indent_size = 4",
                "",
                "[*.xaml]",
                "indent_size = 2",
                "xaml_max_attributes_per_line = 1");

            Directory.CreateDirectory(Path.Combine(_root, "sub"));
            Write(Path.Combine(_root, "sub"), ".editorconfig",
                "[*.xaml]",
                "indent_size = 8");
        }

        [Fact]
        public void TopLevelXamlFile_UsesRootConfig()
        {
            XamlFormatterOptions options =
                new EditorConfigOptionsProvider().GetOptionsForFile(Path.Combine(_root, "Main.xaml"));

            Assert.False(options.IndentWithTabs);        // indent_style = space (from [*])
            Assert.Equal(2, options.IndentSize);         // [*.xaml] override
            Assert.Equal("1", options.XamlProperties["xaml_max_attributes_per_line"]);
        }

        [Fact]
        public void NestedXamlFile_NearestConfigWins()
        {
            XamlFormatterOptions options =
                new EditorConfigOptionsProvider().GetOptionsForFile(Path.Combine(_root, "sub", "View.xaml"));

            Assert.Equal(8, options.IndentSize);         // nested [*.xaml] overrides root
            Assert.False(options.IndentWithTabs);        // still inherited from root [*]
            Assert.Equal("1", options.XamlProperties["xaml_max_attributes_per_line"]); // inherited
        }

        [Fact]
        public void NonXamlFile_DoesNotGetXamlSections()
        {
            XamlFormatterOptions options =
                new EditorConfigOptionsProvider().GetOptionsForFile(Path.Combine(_root, "sub", "notes.txt"));

            Assert.Equal(4, options.IndentSize);         // only [*] applies
            Assert.Empty(options.XamlProperties);
        }

        private static void Write(string directory, string name, params string[] lines)
        {
            File.WriteAllLines(Path.Combine(directory, name), lines);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_root, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup of the temp directory.
            }
        }
    }
}
