---
Order: 60
Title: Creating Packages
---

# Creating Packages

Boxstarter includes some commands that can automate a lot of the "grunt" work involved with creating and packing NuGet packages.

## Do you really need to create a package?

Maybe not. For simple "One-Off" packages or packages that do not need a home in a public NuGet feed and do not need any additional scripts or resources, other than, the ChocolateyInstall.ps1 script, Boxstarter provides a means of creating "on the fly" packages from a single script file or URL. You may pass a file path or URL to the -PackageName parameter of `Install-BoxstarterPackage` and Boxstarter will generate a temporary package to install the script. If using a URL, the URL content must be plain text and not contain any markup.

```powershell
Install-BoxstarterPackage -PackageName https://gist.github.com/mwrock/8066325/raw/e0c830528429cd68a8c71dbff6f48298576d8d20/gistfile1.txt
```

## New-PackageFromScript

The simplest way to create a NuGet package is using the `New-PackageFromScript` command.

```powershell
New-PackageFromScript MyScript.ps1 MyPackage
```

This command takes either a file or http URL (like a [GitHub Gist](https://gist.github.com)) that represents the installation script. The `New-PackageFromScript` command will create a Chocolatey NuGet package with the contents of the script moved to the ChocolateyInstall.ps1 file. While this is an incredibly simple way to create a package for your Boxstarter installations, it does not provide much flexibility in customizing the nuspec manifest or adding other files to your installation package.

## New-BoxstarterPackage

The New-BoxstarterPackage command creates a skeletal package with a minimal *.nuspec file and ChocolateyInstall.ps1 file.

```powershell
New-BoxstarterPackage -Name MyPackage -Description "I hope you enjoy MyPackage"
```

This creates a MyPackage.nuspec file that looks like:

```XML
<?xml version="1.0" ?>
<package>
    <metadata>
    <id>MyPackage</id>
    <version>1.0.0</version>
    <authors>Matt</authors>
    <owners>Matt</owners>
    <description>I hope you enjoy MyPackage</description>
    <tags>Boxstarter</tags>
  </metadata>
</package>
```

And a tools/ChocolateyInstall.ps1 with this code:

```powershell
try {
    Write-ChocolateySuccess 'MyPackage'
} catch {
  Write-ChocolateyFailure 'MyPackage' $($_.Exception.Message)
  throw
}
```

## The Boxstarter Local Repository

These files are saved to your local Boxstarter Repository. This is where Boxstarter will store your local packages and running the Install-BoxstarterPackage command will look here first before any external feed. The repository is located in your AppData directory, but you can always find out exactly where by inspecting the global Boxstarter variable `$Boxstarter.LocalRepo`.

![Windows Powershell output from inspecting the global Boxstarter variable $Boxstarter.LocalRepo](https://img.chocolatey.org/boxstarter/global.png)

You can change the location by using the Set-BoxstarterConfig command:

```powershell
Set-BoxstarterConfig -LocalRepo "c:\some\location"
```

## Editing and adding resources to your package

Optionally, you can call the New-BoxstarterPackage command with a path argument and supply a directory that contains files you want to be included in the package:

```powershell
New-BoxstarterPackage -Name MyPackage -Description "I hope you enjoy MyPackage" -Path "c:\somePath"
```

Boxstarter will copy all files at, and below, c:\somepath, and you can refer to these files in your ChocolateyInstall.ps1 using `Get-PackageRoot`.

```powershell
Copy-Item (Join-Path -Path (Get-PackageRoot($MyInvocation)) -ChildPath 'console.xml') -Force $env:appdata\console\console.xml
```

Assuming you have a console.xml file at the root of the path you provided to `New-BoxstarterPackage`, you can access that file from your ChocolateyInstall.ps1 script.

## Composing the ChocolateyInstall Script

If you are already familiar with authoring [Chocolatey package scripts](https://docs.chocolatey.org/en-us/create/create-packages-quick-start), you know how to do this already. The only difference with Boxstarter scripts is that your script also has access to Boxstarter's API for configuring windows, running updates as well as logging and reboot control.

Lets open the ChocolateyInstall.ps1 script that the New-BoxstarterPackage command created for our MyPackage package:

```powershell
Notepad (Join-Path -Path $Boxstarter.LocalRepo -ChildPath "MyPackage\tools\ChocolateyInstall.ps1")
```

Replace the boilerplate code with something that will actually do something:

```powershell
Update-ExecutionPolicy Unrestricted
Move-LibraryDirectory "Personal" "$env:UserProfile\skydrive\documents"
Set-ExplorerOptions -showHiddenFilesFoldersDrives -showProtectedOSFiles -showFileExtensions
Set-TaskbarSmall
Enable-RemoteDesktop

choco install VisualStudio2013ExpressWeb
choco install fiddler4
choco install mssqlserver2012express
choco install git-credential-winstore
choco install console-devel
choco install poshgit
choco install windbg

choco install Microsoft-Hyper-V-All -source windowsFeatures
choco install IIS-WebServerRole -source windowsfeatures
choco install IIS-HttpCompressionDynamic -source windowsfeatures
choco install TelnetClient -source windowsFeatures

Install-ChocolateyPinnedTaskBarItem "$env:windir\system32\mstsc.exe"
Install-ChocolateyPinnedTaskBarItem "$env:programfiles\console\console.exe"

Copy-Item (Join-Path -Path (Get-PackageRoot($MyInvocation)) -ChildPath 'console.xml') -Force $env:appdata\console\console.xml

Install-ChocolateyVsixPackage xunit http://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099/file/66837/1/xunit.runner.visualstudio.vsix
Install-ChocolateyVsixPackage autowrocktestable http://visualstudiogallery.msdn.microsoft.com/ea3a37c9-1c76-4628-803e-b10a109e7943/file/73131/1/AutoWrockTestable.vsix

Install-WindowsUpdate -AcceptEula
```

This script does several things and leverage's both Chocolatey and Boxstarter commands. Here is what this will do:

- Set the PowerShell execution policy to be able to run all scripts
- Move your personal folders to sync with SkyDrive
- Make Windows Explorer tolerable
- Enable Remote Desktop to the box
- Download and install a bunch of software
- Enable Windows Features Hyper-V, IIS and the Telnet client
- Create some shortcuts in the taskbar for common applications
- Copy your console configuration file with your favorite settings
- Install some Visual Studio extensions from the Visual Studio gallery
- Install all critical Windows Updates

## Boxstarter ChocolateyInstall Considerations

Boxstarter can run any Chocolatey package and any valid PowerShell inside that package. However, there are a few things to consider that may make a Boxstarter Chocolatey package a better installation experience.

- Boxstarter Chocolatey packages should be repeatable. This is especially true if you anticipate the need to reboot. When Boxstarter reboots, it starts running the package from the beginning. So ensure that there is nothing that would cause the package to break if run twice.
- If you have several Chocolatey packages that you want to install during the Boxstarter session, it is preferable to call choco install directly from inside your ChocolateyInstall instead of declaring them as dependencies. This is because Boxstarter cannot intercept Chocolatey dependencies, so those packages will not have any reboot protections.
- Do not use `Restart-Computer` or any other command that will reboot the computer. Instead, use `Invoke-Reboot`. This will allow Boxstarter to get things in order first so that after the machine recovers from the reboot, Boxstarter can log the user back in and restart the installation script.

## Packing your Package .nupkg

Once you are finished composing your package contents, you can call the `Invoke-BoxstarterBuild MyPackage` command to generate the *.nupkg file for MyPackage. Boxstarter saves these files in the root of `$Boxstarter.LocalRepo`. If you have several packages in the local repository that you want to build all at once, omit the package name from the command to build all packages in the repository.