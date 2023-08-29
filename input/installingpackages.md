---
Order: 80
Title: Installing Packages
---

# Installing Packages

Boxstarter can install packages either from a local Boxstarter Repository, a network share or external media, or it can "push" an installation to a remote machine.

## Install-BoxstarterPackage

To kick off a Boxstarter installation session, use the `Install-BoxstarterPackage` command.

```powershell
$cred=Get-Credential domain\username
Install-BoxstarterPackage -PackageName "MyPackage1","MyPackage2" -Credential $cred
```

This will install the MyPackage1 and MyPackage2 packages locally and use the credentials given for any auto login needed after a reboot. Note that you cannot use auto login from a [32-bit PowerShell session on a 64-bit operating system](https://github.com/chocolatey/boxstarter/issues/1).

There may be times when you either know that no reboot will be needed or you want to ensure that none take place. To tell Boxstarter not to perform any reboots, even if a pending reboot is detected, use the `-DisableReboots` argument.

```powershell
Install-BoxstarterPackage -PackageName "MyPackage1","MyPackage2" -DisableReboots
```

Since no reboot will be performed, the credential is unnecessary.

## Reinstall Logic

If you are familiar with Chocolatey, you know that Chocolatey will check if a package has been previously installed and will skip the installation if there is already an installation for that same package. Boxstarter changes this logic just a bit depending on whether or not one or multiple packages are provided to Install-BoxstarterPackage or the Boxstarter URL. If just one package is provided, Boxstarter will always attempt to re-download and install the package, even if it had been previously installed. It's important to note this only applies to the "outer" package. If your package includes calls to `choco install`, those packages will abide by the normal Chocolatey install rules and will not be installed if they already exist in the Chocolatey lib folder. Furthermore, when you supply multiple packages to Boxstarter's command line or URL, it will skip installation for packages already installed.

## Installing a package without a package

As mentioned previously on [Creating Packages](creatingpackages), you may not need to create a package. If your install script is limited to a single script file, you can use a file path or URL to a script and Boxstarter will create a package on the fly using this script as the ChocolateyInstall.ps1.

```powershell
Install-BoxstarterPackage -PackageName https://gist.github.com/mwrock/8066325/raw/e0c830528429cd68a8c71dbff6f48298576d8d20/gistfile1.txt
```

This will download the script in the raw text gist and create a NuGet package using the script as the ChocolateyInstall.ps1. If the script is in a file accessible to Boxstarter, one can pass the path to the file as the -PackageName parameter.

## Understanding and controlling installation output

### Pipeline Output

For each machine that Boxstarter installs (see [below](installingpackages#RemoteInstallations) for remote installation details) an object is emitted to the PowerShell pipeline containing details about the installation:

```powershell
Errors       : {}
ComputerName : localhost
Completed    : True
FinishTime   : 11/30/2013 1:56:36 AM
StartTime    : 11/30/2013 1:56:20 AM
```

The properties included in the pipeline output are:

- **ComputerName:** The name of the computer where the package was installed
- **StartTime:** The time that the installation began
- **FinishTime:** The time that Boxstarter finished the installation
- **Completed:** True or False indicating if Boxstarter was able to complete the installation without a terminating exception interrupting the installation. Even if this value is True, it does not mean that all components installed in the package succeeded. Boxstarter will not terminate an installation if individual Chocolatey packages fail. Use the Errors property to discover errors that were raised throughout the installation.
- **Errors:** An array of all errors encountered during the duration of the installation.

### Increasing Console Output Verbosity

Boxstarter tries to strike the right balance of information provided to the command console. However, if you want more detailed output, use the `-Verbose` parameter of `Install-BoxstarterPackage` command.

### Silencing Console Output

You can completely silence all console host output by setting Boxstarter's SuppressLogging property to true.

```powershell
$Boxstarter.SuppressLogging=$True
Install-BoxstarterPackage -PackageName MyPackage -DisableReboots
Errors       : {}
ComputerName : localhost
Completed    : True
FinishTime   : 11/30/2013 1:56:36 AM
StartTime    : 11/30/2013 1:56:20 AM
```

The only output now is what is sent to the pipeline. You can hide this by capturing it in a variable

```powershell
$result=Install-BoxstarterPackage -PackageName MyPackage -DisableReboots
```

or piping it to $null.

```powershell
Install-BoxstarterPackage -PackageName MyPackage -DisableReboots | Out-Null
```

### Boxstarter Log File

Boxstarter always logs verbose output to `$Boxstarter.Log` which is usually located at $env:LocalAppData\Boxstarter\Boxstarter.log. Installations on remote machines will produce a log file at this location on the remote machine and the local log file will contain just the messages that ran locally.

## Running an installation from a network share or external media

You may want to place the Boxstarter modules and local repo on a network share, so that others on your network can invoke Boxstarter installs with nothing installed. Or you may want to save them on a thumb drive, SD, or Micro SD card, so you can always have Boxstarter and your packages handy. Boxstarter provides a batch file wrapper that can be called to facilitate these scenarios.

```powershell
Copy-Item $Boxstarter.BaseDir E:\ -Recurse
E:\BoxstarterDir\Boxstarter example
```

Boxstarter's Base Directory (`$Boxstarter.BaseDir`) is copied to e:\BoxstarterDir, then Boxstarter.bat is called which will:

- Bypass the user's execution policy and ensure that Boxstarter scripts can run
- Import the Boxstarter Modules
- Invoke a Boxstarter installation of the example package
- Will prompt the user for their password to use during reboots

![Boxstarter installing package example](/assets/images/console.png)

## Creating a Boxstarter Share

As stated already, Boxstarter and its local repository can exist on a network share. Boxstarter provides a command that will share the directory where Boxstarter exists.

```powershell
Set-BoxstarterShare BoxShare
```

This simply creates a network share named BoxShare pointing to the local Boxstarter base directory. Now users simply call:

```powershell
\\mycomputer\BoxShare\Boxstarter MyPackage
```

## Remote Installations

Maybe you don't want to share with others, but would rather like to "push" installations to remote machines and VMs. Well that is now possible with Boxstarter! Boxstarter handles almost all the Remoting details so that installing a package remotely is practically no different from doing so locally.

### Enabling PowerShell Remoting on the target machine

In many cases you may not have to do this. If your machine is on a domain where group policy settings automatically enable PowerShell Remoting or even just WMI, Boxstarter can do this for you. Also, if you are using the built-in administrator account on a Windows server OS, it will likely already be configured for remoting. If you are unsure, enabling it manually will not cause any harm and it is easy.

Open a PowerShell window as administrator on the target machine and simply run:

```powershell
Enable-PSRemoting -Force
```

> :choco-info: **NOTE**
>
> If you are installing a Boxstarter install on a Hyper-V VM from a Hyper-V host machine, using the Boxstarter.HyperV module's `Enable-BoxstarterVM` command will configure the VM remote connectivity for you if needed. See [Installing packages on a Virtual Machine](vmintegration) for details.

### Invoking from the local machine

Now on your machine where Boxstarter is installed:

```powershell
$cred=Get-Credential 'MyTargetMachine\myusername'
Install-BoxstarterPackage -ComputerName MyTargetMachine -PackageName MyPackage -Credential $cred
```

Boxstarter will use the credential to connect to MyTargetMachine, and will copy the Boxstarter modules and your Local Repository packages to its local storage. It will then run the package on that machine. Boxstarter will configure your local machine to be able to connect to the remote machine, and it may prompt you for permission to change settings. You can always provide the -Force parameter to silence the prompts and allow boxstarter to freely enable remoting if it is not already enabled.

Here is a screenshot of Boxstarter installing a Minecraft Server (bukkit) on a AWS EC2 instance:

![Screenshot of Boxstarter installing a Minecraft Server (bukkit) on an AWS EC2 instance](/assets/images/amazon.png)

You can pipe several computer names, URIs or Windows PowerShell sessions to `Install-BoxstarterPackage`. Boxstarter will attempt to install the package on each machine and return a PSObject result for each machine.

```powershell
$cred=Get-Credential 'MyTargetMachine\myusername'
$result = "thing1","thing2" | Install-BoxstarterPackage -PackageName MyPackage -Credential $cred
$result

Errors       : {}
ComputerName : thing1
Completed    : True
FinishTime   : 11/30/2013 1:56:36 AM
StartTime    : 11/30/2013 1:56:20 AM

Errors       : {}
ComputerName : thing2
Completed    : True
FinishTime   : 11/30/2013 1:56:58 AM
StartTime    : 11/30/2013 1:56:36 AM
```

## Using BoxstarterConnectionConfig objects

If you have multiple machines you want to install, but they require different credentials, use a BoxstarterConnectionConfig. This class contains a ConnectionURI, a PSCredential and an optional [PSSessionOption](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/new-pssessionoption) property that you can set and pipe to `Install-BoxstarterPackage`.

```powershell
$cred1=Get-Credential 'MyTargetMachine\myusername'
$cred2=Get-Credential 'Domain\myDomainUser'
$result = (new-Object -TypeName BoxstarterConnectionConfig -ArgumentList 'http://machine1:5985/wsman',$cred1,$null),`
    (new-Object -TypeName BoxstarterConnectionConfig -ArgumentList 'http://machine2:5985/wsman',$cred2,$null) |
    Install-BoxstarterPackage -PackageName MyPackage
```

## Chocolatey Sources and Remote Installations

When using Boxstarter to install packages on a remote machine, the .nupkg file for the provided package name is searched in the following locations and order:

- .\BuildPackages relative to the parent directory of the module file
- The Chocolatey community feed **OR** NugetSources in Boxstarter.config when the 'DelegateChocoSources' switch is used

This can be configured by editing $($Boxstarter.BaseDir)\Boxstarter.Config If the package name provided is a URL or resolves to a file, then it is assumed that this contains the Chocolatey install script and a .nupkg file will be created using the script.

```powershell
$cred=Get-Credential 'MyTargetMachine\myusername'
Install-BoxstarterPackage -ComputerName MyTargetMachine -PackageName MyPackage -Credential $cred -DelegateChocoSources

Boxstarter: Configuring local PowerShell Remoting settings...
Boxstarter: Configuring CredSSP settings...
Boxstarter: Testing remoting access on MyTargetMachine...
Boxstarter: Remoting is accessible on MyTargetMachine
Boxstarter: Copying Boxstarter Modules and LocalRepo packages at C:\ProgramData\Boxstarter to C:\Users\ADMINI~1.TRI\AppData\Local\Temp\ on MyTargetMachine...
Boxstarter: Delegating Boxstarter NugetSources to remote host...
Boxstarter: Running remote install...
```

### Troubleshooting Remote Installations

### Establishing remote connections with servers not on the local subnet

By default, server versions of Windows have a firewall rule restricting traffic that is not on the local subnet. There may be circumstances where you might want to connect over the public Internet to another machine. This can be done by manually enabling the "WINRM-HTTP-In-TCP-PUBLIC" firewall rule using the PowerShell 3.0 `Set-NetFirewallRule` or the command line `netsh advfirewall` command.

Client versions of Windows will only allow PSRemoting on Private and Domain networks. Clients on PowerShell 3.0 and higher can provide the `-SkipNetorkProfileCheck` to Enable-PSRemoting to allow connections to public computers on the local subnet. Boxstarter uses this parameter when enabling Remoting on clients. Like with Server versions, you can enable a firewall rule to work around the local subnet restriction.

Windows 7 OSs running PowerShell 2.0 can not enable PSRemoting when on a public network connection. You will need to change your connection type to a private or Domain based connection.

### CredSSP and Access Exceptions when referencing network resources

When establishing a remote connection, Boxstarter uses CredSSP authentication so that the session can access any network resources normally accessible to the Credential. If necessary, Boxstarter configures CredSSP authentication on both the local and remote machines as well as the necessary Group Policy and WSMan settings for credential delegation. When the installation completes, Boxstarter rolls back all settings that it changed to their original state. When using a Windows PowerShell session instead of ComputerName or ConnectionURI, Boxstarter will use the authentication mechanism of the existing session and will not configure CredSSP if the session provided is not using CredSSP. If the session is not using CredSSP, it may be denied access to network resources normally accessible to the Credential being used. If you do need to access network resources external to the session, you should use CredSSP when establishing the connection.

### Installation Failures

Remote installations performed via PowerShell Remoting can sometimes fail. The symptoms of such failures can manifest themselves in a variety of ways and do not fail in a consistent manner. Boxstarter makes great effort to ensure the success of remote installations by leveraging scheduled tasks when it sees certain install activities. Boxstarter intercepts all calls to MSIEXEC, DISM (Installer for "Windows Features") and Windows Update. It is possible for more involved installation scripts to perform these operations in such a way that Boxstarter will not be able to intercept. If you suspect that your installation is failing due to using a remote session, you can use Boxstarter's `Invoke-FromTask` command to manually wrap your install in a scheduled task and thereby use a Local token.

```powershell
Invoke-FromTask "DISM /Online /NoRestart /Enable-Feature:TelnetClient" -IdleTimeout 20
```

This will run the DISM command to install the Telnet client in a scheduled task, and will kill the process if it becomes idle for more than 20 seconds. The scheduled task will always run with elevated privileges and be invoked immediately. If the same user whose credentials created the task is logged on to an active session, the scheduled task will run interactively on the remote machine. All console output from the scheduled task will be intercepted by Boxstarter and rendered to the user's console.