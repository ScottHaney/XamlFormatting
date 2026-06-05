using System;

namespace XamlFormatting.Core
{
    /// <summary>
    /// Entry point for formatting XAML documents.
    /// </summary>
    /// <remarks>
    /// This is the scaffold introduced in step 1. The real behavior arrives in
    /// later steps:
    /// <list type="bullet">
    ///   <item><description>Step 2 resolves <see cref="XamlFormatterOptions"/> from the
    ///   nearest <c>.editorconfig</c> chain for the document being formatted.</description></item>
    ///   <item><description>Step 3 implements the actual XAML formatting logic
    ///   (attribute ordering, indentation, line wrapping, etc.).</description></item>
    /// </list>
    /// Until then <see cref="Format(string, XamlFormatterOptions)"/> returns the input
    /// unchanged so callers (and tests) have a stable, well-defined contract to build on.
    /// </remarks>
    public sealed class XamlFormatter
    {
        /// <summary>
        /// Formats the supplied XAML source.
        /// </summary>
        /// <param name="xaml">The XAML document contents to format.</param>
        /// <param name="options">
        /// The formatting options to apply. When <see langword="null"/>, the default
        /// options are used. In later steps these are typically produced from an
        /// <c>.editorconfig</c> lookup.
        /// </param>
        /// <returns>The formatted XAML.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="xaml"/> is <see langword="null"/>.</exception>
        public string Format(string xaml, XamlFormatterOptions? options = null)
        {
            if (xaml is null)
            {
                throw new ArgumentNullException(nameof(xaml));
            }

            _ = options ?? XamlFormatterOptions.Default;

            // TODO (step 3): implement real formatting. For now the formatter is a
            // pass-through so the rest of the system can be wired up and tested.
            return xaml;
        }
    }
}
