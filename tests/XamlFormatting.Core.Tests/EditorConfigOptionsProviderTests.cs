using System;
using System.Collections.Generic;
using XamlFormatting.Core;
using XamlFormatting.Core.EditorConfig;
using Xunit;

namespace XamlFormatting.Core.Tests
{
    public class EditorConfigOptionsProviderTests
    {
        private static XamlFormatterOptions Map(params (string Key, string Value)[] pairs)
        {
            var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach ((string key, string value) in pairs)
            {
                props[key] = value;
            }

            return new EditorConfigOptionsProvider().GetOptions(props);
        }

        [Fact]
        public void GetOptions_NullProperties_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new EditorConfigOptionsProvider().GetOptions(null!));
        }

        [Fact]
        public void GetOptions_EmptyProperties_ReturnsDefaults()
        {
            XamlFormatterOptions options = Map();

            Assert.False(options.IndentWithTabs);
            Assert.Equal(4, options.IndentSize);
            Assert.Equal(4, options.TabWidth);
            Assert.Equal(EndOfLineMarker.CrLf, options.EndOfLine);
            Assert.False(options.InsertFinalNewline);
            Assert.False(options.TrimTrailingWhitespace);
            Assert.Equal(0, options.MaxLineLength);
            Assert.Empty(options.XamlProperties);
        }

        [Theory]
        [InlineData("tab", true)]
        [InlineData("space", false)]
        [InlineData("TAB", true)]
        public void GetOptions_MapsIndentStyle(string value, bool expectTabs)
        {
            Assert.Equal(expectTabs, Map(("indent_style", value)).IndentWithTabs);
        }

        [Fact]
        public void GetOptions_IndentSize_AlsoDefaultsTabWidth()
        {
            XamlFormatterOptions options = Map(("indent_size", "2"));

            Assert.Equal(2, options.IndentSize);
            Assert.Equal(2, options.TabWidth); // tab_width defaults to indent_size when unset
        }

        [Fact]
        public void GetOptions_ExplicitTabWidth_DoesNotOverrideIndentSize()
        {
            XamlFormatterOptions options = Map(("indent_size", "2"), ("tab_width", "8"));

            Assert.Equal(2, options.IndentSize);
            Assert.Equal(8, options.TabWidth);
        }

        [Fact]
        public void GetOptions_IndentSizeTab_FollowsTabWidth()
        {
            XamlFormatterOptions options = Map(("indent_size", "tab"), ("tab_width", "3"));

            Assert.Equal(3, options.IndentSize);
            Assert.Equal(3, options.TabWidth);
        }

        [Theory]
        [InlineData("lf", EndOfLineMarker.Lf)]
        [InlineData("cr", EndOfLineMarker.Cr)]
        [InlineData("crlf", EndOfLineMarker.CrLf)]
        [InlineData("CRLF", EndOfLineMarker.CrLf)]
        public void GetOptions_MapsEndOfLine(string value, EndOfLineMarker expected)
        {
            Assert.Equal(expected, Map(("end_of_line", value)).EndOfLine);
        }

        [Fact]
        public void GetOptions_MapsBooleanKeys()
        {
            XamlFormatterOptions options = Map(
                ("insert_final_newline", "true"),
                ("trim_trailing_whitespace", "TRUE"));

            Assert.True(options.InsertFinalNewline);
            Assert.True(options.TrimTrailingWhitespace);
        }

        [Theory]
        [InlineData("100", 100)]
        [InlineData("off", 0)]
        public void GetOptions_MapsMaxLineLength(string value, int expected)
        {
            Assert.Equal(expected, Map(("max_line_length", value)).MaxLineLength);
        }

        [Fact]
        public void GetOptions_UnsetValue_IsTreatedAsAbsent()
        {
            XamlFormatterOptions options = Map(("indent_size", "unset"), ("insert_final_newline", "unset"));

            Assert.Equal(4, options.IndentSize); // default retained
            Assert.False(options.InsertFinalNewline);
        }

        [Fact]
        public void GetOptions_InvalidValues_FallBackToDefaults()
        {
            XamlFormatterOptions options = Map(("indent_size", "wide"), ("insert_final_newline", "yes"));

            Assert.Equal(4, options.IndentSize);
            Assert.False(options.InsertFinalNewline);
        }

        [Fact]
        public void GetOptions_CapturesOnlyXamlPrefixedKeys()
        {
            XamlFormatterOptions options = Map(
                ("indent_size", "2"),
                ("xaml_max_attributes_per_line", "1"),
                ("xaml_attributes_tolerance", "2"));

            Assert.Equal(2, options.XamlProperties.Count);
            Assert.Equal("1", options.XamlProperties["xaml_max_attributes_per_line"]);
            // Lookup is case-insensitive, and standard keys are excluded.
            Assert.Equal("2", options.XamlProperties["XAML_ATTRIBUTES_TOLERANCE"]);
            Assert.False(options.XamlProperties.ContainsKey("indent_size"));
        }
    }
}
