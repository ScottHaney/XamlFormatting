using System;
using System.Collections.Generic;

namespace XamlFormatting.Core
{
    /// <summary>
    /// The end-of-line marker a formatted document should use.
    /// Maps to the editorconfig <c>end_of_line</c> key.
    /// </summary>
    public enum EndOfLineMarker
    {
        /// <summary>Line feed only (<c>\n</c>) — editorconfig <c>lf</c>.</summary>
        Lf,

        /// <summary>Carriage return only (<c>\r</c>) — editorconfig <c>cr</c>.</summary>
        Cr,

        /// <summary>Carriage return + line feed (<c>\r\n</c>) — editorconfig <c>crlf</c>.</summary>
        CrLf,
    }

    /// <summary>
    /// Settings that control how a XAML document is formatted.
    /// </summary>
    /// <remarks>
    /// The strongly-typed properties cover the standard editorconfig keys that affect
    /// XAML output. XAML-specific keys follow the <see cref="XamlKeyPrefix"/> naming
    /// convention (e.g. <c>xaml_max_attributes_per_line</c>) and are surfaced as raw
    /// strings in <see cref="XamlProperties"/>; step 3 maps individual <c>xaml_*</c> keys
    /// to typed behavior as the formatter starts consuming them.
    /// </remarks>
    public sealed class XamlFormatterOptions
    {
        /// <summary>
        /// Prefix that marks XAML-specific editorconfig keys handled by this formatter.
        /// </summary>
        public const string XamlKeyPrefix = "xaml_";

        private static readonly IReadOnlyDictionary<string, string> EmptyProperties =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
        /// The number of columns per indent level. Maps to the editorconfig
        /// <c>indent_size</c> key. Ignored when <see cref="IndentWithTabs"/> is set.
        /// </summary>
        public int IndentSize { get; set; } = 4;

        /// <summary>
        /// The width of a tab character in columns. Maps to the editorconfig
        /// <c>tab_width</c> key. When unset in editorconfig it defaults to
        /// <see cref="IndentSize"/>.
        /// </summary>
        public int TabWidth { get; set; } = 4;

        /// <summary>
        /// The end-of-line marker to emit. Maps to the editorconfig <c>end_of_line</c> key.
        /// </summary>
        public EndOfLineMarker EndOfLine { get; set; } = EndOfLineMarker.CrLf;

        /// <summary>
        /// Whether to ensure the document ends with a single newline. Maps to the
        /// editorconfig <c>insert_final_newline</c> key.
        /// </summary>
        public bool InsertFinalNewline { get; set; }

        /// <summary>
        /// Whether to strip trailing whitespace from each line. Maps to the editorconfig
        /// <c>trim_trailing_whitespace</c> key.
        /// </summary>
        public bool TrimTrailingWhitespace { get; set; }

        /// <summary>
        /// The soft column limit used when deciding how to wrap attributes. Maps to the
        /// editorconfig <c>max_line_length</c> key. A value of <c>0</c> means "no limit"
        /// (editorconfig <c>off</c>).
        /// </summary>
        public int MaxLineLength { get; set; }

        /// <summary>
        /// Raw XAML-specific editorconfig values (keys beginning with
        /// <see cref="XamlKeyPrefix"/>), with the prefix retained and case-insensitive
        /// lookup. Step 3 reads typed values from here as features are implemented.
        /// </summary>
        public IReadOnlyDictionary<string, string> XamlProperties { get; set; } = EmptyProperties;
    }
}
