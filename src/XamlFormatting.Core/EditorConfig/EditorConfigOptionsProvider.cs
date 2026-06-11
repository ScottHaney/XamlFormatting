using System;
using System.Collections.Generic;
using EditorConfig.Core;

namespace XamlFormatting.Core.EditorConfig
{
    /// <summary>
    /// Resolves <see cref="XamlFormatterOptions"/> for a document by reading the
    /// applicable <c>.editorconfig</c> settings.
    /// </summary>
    /// <remarks>
    /// Discovery, glob matching, and cascade/merge are delegated to editorconfig-core-net
    /// (<see cref="EditorConfigParser"/>): given a file path it walks up the directory tree
    /// to the nearest <c>root = true</c>, matches sections, and produces a single flat
    /// property set. This class maps that property set onto the formatter's options.
    /// </remarks>
    public sealed class EditorConfigOptionsProvider
    {
        private static readonly IReadOnlyDictionary<string, EndOfLineMarker> EndOfLineValues =
            new Dictionary<string, EndOfLineMarker>(StringComparer.OrdinalIgnoreCase)
            {
                ["lf"] = EndOfLineMarker.Lf,
                ["cr"] = EndOfLineMarker.Cr,
                ["crlf"] = EndOfLineMarker.CrLf,
            };

        private readonly EditorConfigParser _parser = new EditorConfigParser();

        /// <summary>
        /// Resolves options for the document at <paramref name="filePath"/> from the
        /// applicable <c>.editorconfig</c> chain. Keys not specified fall back to the
        /// <see cref="XamlFormatterOptions"/> defaults.
        /// </summary>
        /// <param name="filePath">Absolute path of the document being formatted.</param>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null or empty.</exception>
        public XamlFormatterOptions GetOptionsForFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("A non-empty file path is required.", nameof(filePath));
            }

            FileConfiguration configuration = _parser.Parse(filePath);
            return GetOptions(configuration.Properties);
        }

        /// <summary>
        /// Maps an already-resolved editorconfig property set onto options. Exposed
        /// separately so the mapping can be tested without touching the file system.
        /// </summary>
        /// <param name="properties">Merged editorconfig properties for a single file.</param>
        public XamlFormatterOptions GetOptions(IReadOnlyDictionary<string, string> properties)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            // Case-insensitive view so lookups don't depend on how the source dictionary
            // was keyed (the editorconfig spec treats keys case-insensitively).
            var props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in properties)
            {
                props[pair.Key] = pair.Value;
            }

            var options = new XamlFormatterOptions();

            MapIndentation(props, options);

            EndOfLineMarker? endOfLine = EditorConfigValueParser.GetEnum(props, "end_of_line", EndOfLineValues);
            if (endOfLine.HasValue)
            {
                options.EndOfLine = endOfLine.Value;
            }

            bool? insertFinalNewline = EditorConfigValueParser.GetBool(props, "insert_final_newline");
            if (insertFinalNewline.HasValue)
            {
                options.InsertFinalNewline = insertFinalNewline.Value;
            }

            bool? trimTrailing = EditorConfigValueParser.GetBool(props, "trim_trailing_whitespace");
            if (trimTrailing.HasValue)
            {
                options.TrimTrailingWhitespace = trimTrailing.Value;
            }

            MapMaxLineLength(props, options);

            options.XamlProperties = ExtractXamlProperties(props);

            return options;
        }

        private static void MapIndentation(IReadOnlyDictionary<string, string> props, XamlFormatterOptions options)
        {
            if (EditorConfigValueParser.TryGetValue(props, "indent_style", out string indentStyle))
            {
                options.IndentWithTabs = string.Equals(indentStyle, "tab", StringComparison.OrdinalIgnoreCase);
            }

            int? tabWidth = EditorConfigValueParser.GetInt(props, "tab_width");

            // indent_size may be an integer or the special value "tab" (follow tab_width).
            int? indentSize = null;
            bool indentSizeFollowsTab = false;
            if (EditorConfigValueParser.TryGetValue(props, "indent_size", out string indentSizeValue))
            {
                if (string.Equals(indentSizeValue, "tab", StringComparison.OrdinalIgnoreCase))
                {
                    indentSizeFollowsTab = true;
                }
                else
                {
                    indentSize = EditorConfigValueParser.GetInt(props, "indent_size");
                }
            }

            if (indentSize.HasValue)
            {
                options.IndentSize = indentSize.Value;
            }

            // Per spec: tab_width defaults to indent_size when unset.
            if (tabWidth.HasValue)
            {
                options.TabWidth = tabWidth.Value;
            }
            else if (indentSize.HasValue)
            {
                options.TabWidth = indentSize.Value;
            }

            // Per spec: indent_size = tab means indent width follows tab_width.
            if (indentSizeFollowsTab)
            {
                options.IndentSize = options.TabWidth;
            }
        }

        private static void MapMaxLineLength(IReadOnlyDictionary<string, string> props, XamlFormatterOptions options)
        {
            if (!EditorConfigValueParser.TryGetValue(props, "max_line_length", out string value))
            {
                return;
            }

            if (string.Equals(value, "off", StringComparison.OrdinalIgnoreCase))
            {
                options.MaxLineLength = 0;
                return;
            }

            int? max = EditorConfigValueParser.GetInt(props, "max_line_length");
            if (max.HasValue)
            {
                options.MaxLineLength = max.Value;
            }
        }

        private static IReadOnlyDictionary<string, string> ExtractXamlProperties(IReadOnlyDictionary<string, string> props)
        {
            var xaml = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<string, string> pair in props)
            {
                if (pair.Key.StartsWith(XamlFormatterOptions.XamlKeyPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    xaml[pair.Key] = pair.Value;
                }
            }

            return xaml;
        }
    }
}
