---
Order: 70
Title: Pushing Packages
---

# Publishing Boxstarter Packages

How to make your packages discoverable to Boxstarter and possibly share them with others

There are several options available as to where to publish your packages:

- Keep them in your local Boxstarter repository
- Push them to a public NuGet feed like [Chocolatey.org](https://community.chocolatey.org/packages), or any other public feed
- Push to a private NuGet feed
- Keep packages on a network share

## Where does Boxstarter look for Packages

Boxstarter can be configured to look anywhere for packages. By default, Boxstarter checks the following locations in this order:

- The Boxstarter Local Repository `$Boxstarter.LocalRepo` where packages are saved using `New-BoxstarterPackage`
- The [Chocolatey.org](https://community.chocolatey.org/packages) public feed

The first location that contains a package name that Boxstarter is looking for will be the last location Boxstarter probes. So, for example, if the local repository and the Chocolatey community feed have a package with the same name, the package in the local repository will be chosen.

## Configuring where Boxstarter looks for packages

You can change the directory that Boxstarter considers the local repository or change the feeds that Boxstarter searches using the `Set-BoxstarterConfig` command.

```powershell
Set-BoxstarterConfig -LocalRepo "\\server\share\myRepo"
```

This will cause Boxstarter to search for packages in \\server\share\myRepo

```powershell
Set-BoxstarterConfig -NugetSources "https://www.myget.org/F/MyNugetFeed/api/v2"
```

This will cause Boxstarter to search for packages in the local repository and then in your MyNugetFeed hosted by MyGet.org. No other feed will be searched.

```powershell
Set-BoxstarterConfig -NugetSources "https://www.myget.org/F/MyNugetFeed/api/v2;https://chocolatey.org/api/v2"
```

This will cause Boxstarter to search for packages in the local repository and then in your MyNugetFeed followed by the Chocolatey.org feed.

## Pushing packages to a feed

Boxstarter does not provide any functionality for publishing packages to a NuGet feed. However, you can use NuGet.exe which does provide this capability. You can obtain the NuGet.exe command line interface by installing the nuget.commandline package. Calling the NuGet command line with the [Push](https://docs.nuget.org/docs/reference/command-line-reference#Push_Command) command and the feed URL will publish your package to that feed.

```powershell
nuget push MyPackage.1.0.0.nupkg <Your MyGet API Key> -Source https://www.myget.org/F/MyFeed/api/v2/package
```

If you want to publish to the Chocolatey community feed, Chocolatey has a command, [choco push](https://docs.chocolatey.org/en-us/create/commands/push), which will publish your package to Chocolatey without needing to specify the feed URL.

```powershell
choco push MyPackage.1.0.0.nupkg <Your Chocolatey API Key>
```

All NuGet feed hosts require package authors to provide a key when publishing packages. This identifies you as the actual package owner. See the host's documentation for information on how to obtain a key.