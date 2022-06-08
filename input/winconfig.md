---
Order: 110
Title: Boxstarter WinConfig Features
---

# Boxstarter WinConfig Features

Boxstarter provides several commands for customizing the Windows experience.

Below are the commands that Boxstarter makes available to all packages run from within Boxstarter:

## Disable-InternetExplorerESC

Turns off Internet Explorer Enhanced Security Configuration that is on by default on Server OS versions.

## Disable-GameBarTips

Turns off the GameBar Tips of Windows 10 that are shown when a game - or what Windows 10 thinks is a game - is launched.

## Disable-MicrosoftUpdate

Turns off the Windows Update option to include updates for other Microsoft products installed on the system.

## Disable-UAC

Disables UAC. Note that Windows 8 and 8.1 can not launch Windows Store applications with UAC disabled.

## Disable-BingSearch

Disables the Bing Internet Search when searching from the search field in the Taskbar or Start Menu.

## Enable-RemoteDesktop

Allows Remote Desktop access to machine and enables Remote Desktop firewall rule.

## Enable-MicrosoftUpdate

Turns on the Windows Update option to include updates for other Microsoft products installed on the system.

## Enable-UAC

Enables UAC.

## Install-WindowsUpdate

Finds, downloads and installs all Windows Updates. By default, only critical updates will be searched. However the command takes a `-Criteria` argument allowing one to pass a custom Windows Update query.

For details about the `Install-WindowsUpdate` command, run:

```powershell
Help Install-WindowsUpdate -Full
```

## Move-LibraryDirectory

Libraries are special folders that map to a specific location on disk. These are usually found somewhere under $env:userprofile. This function can be used to redirect the library folder to a new location on disk. If the new location does not already exist, the directory will be created. Any content in the former library directory will be moved to the new location unless the DoNotMoveOldContent switch is used. Use Get-LibraryNames to discover the names of different libraries and their current physical directories.

```powershell
Move-LibraryDirectory "Personal" "$env:UserProfile\skydrive\documents"
```

This moves the Personal library (aka Documents) to the documents folder off of the default SkyDrive directory.

## Set-StartScreenOptions

Sets options for the Start Screen in Windows 8/8.1.

```powershell
Set-StartScreenOptions -EnableBootToDesktop -EnableDesktopBackgroundOnStart -EnableShowStartOnActiveScreen -EnableShowAppsViewOnStartScreen -EnableSearchEverywhereInAppsView -EnableListDesktopAppsFirst
```

It is also possible to do the converse actions, if required.

```powershell
Set-StartScreenOptions -DisableBootToDesktop -DisableDesktopBackgroundOnStart -DisableShowStartOnActiveScreen -DisableShowAppsViewOnStartScreen -DisableSearchEverywhereInAppsView -DisableListDesktopAppsFirst
```

## Set-CornerNavigationOptions

Sets options for the Windows Corner Navigation in Windows 8/8.1.

```powershell
Set-CornerNavigationOptions -EnableUpperRightCornerShowCharms -EnableUpperLeftCornerSwitchApps -EnableUsePowerShellOnWinX
```

It is also possible to do the converse actions, if required.

```powershell
Set-CornerNavigationOptions -DisableUpperRightCornerShowCharms -DisableUpperLeftCornerSwitchApps -DisableUsePowerShellOnWinX
```

## Set-WindowsExplorerOptions

Sets options on the Windows Explorer shell.

```powershell
Set-WindowsExplorerOptions -EnableShowHiddenFilesFoldersDrives -EnableShowProtectedOSFiles -EnableShowFileExtensions -EnableShowFullPathInTitleBar -EnableOpenFileExplorerToQuickAccess -EnableShowRecentFilesInQuickAccess -EnableShowFrequentFoldersInQuickAccess -EnableExpandToOpenFolder -EnableShowRibbon -EnableItemCheckBox
```

It is also possible to do the converse actions, if required.

```powershell
Set-WindowsExplorerOptions -DisableShowHiddenFilesFoldersDrives -DisableShowProtectedOSFiles -DisableShowFileExtensions -DisableShowFullPathInTitleBar -DisableOpenFileExplorerToQuickAccess -DisableShowRecentFilesInQuickAccess -DisableShowFrequentFoldersInQuickAccess -DisableExpandToOpenFolder -DisableShowRibbon -DisableItemCheckBox
```

## Set-BoxstarterTaskbarOptions

Sets options on the Windows Taskbar (formerly called Set-TaskbarOptions).

AlwaysShowIconsOn/AlwaysShowIconsOff allows turning on or off always showing all icons in the notification area.

```powershell
Set-BoxstarterTaskbarOptions -Size Small -Dock Top -Combine Always -AlwaysShowIconsOn -MultiMonitorOn -MultiMonitorMode All -MultiMonitorCombine Always -EnableSearchBox
```

It is also possible to do the converse actions, if required.

```powershell
Set-BoxstarterTaskbarOptions -Size Large -Dock Bottom -Combine Never -AlwaysShowIconsOff -MultiMonitorOff -DisableSearchBox
```

## Update-ExecutionPolicy

The execution policy is set in a separate elevated PowerShell process. If running in the Chocolatey runner, the current window cannot be used because its execution policy has been explicitly set.

If on a 64 bit machine, the policy will be set for both 64 and 32 bit shells.
