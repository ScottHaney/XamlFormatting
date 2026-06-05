using System;
using Xunit;

namespace XamlFormatting.Core.Tests
{
    public class XamlFormatterTests
    {
        [Fact]
        public void Format_NullInput_Throws()
        {
            var formatter = new XamlFormatter();

            Assert.Throws<ArgumentNullException>(() => formatter.Format(null!));
        }

        [Fact]
        public void Format_WithoutOptions_UsesDefaultsAndReturnsInput()
        {
            // Step 1 scaffold: the formatter is a pass-through until step 3 lands.
            var formatter = new XamlFormatter();
            const string xaml = "<Grid><TextBlock Text=\"Hi\" /></Grid>";

            string result = formatter.Format(xaml);

            Assert.Equal(xaml, result);
        }

        [Fact]
        public void DefaultOptions_HaveEditorConfigDefaults()
        {
            XamlFormatterOptions options = XamlFormatterOptions.Default;

            Assert.False(options.IndentWithTabs);
            Assert.Equal(4, options.IndentSize);
        }
    }
}
