namespace MauiDotnetPublishBug.Common.Resources
{
    /// <summary>
    /// Service used to access localized strings and common image resources.
    /// </summary>
    public interface IResourceManager
    {
        #region Methods

        #region Public

        /// <summary>
        /// Retrieves a common <see cref="ImageSource"/> using its fully qualified name.
        /// </summary>
        /// <param name="fullName">The fully qualified name of the image resource.</param>
        /// <returns>
        /// The <see cref="ImageSource"/> if found, otherwise <see langword="null"/>.
        /// </returns>
        public ImageSource? GetImageResource(string fullName);

        /// <summary>
        /// Retrieves a localized string for the specified resource name.
        /// </summary>
        /// <param name="resourceName">The name of the string resource.</param>
        /// <returns>
        /// The localized string if found, otherwise <see langword="null"/>.
        /// </returns>
        public string? GetLocalisedString(string resourceName);

        #endregion

        #endregion
    }
}
