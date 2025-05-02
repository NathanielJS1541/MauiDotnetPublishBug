using System.Reflection;
using System.Text.RegularExpressions;
using MauiDotnetPublishBug.Common.Resources;

namespace MauiDotnetPublishBug.Resources
{
    /// <inheritdoc cref="IResourceManager"/>
    public sealed class ResourceManager : IResourceManager
    {
        #region Constants

        #region Private

        /// <summary>
        /// <see cref="Regex"/> pattern to split the baseName of the resource file
        /// (i.e. the namespace) and stringName (i.e. the string key within the resource file).
        /// <para>
        /// Stores the <c>baseName</c> and <c>stringName</c> in named capture groups of the same
        /// name.
        /// </para>
        /// </summary>
        private const string StringResourceNamePattern =
            @"^(?<baseName>[\w\.]+)\.(?<stringName>\w+)$";

        #endregion

        #endregion

        #region Fields

        #region Private

        /// <summary>
        /// The <see cref="Assembly"/> that contains the resources.
        /// </summary>
        private readonly Assembly _resourceAssembly;

        #endregion

        #endregion

        #region Construction

        public ResourceManager()
        {
            // In this simple app, all resources are in the same assembly, alongside the
            // ResourceManager.
            _resourceAssembly = GetType().GetTypeInfo().Assembly;
        }

        #endregion

        #region Methods

        #region Public

        /// <inheritdoc />
        public ImageSource? GetImageResource(string resourceName)
        {
            return ImageSource.FromResource(resourceName, _resourceAssembly);
        }

        /// <inheritdoc />
        public string? GetLocalisedString(string resourceName)
        {
            // Guard against invalid string resourceNames.
            if (Regex.Match(resourceName, StringResourceNamePattern) is not { } resourceMatch
                || resourceMatch.Success == false)
            {
                // Return null if the resourceName is invalid.
                return null;
            }

            // Get a ResourceManager for specified basename within the _resourceAssembly.
            var resourceManager = new System.Resources.ResourceManager(
                resourceMatch.Groups["baseName"].Value,
                _resourceAssembly);

            // Safely fetch the localised string from the ResourceManager.
            string? localisedString;
            try
            {
                // Try and get the specified string from the ResourceManager.
                localisedString = resourceManager.GetString(resourceMatch.Groups["stringName"].Value);
            }
            catch
            {
                // Return null if any exception was thrown, as the resource could not be found.
                localisedString = null;
            }

            // Return the localised string, or null if one was not found.
            return localisedString;
        }

        #endregion

        #endregion
    }
}
