using System;
using System.Collections.Generic;
using System.Globalization;

namespace XamlFormatting.Core.EditorConfig
{
    /// <summary>
    /// Tolerant parsing of editorconfig string values into typed values. Shared by the
    /// standard-key mapping and (later) the <c>xaml_*</c> key mapping in step 3.
    /// </summary>
    /// <remarks>
    /// EditorConfig values are case-insensitive for known keys, and the special value
    /// <c>unset</c> means "no value" — both are handled here. Lookups are case-insensitive
    /// on the key as well, matching the spec's case-insensitive key handling.
    /// </remarks>
    public static class EditorConfigValueParser
    {
        /// <summary>The editorconfig sentinel meaning a property was explicitly unset.</summary>
        public const string UnsetValue = "unset";

        /// <summary>
        /// Gets the raw value for <paramref name="key"/>, treating missing, empty, and
        /// <c>unset</c> values as "not present".
        /// </summary>
        public static bool TryGetValue(IReadOnlyDictionary<string, string> properties, string key, out string value)
        {
            if (properties is null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (properties.TryGetValue(key, out string? raw)
                && !string.IsNullOrEmpty(raw)
                && !string.Equals(raw, UnsetValue, StringComparison.OrdinalIgnoreCase))
            {
                value = raw!;
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>Parses a boolean (<c>true</c>/<c>false</c>) editorconfig value.</summary>
        public static bool? GetBool(IReadOnlyDictionary<string, string> properties, string key)
        {
            if (!TryGetValue(properties, key, out string value))
            {
                return null;
            }

            if (string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return null;
        }

        /// <summary>Parses a non-negative integer editorconfig value.</summary>
        public static int? GetInt(IReadOnlyDictionary<string, string> properties, string key)
        {
            if (!TryGetValue(properties, key, out string value))
            {
                return null;
            }

            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
                ? result
                : (int?)null;
        }

        /// <summary>
        /// Parses an enum editorconfig value case-insensitively, restricted to the
        /// supplied name→value map (so only spec-valid tokens are accepted).
        /// </summary>
        public static T? GetEnum<T>(
            IReadOnlyDictionary<string, string> properties,
            string key,
            IReadOnlyDictionary<string, T> valuesByName)
            where T : struct
        {
            if (valuesByName is null)
            {
                throw new ArgumentNullException(nameof(valuesByName));
            }

            if (!TryGetValue(properties, key, out string value))
            {
                return null;
            }

            return valuesByName.TryGetValue(value, out T result) ? result : (T?)null;
        }
    }
}
