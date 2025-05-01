using System.Globalization;
using MauiDotnetPublishBug.Common.Resources;
using MauiDotnetPublishBug.Common.Settings;

namespace MauiDotnetPublishBug.Views
{
    public partial class MainPage : ContentPage
    {
        #region Constants

        #region Private

        /// <summary>
        /// The format string used to convert a <see cref="DateTime"/> to the
        /// <see cref="ColourGenerationTimeLabel"/>.
        /// </summary>
        private const string ColourGenerationTimeFormat = @"yyyy/MM/dd HH:mm:ss.fff";

        /// <summary>
        /// Resource name for the localised string for the
        /// <see cref="ColourGenerationTimeHeadingLabel"/>.
        /// </summary>
        private const string ColourGenerationTimeHeadingLabelResourceName =
            @"MauiDotnetPublishBug.Resources.Strings.Common.ColourGenerationTimeLabel";

        /// <summary>
        /// Resource name for the localised string for the <see cref="ColourGenerationTimeLabel"/>
        /// to fall back to when the setting has not been set.
        /// </summary>
        private const string ColourNeverSetResourceName =
            @"MauiDotnetPublishBug.Resources.Strings.Common.Never";

        /// <summary>
        /// Resource name for the <see cref="DeleteIcon"/>.
        /// </summary>
        private const string DeleteImageResourceName =
            @"MauiDotnetPublishBug.Resources.Icons.delete.png";

        /// <summary>
        /// Resource name for the <see cref="DiceIcon"/>.
        /// </summary>
        private const string DiceImageResourceName =
            @"MauiDotnetPublishBug.Resources.Icons.dice.png";

        /// <summary>
        /// Resource name for the localised string for the <see cref="RandomColourLabel"/>.
        /// </summary>
        private const string RandomColourLabelResourceName =
            @"MauiDotnetPublishBug.Resources.Strings.Common.RandomColourLabel";

        /// <summary>
        /// Resource name for the localised string for the <see cref="TitleLabel"/>.
        /// </summary>
        private const string TitleResourceName =
            @"MauiDotnetPublishBug.Resources.Strings.Common.AppDisplayName";

        /// <summary>
        /// The default <see cref="Color"/> for the <see cref="RandomColour"/> to fall back to when
        /// one has not been generated yet.
        /// </summary>
        private static readonly Color s_defaultColor =
            Color.FromRgba(0, 0, 0, 255);

        /// <summary>
        /// PRNG used to generate the <see cref="RandomColour"/>.
        /// </summary>
        /// <remarks>
        /// This is seeded using part of the <see cref="DateTime.Now"/>, at the time of first
        /// access.
        /// </remarks>
        private static readonly Random s_random = new(DateTime.Now.Millisecond);

        #endregion

        #endregion

        #region Fields

        #region Private

        /// <summary>
        /// Instance of the <see cref="IResourceManager"/> service.
        /// </summary>
        private readonly IResourceManager _resourceManager;

        /// <summary>
        /// Instance of the <see cref="ISettingsManager"/> service.
        /// </summary>
        private readonly ISettingsManager _settingsManager;

        #endregion

        #endregion

        #region Construction

        public MainPage(IResourceManager resourceManager, ISettingsManager settingsManager)
        {
            // Store dependency-injected services.
            _resourceManager = resourceManager;
            _settingsManager = settingsManager;

            InitializeComponent();

            // Set the binding context to this to allow bindings to the code-behind.
            BindingContext = this;
        }

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// The <see cref="DateTime"/> at which the <see cref="RandomColour"/> was generated.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DateTime.MinValue"/> if the <see cref="RandomColour"/> has not
        /// been generated yet.
        /// </remarks>
        public DateTime ColourGenerationTime
        {
            get => _settingsManager.GetColourGenerationTime();
            set
            {
                // Store the setting and raise the PropertyChanged event.
                _settingsManager.SetColourGenerationTime(value);
                OnPropertyChanged();

                // Raise the PropertyChanged event for the ColourGenerationTimeLabel, as it is a
                // dependent property.
                OnPropertyChanged(nameof(ColourGenerationTimeLabel));
            }
        }

        /// <summary>
        /// The <see cref="string"/> to display in a label above the time that the
        /// <see cref="RandomColour"/> was generated at.
        /// </summary>
        public string ColourGenerationTimeHeadingLabel =>
            _resourceManager.GetLocalisedString(ColourGenerationTimeHeadingLabelResourceName) ?? string.Empty;

        /// <summary>
        /// A <see cref="string"/> used to display the time that the <see cref="RandomColour"/>
        /// was generated at.
        /// </summary>
        /// <remarks>
        /// Falls back to the localised equivalent of "Never" when the <see cref="RandomColour"/>
        /// has not been generated yet.
        /// </remarks>
        public string ColourGenerationTimeLabel
        {
            get
            {
                var storedTime = ColourGenerationTime;

                // ColourGenerationTime will return DateTime.MinValue if a random colour has not
                // yet been generated. In this case, display a localised string similar to "Never".
                return storedTime != DateTime.MinValue
                    ? storedTime.ToString(ColourGenerationTimeFormat, CultureInfo.CurrentUICulture)
                    : _resourceManager.GetLocalisedString(ColourNeverSetResourceName) ?? string.Empty;
            }
        }

        /// <summary>
        /// The icon displayed on the "delete" or "clear" button.
        /// </summary>
        public ImageSource? DeleteIcon =>
            _resourceManager.GetImageResource(DeleteImageResourceName);

        /// <summary>
        /// The icon displayed on the button used to generate a new <see cref="RandomColour"/>.
        /// </summary>
        public ImageSource? DiceIcon => _resourceManager.GetImageResource(DiceImageResourceName);

        /// <summary>
        /// The randomly generated <see cref="Color"/>.
        /// </summary>
        public Color RandomColour
        {
            get => _settingsManager.GetRandomColour() ?? s_defaultColor;
            set
            {
                // Store the setting and raise the PropertyChanged event.
                _settingsManager.SetRandomColour(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The <see cref="string"/> to display in a label above the random <see cref="Color"/>
        /// preview.
        /// </summary>
        public string RandomColourLabel =>
            _resourceManager.GetLocalisedString(RandomColourLabelResourceName) ?? string.Empty;

        /// <summary>
        /// The <see cref="string"/> to display as the title.
        /// </summary>
        public string TitleLabel =>
            _resourceManager.GetLocalisedString(TitleResourceName) ?? string.Empty;

        #endregion

        #endregion

        #region Methods

        #region Event Handlers

        /// <summary>
        /// Delete all stored settings.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void OnDeleteSettings(object sender, EventArgs e)
        {
            // Clear all stored settings.
            _settingsManager.Clear();

            // Raise the PropertyChanged event for every property that is backed by a setting.
            OnPropertyChanged(nameof(ColourGenerationTime));
            OnPropertyChanged(nameof(ColourGenerationTimeLabel));
            OnPropertyChanged(nameof(RandomColour));
        }

        /// <summary>
        /// Generate a new <see cref="RandomColour"/>.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void OnGenerateColour(object sender, EventArgs e)
        {
            // Update the ColourGenerationTime with the current time.
            ColourGenerationTime = DateTime.Now;

            // Generate a new random colour using the PRNG.
            RandomColour = new Color(
                s_random.NextSingle(), // Red channel.
                s_random.NextSingle(), // Green channel.
                s_random.NextSingle(), // Blue channel.
                1.0f                   // Alpha channel. Force the colour to be fully opaque.
            );
        }

        #endregion

        #endregion
    }

}
