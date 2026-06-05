namespace XamlFormatting.Core
{
    /// <summary>
    /// Settings that control how a XAML document is formatted.
    /// </summary>
    /// <remarks>
    /// In step 2 these properties are populated from the resolved <c>.editorconfig</c>
    /// (e.g. <c>indent_style</c>, <c>indent_size</c>, plus XAML-specific keys). For now
    /// the type carries only the editorconfig-standard formatting basics so the option
    /// flow can be exercised end-to-end before the full rule set lands.
    /// </remarks>
    public sealed class XamlFormatterOptions
    {
        /// <summary>
        /// The default options used when none are supplied.
        /// </summary>
        public static XamlFormatterOptions Default { get; } = new XamlFormatterOptions();

        /// <summary>
        /// Whether to indent with tabs (<see langword="true"/>) or spaces
        /// (<see langword="false"/>). Maps to the editorconfig <c>indent_style</c> key.
        /// </summary>
        public bool IndentWithTabs { get; set; }

        /// <summary>
        /// The number of spaces per indent level. Maps to the editorconfig
        /// <c>indent_size</c> key. Ignored when <see cref="IndentWithTabs"/> is set.
        /// </summary>
        public int IndentSize { get; set; } = 4;
    }
}
