namespace MauiDotnetPublishBug.Controls
{
    /// <summary>
    /// A basic card control to display a preview of a <see cref="Color"/>, along with a readable
    /// hex representation of the <see cref="Colour"/> above it.
    /// </summary>
    public partial class ColourPreviewCard : ContentView
    {
        #region Constants

        #region Private

        /// <summary>
        /// A completely transparent <see cref="Color"/> to use as the default
        /// <see cref="ColourProperty"/> value.
        /// </summary>
        private static readonly Color s_transparency =
            Color.FromRgba(0, 0, 0, 0);

        #endregion

        #region Public

        /// <summary>
        /// The <see cref="BindableProperty"/> for the <see cref="Colour"/>.
        /// </summary>
        public static readonly BindableProperty ColourProperty =
            BindableProperty.Create(
                nameof(Colour),
                typeof(Color),
                typeof(ColourPreviewCard),
                s_transparency,
                BindingMode.Default,
                ValidateColour,
                ColourPropertyChanged);

        #endregion

        #endregion

        #region Construction

        public ColourPreviewCard()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// The background <see cref="Color"/> for the card.
        /// </summary>
        public Color Colour
        {
            get => (Color)GetValue(ColourProperty);
            set => SetValue(ColourProperty, value);
        }

        /// <summary>
        /// The hex representation of the <see cref="Colour"/>.
        /// </summary>
        public string ColourHex => Colour.ToRgbaHex(true);

        #endregion

        #endregion

        #region Methods

        #region Private Static

        /// <summary>
        /// Handles changes to the <see cref="Colour"/> property on a
        /// <see cref="ColourPreviewCard"/>. This triggers a PropertyChanged event for the
        /// <see cref="ColourHex"/> property, as it is derived from the <see cref="Colour"/> value.
        /// </summary>
        /// <param name="bindable">The bindable object where the property changed.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        private static void ColourPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ColourPreviewCard card)
            {
                // Propagate the PropertyChanged event to the derived ColourHex property.
                card.OnPropertyChanged(nameof(ColourHex));
            }
        }

        /// <summary>
        /// Validates that the provided value for the <see cref="ColourProperty"/> is a
        /// <see cref="Color"/>.
        /// </summary>
        /// <param name="bindable">The bindable object the value is being set on.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>
        /// <see langword="true"/> if the value is a <see cref="Color"/>, otherwise
        /// <see langword="false"/>.
        /// </returns>
        private static bool ValidateColour(BindableObject bindable, object value)
        {
            return value is Color;
        }

        #endregion

        #endregion
    }
}