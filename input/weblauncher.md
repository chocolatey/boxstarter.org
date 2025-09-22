---
Order: 20
Title: Launch from the Web
---

# Quickly setup any machine with one simple URL

Perhaps you have a common collection of applications and settings that you would like to install on any machine you have to work on. You don't want to have to always carry thumb drives or memorize complicated commands and URLs.

Boxstarter makes this process a snap!

## Installing several packages

While often install scripts may be complex and the information on this page will show you how to capture such scripts in a gist, if you simply want to install a list of Chocolatey packages, you can use this URL:

```
https://boxstarter.org/package/sysinternals,fiddler4,itunes
```

Launch this from your browser, that has ClickOnce support, and the Boxstarter launcher should install and run. The following browsers are supported:

- Internet Explorer has native support.
- Microsoft Edge has [native support](https://docs.microsoft.com/en-us/deployedge/edge-learn-more-co-di).
- Firefox with a [ClickOnce extension](https://addons.mozilla.org/en-US/firefox/search/?q=click%20once).
- Google Chrome with a [ClickOnce extension](https://chrome.google.com/webstore/search/clickonce).

_Due to the number of browsers and extensions we cannot make any specific recommendations on what you should use._

If you do not want Boxstarter to automatically reboot if needed, add /nr/ in the URL:

```
https://boxstarter.org/package/nr/sysinternals,fiddler4,itunes
```

## Installing from a Gist

### Step 1

#### Compose the installation script.

```powershell
Set-ExplorerOptions -showHiddenFilesFoldersDrives -showProtectedOSFiles -showFileExtensions
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

#### It's Simply Chocolatey

Other than the [Boxstarter commands](winconfig) that are imported into all Boxstarter installation sessions, you will likely recognize this syntax if you are already familiar with Chocolatey. This script is going to configure Windows Explorer in a way that won't drive you crazy, allow you to use Remote Desktop to connect to the machine, install a handful of applications that you commonly use and install Hyper-V's virtualization host and the IIS Web Server.

Boxstarter can run any [Chocolatey package](https://docs.chocolatey.org/en-us/create/create-packages-quick-start). The only difference as far as the script is concerned is that your script also has access to Boxstarter's commands for configuring Windows, running updates as well as logging and reboot control.

### Step 2

#### Save your script

You have lots of options here. Boxstarter supports:

- Saving as a package to any NuGet feed.
- Saving a package to a Network share or any local media such as a thumb drive.
- Saving to a single text file or any text based HTTP resource (like a [GitHub Gist](https://gist.github.com/))

We'll stay simple here and save to a GitHub Gist.

![Boxstarter GitHub Gist](https://img.chocolatey.org/boxstarter/gist1.png)

After saving the Gist, we'll click the "View Raw" link

![View raw link on Boxstarter GitHub Gist](https://img.chocolatey.org/boxstarter/gist2.png)

Now we will copy the raw URL:

![Copy the raw URL for the Boxstarter GitHub Gist](https://img.chocolatey.org/boxstarter/gist3.png)

### Step 3

#### Run the script

From a console (CMD or PowerShell), invoke the Boxstarter launcher:

```powershell
START https://boxstarter.org/package/nr/url?https://gist.github.com/mwrock/7382880/raw/f6525387b4b524b8eccef6ed4d5ec219c82c0ac7/gistfile1.txt
```

> Note this URL is a simple Boxstarter URL: https://boxstarter.org/package/nr/url. The /nr/ tells Boxstarter not to reboot the machine. Depending on what you are installing, you may not want to include that. Especially if you are including more heavy weight applications, frameworks or Windows Updates.

We add our raw Gist URL to the Boxstarter URL separated by a '?'.

Additionally, if the script is saved locally to a single text file, the path can be added to the Boxstarter URL, again separated by a '?'.

```powershell
START https://boxstarter.org/package/nr/url?c:\temp\myscript.txt
```

This invokes a Boxstarter installer that will install everything according to your script.

![Boxstarter installer prompt](https://img.chocolatey.org/boxstarter/install.png)

If not already installed, this will install the Chocolatey CLI and any of its prerequisites. If you did not include /nr/ in the Boxstarter URL, it will manage reboots and automatically log the machine back in, so you do not have to attend to it throughout the installation.

#### Other facts about the Boxstarter web installer:

- You can also install any package from the public [Chocolatey Community Repository](https://community.chocolatey.org/packages) feed. To install these packages, use the package name instead of URL. For example, if you wanted to install FireFox, you would use https://boxstarter.org/package/nr/firefox.
- As [noted above](#installing-several-packages), you need to install a ClickOnce extension for some browsers.