---
Order: 20
Title: Simple Package Creation
Description: Learn how to create a package for a machine install.
---

# Create a package for your machine install

Perhaps you have a common collection of applications and settings that you would like to install on any machine you have to work on. You don't want to memorize complicated URLs.

Create and publish a package to create simple Boxstarter URLs!

## Step 1

### Install Boxstarter

If you have Chocolatey:

```powershell
choco install Boxstarter
```

Otherwise, click on the download link at the top of this page and run the Setup.bat file. See [Installing Boxstarter](/installboxstarter) for details.

## Step 2

### Create your installer script

Open your favorite text editor and compose your installer. Although this can contain any valid PowerShell, you really do not need to know any PowerShell to create your script. This could just be a collection of Boxstarter commands. Here is an example:

```powershell
Set-WindowsExplorerOptions -EnableShowHiddenFilesFoldersDrives -EnableShowProtectedOSFiles -EnableShowFileExtensions
Enable-RemoteDesktop
Set-StartScreenOptions -EnableBootToDesktop
choco install fiddler4
choco install git-credential-winstore
choco install console-devel
choco install sublimetext2
choco install poshgit
choco install dotpeek
Install-WindowsUpdate -AcceptEula
```

These are just Chocolatey Install calls and Boxstarter commands. Save this to a file - name and location do not matter. For a complete list of Boxstarter commands available in a package, see [Boxstarter WinConfig Commands](/winconfig) and [Creating Packages](/creatingpackages) for details covering the creation of Chocolatey packages.

## Step 3

### Invoke the Boxstarter shell

![Boxstarter shortcut icon](/assets/images/shortcut.png)

The Boxstarter install creates a shortcut to a shell that provides the easiest way to access Boxstarter commands especially if you are not familiar with PowerShell. See [Using Boxstarter Commands](/usingboxstarter) for information about running Boxstarter in your own shell.

## Step 4

### Convert your Script to a Package

Lets assume the script was saved to c:\dev\script.ps1, and I wanted to name my package mwrock.DefaultInstall. Run this command:

```powershell
New-PackageFromScript -Source c:\dev\script.ps1 -PackageName mwrock.DefaultInstall
```

This creates a simple Chocolatey package and packs it to a .nupkg file.

## Step 5

### Publish the Package

You can publish your package to any feed you have access to. We will publish to the public Chocolatey community feed. To do this you will need to [register](https://community.chocolatey.org/account/Register) a free Chocolatey account and obtain an API key.

Once you have a Chocolatey account and have followed the instructions on the Registration confirmation regarding setting your API key, you can use Chocolatey's choco push command to accomplish this:

```powershell
choco push (Join-Path $Boxstarter.LocalRepo mwrock.DefaultInstall.1.0.0.nupkg)
```

Boxstarter saves all packages to its LocalRepo property. Now choco push will find the .nupkg file and upload it to the Chocolatey community feed. See [Publishing Packages](/publishingpackages)for more details regarding package publishing and using different feeds.

## Step 6

### Install the Package anywhere

Using IE (or any other browser that has a click-once extension installed) or from a command line (if IE or a click-once enabled browser is your default browser) run:

START https://boxstarter.org/package/mwrock.DefaultInstall

Accept the prompts for installing the Boxstarter installer and running with administrative privileges. When the installation begins, Boxstarter will ask for a password do that it can resume the installer in the event it needs to reboot.

There are other ways to install the package as well. You can even [install to a remote machine](/installingpackages#RemoteInstallations) or to an [Azure or Hyper-V VM with checkpointing support](/vmintegration).
