using System.Globalization;

namespace MauiDotnetPublishBug.Controls.Converters
{
    /// <summary>
    /// <see cref="IValueConverter"/> to invert a <see cref="Color"/>.
    /// </summary>
    public sealed class InvertedColourConverter : IValueConverter
    {
        #region Constants

        #region Private

        /// <summary>
        /// The maximum value for each <see cref="Color"/> channel.
        /// </summary>
        private const float MaxChannelValue = 1f;

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Invert a <see cref="Color"/>. Note that this retains the
        /// <see cref="Color.Alpha"/> channel value.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to convert.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">Ignored.</param>
        /// <returns>
        /// The complementary <see cref="Color"/> to the input value, or <c>null</c> if the input
        /// value was not a <see cref="Color"/>.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Color colour)
            {
                // If the input value is not a colour, return null.
                return null;
            }

            // Convert the colour by inverting the values in the red, green and blue channels. The
            // alpha channel value is not changed.
            //
            // Note that the channel values are returned as floats between 0 and 1.
            return Color.FromRgba(
                MaxChannelValue - colour.Red,
                MaxChannelValue - colour.Green,
                MaxChannelValue - colour.Blue,
                colour.Alpha);
        }

        /// <inheritdoc cref="Convert"/>.
        /// <remarks>
        /// The process of inverting <see cref="Color"/> is reversible, so this just wraps the
        /// <see cref="Convert"/> method. <see cref="Convert"/> should be called directly if
        /// possible.
        /// </remarks>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Just call the Convert() method, as this is a reversible process.
            return Convert(value, targetType, parameter, culture);
        }

        #endregion

        #endregion
    }
}
