# MauiDotnetPublishBug

A simple app with a GitHub action / Azure pipeline to reproduce
dotnet/maui#29004.

The Action and pipeline don't deploy the build artifacts anywhere, or do
anything fancy for that matter, since the bug being reproduced occurs during the
`dotnet publish` / `dotnet build` step in the action. Any other contents in the
action would just add noise.

Because I needed to add multiple projects to reproduce the bug, I've had to add
some (slightly ridiculous) classes and usage examples to each project. The uses
of the projects and general codebase structure is
[explained in more detail below](#codebase-structure).

> [!IMPORTANT]
> I don't seem to be able to reproduce the bug using GitHub workflows, at least
> not with the `macos-15` runners. We use `macos-15` runners in our Azure
> Pipelines (where I am able to reliably reproduce the issue), but I believe
> that is equivalent to a `macos-15-large` runner in a GitHub workflow.

<!-- omit from toc -->
## Contents

- [Bug Details](#bug-details)
- [Workaround](#workaround)
- [Codebase Structure](#codebase-structure)
  - [MauiDotnetPublishBug.App](#mauidotnetpublishbugapp)
  - [MauiDotnetPublishBug.Common](#mauidotnetpublishbugcommon)
  - [MauiDotnetPublishBug.Controls](#mauidotnetpublishbugcontrols)
  - [MauiDotnetPublishBug.Core](#mauidotnetpublishbugcore)
  - [MauiDotnetPublishBug.Resources](#mauidotnetpublishbugresources)
- [Azure Pipeline File](#azure-pipeline-file)
  - [Pipeline Variables](#pipeline-variables)
  - [Secure Files](#secure-files)
- [Binlogs](#binlogs)
- [Licenses and Attributions](#licenses-and-attributions)

## Bug Details

I first discovered the bug when I had to update a couple of things all at once
with our app / pipeline:

- macOS runner image: [20250331.1080](https://github.com/actions/runner-images/releases/tag/macos-15%2F20250331.1080) -> [20250408.1132](https://github.com/actions/runner-images/releases/tag/macos-15%2F20250408.1132).
  As we use Microsoft-Hosted agents, this is out of our control.
- .NET MAUI version (`MauiVersion`): `9.0.50` -> `9.0.60`. We needed to do this
  to resolve some bugs in our app.
- `Xcode` version: `16.2` -> `16.3`. This was needed for the new .NET MAUI
  version.

Something about this combination caused `dotnet publish` to fail with lots of
`System.IO.IOException`s when building on our Azure Pipeline:

```text
iOS Pipeline:
/Users/runner/.nuget/packages/microsoft.maui.controls.build.tasks/9.0.60/buildTransitive/netstandard2.0/Microsoft.Maui.Controls.targets(172,3): error : The process cannot access the file '/Users/runner/work/1/s/src/MyApp.Controls/obj/Release/net9.0-ios/MyApp.Controls.pdb' because it is being used by another process.

Android Pipeline:
/Users/runner/hostedtoolcache/dotnet/packs/Microsoft.Android.Sdk.Darwin/35.0.50/tools/Xamarin.Android.EmbeddedResource.targets(39,5): error XARLP7024: System.IO.IOException: The process cannot access the file '/Users/runner/work/1/s/src/Tools/MyApp.Tools/obj/Release/net9.0-android/lp/165/jl/res/anim/design_bottom_sheet_slide_out.xml' because it is being used by another process. [/Users/runner/work/1/s/src/Tools/MyApp.Tools/MyApp.Tools.csproj::TargetFramework=net9.0-android]
```

This repo was set up as a way to try and narrow down the bug on a much smaller
project, and help to pin down the issue.

It may be worth noting that our app contains multiple projects to help
[structure our codebase](#codebase-structure). I've replicated this here as I
believe it is relevant.

## Workaround

I found that to work around the error, you can pass the `-m:1` flag as the
_first_ argument to `dotnet publish`. This limits `dotnet publish` to a single
process and resolves issues where multiple processes try and access the same
file at once.

## Codebase Structure

This repo has been set up to try and mimic the structure of a production app I
maintain, as I believe it's relevant to the bug. If some of these projects are
superfluous, you'd be absolutely right at this scale... Some of these projects
and even source files are a bit of a stretch... But try and suspend your
disbelief! We have good reasons for structuring a project like this for our
larger app, which I will try and describe below.

Our app is split across multiple projects, in a similar structure to this:

```text
 📁 src
 ├─ 📁 MauiDotnetPublishBug.App
 │  ├─ 📁 Platforms
 │  │  ├─ 📁 Android
 │  │  └─ 📁 iOS
 │  ├─ 📁 Resources
 │  ├─ 📁 Views
 │  │  ├─ 🗎 MainPage.xaml
 │  │  └─ 🗎 MainPage.xaml.cs
 │  ├─ 🗎 App.xaml
 │  ├─ 🗎 App.xaml.cs
 │  ├─ 🗎 AppShell.xaml
 │  ├─ 🗎 AppShell.xaml.cs
 │  ├─ 🗎 MauiDotnetPublishBug.App.csproj
 │  └─ 🗎 MauiProgram.cs
 ├─ 📁 MauiDotnetPublishBug.Common
 │  ├─ 📁 Resources
 │  │  └─ 🗎 IResourceManager.cs
 │  ├─ 📁 Settings
 │  │  └─ 🗎 ISettingsManager.cs
 │  └─ 🗎 MauiDotnetPublishBug.Common.csproj
 ├─ 📁 MauiDotnetPublishBug.Controls
 │  ├─ 📁 Converters
 │  │  ├─ 🗎 ContrastingTextColourConverter.cs
 │  │  └─ 🗎 InvertedColourConverter.cs
 │  ├─ 🗎 ColourPreviewCard.xaml
 │  ├─ 🗎 ColourPreviewCard.xaml.cs
 │  └─ 🗎 MauiDotnetPublishBug.Controls.csproj
 ├─ 📁 MauiDotnetPublishBug.Core
 │  ├─ 📁 Settings
 │  │  └─ 🗎 SettingsManager.cs
 │  └─ 🗎 MauiDotnetPublishBug.Core.csproj
 ├─ 📁 MauiDotnetPublishBug.Resources
 │  ├─ 📁 Icons
 │  ├─ 📁 Strings
 │  ├─ 🗎 MauiDotnetPublishBug.Resources.csproj
 │  └─ 🗎 ResourceManager.cs
 └─ 🗎 MauiDotnetPublishBug.sln
```

**Note: Some items are omitted for brevity.**

Each project will be explained below:

### MauiDotnetPublishBug.App

This project is our app shell. This is the "deployable" project which contains
all of the MAUI-specific components.

The intention with this is that another project could implement a console app or
test client, and still access all of the logic contained within separate
projects to run tests etc.

This contains all our `Views` (and in our production app, all our `ViewModels`).

### MauiDotnetPublishBug.Common

The common project contains common components that can be used in all other
projects. This can be useful for interfaces for dependency-injected services,
which may be specific to a particular framework, or may be implemented in
another project that can't be directly referenced.

As mentioned above, a different framework or app implementation may need
different service implementations which would need to be called from common
code. A common interface would ensure this can be referenced by both app
implementations.

**This project must not reference any others!**

### MauiDotnetPublishBug.Controls

The controls project contains controls that can be shared between app
implementations. This may contain common UI components, behaviours, converters,
etc.

### MauiDotnetPublishBug.Core

The core project contains implementations of our core services that are
framework and app independent. The idea here is that these services can be used
by multiple app implementations to provide consistent behaviour.

### MauiDotnetPublishBug.Resources

The resources project contains localised animations, icons, images, strings,
themes, etc. that are not specific to a particular framework or app shell. This
would allow us to have consistency between different app implementations, and
share resources like translated strings between multiple projects.

## Azure Pipeline File

I've included [azure-pipelines.yml](./azure-pipelines.yml), which is a
(massively) trimmed down version of our pipeline configuration. This should
allow you to reproduce the bug using an azure pipeline. However, you will need
to update a couple of things first.

### Pipeline Variables

You will need to define the following pipeline variables:
- `android-key-alias`: The alias name of the key within your keystore file that
  will be used to sign the app.
- `android-keystore-password`: The password to your keystore file. This should
  be configured as a secret.
- `ios-p12-password`: The password to your `.p12` certificate. This should be
  configured as a secret.

**The names of these variables must match exactly.**

### Secure Files

You will also need to upload the following files to the secure files library:
- `MyApp.keystore`: The Android keystore file used to sign the Android app.
- `MyApp.mobileprovision`: The iOS provisioning profile for the app.
- `MyApp.p12`: The iOS signing certificate to sign the app with.

**These can be renamed, but you should ensure to also update the following names
configured in the pipeline:**

```yml
variables:
  ...
  # Define secure file names.
  keystoreFile: 'MyApp.keystore' # Android keystore file name in secure files.
  provisioningProfile: 'MyApp.mobileprovision' # Provisioning profile name in secure files.
  appleCertificate: 'MyApp.p12' # Apple signing certificate name in secure files.
```

## Binlogs

Both the [GitHub workflow](.github/workflows/ci-build.yml) and
[azure pipeline](./azure-pipelines.yml) are configured to capture binlogs during
the `dotnet publish` / `dotnet build` steps. These will then be uploaded with
the rest of the artifacts.

## Licenses and Attributions

This project uses icons from the
[Google Material Icons](https://fonts.google.com/icons) collection, which are
licensed under the
[Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0).
