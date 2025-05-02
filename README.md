# MauiDotnetPublishBug

A simple app with a GitHub action to reproduce dotnet/maui#29004.

The Action doesn't deploy the build artifacts anywhere, or do anything fancy for
that matter, since the bug being reproduced occurs during the `dotnet publish`
step in the action. Any other contents in the action would just add noise.

## Bug Details

I first discovered the bug when I had to update a couple of things all at once
with our app / pipeline:

- macOS runner image: [ 20250331.1080](https://github.com/actions/runner-images/releases/tag/macos-15%2F20250331.1080) -> [20250408.1132](https://github.com/actions/runner-images/releases/tag/macos-15%2F20250408.1132).
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
[structure our codebase](#codebase-structure). I've replicated this here in case
it is relevant.

## Workaround

I found that to work around the error, you can pass the `-m:1` flag as the
_first_ argument to `dotnet publish`. This limits `dotnet publish` to a single
process and resolves issues where multiple processes try and access the same
file at once.

## Codebase Structure

This repo has been set up to try and mimic the structure of our app, in case
it's relevant to the bug. The project is structured as follows:

**TODO!**

```text

```
