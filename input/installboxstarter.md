---
Order: 30
Title: Installing Boxstarter
---

# Installing Boxstarter

Boxstarter can be installed using [Chocolatey](https://chocolatey.org) or by downloading the Boxstarter zip file and running Setup.Bat

> NOTE: If you simply want to install a "one-off" script locally on a machine, you may find it easiest to [launch Boxstarter from the web](weblauncher).

 Prerequisites

Boxstarter requires the following to work:

- OS: Windows 7 or Windows Server 2008 R2 and higher
- PowerShell Version 2 or higher
- Administrative privileges on the machine where Boxstarter is running

## Installing from Chocolatey

```powershell
choco install Boxstarter
```

![Boxstarter Windows PowerShell install output](/assets/images/installed.png)

This will download and install the Boxstarter modules to your PSModulePath.

## Downloading the zip file

The zip file contains a Setup.Bat file that will go to Chocolatey and install Boxstarter. If Chocolatey is not installed on your machine, the Boxstarter installer will request the user's permission to install it. Note: Setup.bat accepts a `-Force` argument to suppress this prompt.

![Boxstarter Module installer](/assets/images/setup.png)

Boxstarter is now installed in your modules path!

Both the Chocolatey Boxstarter package and the downloaded zip file available on this website install all Boxstarter modules except the Boxstarter.Azure module which provides integration with Windows Azure VMs. You can install the Azure module by running `choco install Boxstarter.Azure`. In addition to the Boxstarter.Azure module, it also installs the Windows Azure PowerShell toolkit, the Azure SDK for .Net Libraries and .Net 4.5. It requires PowerShell 3 or higher (the version installed with Windows 8/server 2012 and higher but also [freely available for download](https://www.microsoft.com/en-us/download/details.aspx?id=40855) on older versions) to install successfully.

After Boxstarter is installed, a new PowerShell console must be opened before you can load the modules and run its commands. For convenience you can use the [Boxstarter Shell](usingboxstarter) for running Boxstarter commands which is the easiest way to ensure that all Boxstarter modules are properly loaded regardless of the version of Windows and PowerShell you are running.

## Installing from the web

Run the following command from PowerShell:

```powershell
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://boxstarter.org/bootstrapper.ps1')); Get-Boxstarter -Force
```

## Uninstalling Boxstarter

If you need to remove Boxstarter from your system, run code below in a PowerShell console:

```powershell
# Remove the Chocolatey packages in a specific order!
'Boxstarter.Azure', 'Boxstarter.TestRunner', 'Boxstarter.WindowsUpdate', 'Boxstarter',
    'Boxstarter.HyperV', 'Boxstarter.Chocolatey', 'Boxstarter.Bootstrapper', 'Boxstarter.WinConfig', 'BoxStarter.Common' |
    ForEach-Object { choco uninstall $_ -y }

# Remove the Boxstarter data folder
Remove-Item -Path (Join-Path -Path $env:ProgramData -ChildPath 'Boxstarter') -Recurse -Force

# Remove Boxstarter from the path in both the current session and the system
$env:PATH = ($env:PATH -split ';' | Where-Object { $_ -notlike '*Boxstarter*' }) -join ';'
[Environment]::SetEnvironmentVariable('PATH', $env:PATH, 'Machine')

# Remove Boxstarter from the PSModulePath in both the current session and the system
$env:PSModulePath = ($env:PSModulePath -split ';' | Where-Object { $_ -notlike '*Boxstarter*' }) -join ';'
[Environment]::SetEnvironmentVariable('PSModulePath', $env:PSModulePath, 'Machine')
```

Boxstarter installs [Chocolatey](https://chocolatey.org) and this needs to be [uninstalled](https://docs.chocolatey.org/en-us/choco/uninstallation) separately.