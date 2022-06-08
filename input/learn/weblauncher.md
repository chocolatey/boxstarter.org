---
Order: 30
Title: Run With A Gist
Description: Learn how to quickly setup and machine with just a Gist.
---

# Quickly setup any machine with just a Gist - No Preinstalled software required

Want to setup your box without downloading any software or fussing with package authoring or publishing?

Well buckle up! Boxstarter makes this a snap!

## Step 1

### Compose the installation script.

``` powershell
Set-WindowsExplorerOptions -EnableShowHiddenFilesFoldersDrives -EnableShowProtectedOSFiles -EnableShowFileExtensions
Enable-RemoteDesktop

choco install fiddler4
choco install git-credential-winstore
choco install console-devel
choco install sublimetext2
choco install poshgit
choco install dotpeek

choco install Microsoft-Hyper-V-All -source windowsFeatures
choco install IIS-WebServerRole -source windowsfeatures

```

### Its Simply Chocolatey

Other than the [Boxstarter commands](/winconfig) that are imported into all Boxstarter Installation sessions, you will likely recognize this syntax if you are already familiar with Chocolatey. This script is going to configure Windows Explorer in a way that won't drive you crazy, allow you to use Remote Desktop to connect to the machine, install a handful of applications that you commonly use and install Hyper-V's virtualization host and the IIS Web Server.

Boxstarter can run any [Chocolatey package](https://docs.chocolatey.org/en-us/create/create-packages-quick-start). The only difference as far as the script is concerned is that your script also has access to Boxstarter's commands for configuring Windows, running updates as well as logging and reboot control.

## Step 2

### Save your script

You have lots of options here. Boxstarter supports:

- Saving to as a package on any NuGet feed.
- Saving a package to a Network share or any local media such as a thumb drive.
- Saving to a single text file or any text based HTTP resource (like a [Github Gist](https://gist.github.com/))

We'll stay simple here and save to a Github Gist.

![Boxstarter GitHub Gist](/assets/images/gist1.png)

After saving the Gist, we'll click the "View Raw" link

![View raw link on Boxstarter GitHub Gist](/assets/images/gist2.png)

Now we will copy the raw URL:

![Copy the raw URL for the Boxstarter GitHub Gist](/assets/images/gist3.png)

## Step 3

### Install the Boxstarter Modules

You can download the Boxstarter module installer from this web site or you can use Chocolatey to install the Boxstarter. Alternatively, you may invoke the module installer over the web using PowerShell.

If you are running PowerShell v3 or higher:

```powershell
. { iwr -useb https://boxstarter.org/bootstrapper.ps1 } | iex; Get-Boxstarter -Force

```

If you are running PowerShell v2:

```powershell
iex ((New-Object System.Net.WebClient).DownloadString('https://boxstarter.org/bootstrapper.ps1')); Get-Boxstarter -Force
```

This will install Chocolatey if necessary and then install the necessary boxstarter modules.

## Step 4

### Run the script

Invoke the the Install-BoxstarterPackage command pointing to your gist created above:

```powershell
Install-BoxstarterPackage -PackageName https://gist.github.com/mwrock/7382880/raw/f6525387b4b524b8eccef6ed4d5ec219c82c0ac7/gistfile1.txt -DisableReboots
```

While you can use the -PackageName argument to point to any public Chocolatey package, you can also point to any URL that resolves to a plain text script like the gist we created. Note that we use the -DisableReboots argument to suppress automatic reboots.
