using System.Globalization;
using MauiDotnetPublishBug.Common.Settings;

namespace MauiDotnetPublishBug.Core.Settings
{
    /// <inheritdoc cref="ISettingsManager"/>
    public sealed class SettingsManager : ISettingsManager
    {
        #region Constants

        #region Private

        /// <summary>
        /// Use the "round-trip" format specifier to store a <see cref="DateTime"/> as a string
        /// that complies with ISO 8601.
        /// <see langword="true"/>
        /// </summary>
        private const string ColourGenerationTimeFormat = @"o";

        /// <summary>
        /// The <see cref="string"/> used as a key to store and retrieve the time that the random
        /// <see cref="Color"/> was generated at.
        /// </summary>
        private const string ColourGenerationTimeKey = @"colourgenerationtime";

        /// <summary>
        /// The <see cref="string"/> used as a key to store and retrieve the randomly generated
        /// <see cref="Color"/>.
        /// </summary>
        private const string RandomColourKey = @"randomcolour";

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <inheritdoc />
        public void Clear()
        {
            // Clear all keys and values in the underlying key/value store.
            Preferences.Clear();
        }

        /// <inheritdoc />
        public DateTime GetColourGenerationTime()
        {
            // Get the string representation of the DateTime.
            var settingValue = Preferences.Get(ColourGenerationTimeKey, string.Empty);

            // Parse the DateTime string if it was retrieved, otherwise fall back to the default
            // value.
            //
            // InvariantCulture is used to ensure the same format is used every time, since it is
            // not directly displayed on the UI.
            return string.IsNullOrWhiteSpace(settingValue)
                ? DateTime.MinValue
                : DateTime.ParseExact(settingValue, ColourGenerationTimeFormat, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public Color? GetRandomColour()
        {
            // Get the string representation of the colour (hex RGBA).
            var settingValue = Preferences.Get(RandomColourKey, string.Empty);

            // Parse the colour string if it was retrieved, otherwise fall back to the default
            // value.
            return string.IsNullOrWhiteSpace(settingValue)
                ? null
                : Color.FromRgba(settingValue);
        }

        /// <inheritdoc />
        public void SetColourGenerationTime(DateTime generationTime)
        {
            // Convert the DateTime to a string to be stored as a key/value pair.
            //
            // InvariantCulture is used to ensure the same format is used every time, since it is
            // not directly displayed on the UI.
            Preferences.Set(ColourGenerationTimeKey, generationTime.ToString(ColourGenerationTimeFormat, CultureInfo.InvariantCulture));
        }

        /// <inheritdoc />
        public void SetRandomColour(Color colour)
        {
            // Convert the colour to an RGBA hex string to be stored as a key/value pair.
            Preferences.Set(RandomColourKey, colour.ToRgbaHex(true));
        }

        #endregion

        #endregion
    }
}
