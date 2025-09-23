---
Order: 10
Title: Deploying A Web Application Remotely
Description: Learn how to install web services and deploy a web application to a remote server.
---

# Installing web services and deploying a web application to a remote server

Sometimes a package may contain more than just a nuspec file and an installation script. You may also want to install the package onto a remote machine.

Boxstarter can create a template package targeting an existing directory and install the package on any machine open to WINRM or WMI remoting.

This walk through will create a NuGet ODATA feed application and deploy it to a remote server after enabling web services on the server.

## Step 1

### Create and Build the NuGet server

Create an empty Web Application in Visual Studio and add a NuGet reference to NuGet.Server. After the package installation completes, build the solution. NOTE: This can be performed with the free Visual Studio Express for Web which can be installed via Chocolatey - `choco install VisualStudio2013ExpressWeb`.

![Manage Nuget packages from Visual Studio](https://img.chocolatey.org/boxstarter/NugetServer.png)

## Step 2

### Install Boxstarter

If you have Chocolatey:

```powershell
choco install Boxstarter
```

Otherwise, click on the download link at the top of this page and run the Setup.bat file. See [Installing Boxstarter](/installboxstarter) for details.

## Step 3

### Invoke the Boxstarter shell

![Boxstarter shortcut icon](https://img.chocolatey.org/boxstarter/shortcut.png)

The Boxstarter install creates a shortcut to a shell that provides the easiest way to access Boxstarter commands especially if you are not familiar with PowerShell. See [Using Boxstarter Commands](/usingboxstarter) for information about running Boxstarter in your own shell.

## Step 4

### Create the Package

Lets assume that you saved the above Visual Studio project to c:\dev\NugetServer. You can now create a package with the artifacts of this project embedded in the package:

```powershell
New-BoxstarterPackage -Name NugetServer
Copy-Item c:\dev\NugetServer "$($Boxstarter.LocalRepo)\NugetServer" -Recurse -Force
```

This creates a skeleton Chocolatey package with the contents of your NugetServer application embedded in the package. You will need to complete the ChocolateyInstall.ps1 file.

## Step 5

### Add installation details to ChocolateyInstall.ps1

Boxstarter has created a skeletal ChocolateyInstall.ps1 file at $($Boxstarter.LocalRepo)\NugetServer\tools\chocolateyInstall.ps1. Edit this file and replace its contents with the following script:

```powershell
try {
    choco install DotNet4.5

    #Enable Web Services
    choco install IIS-WebServerRole -source WindowsFeatures
    choco install IIS-ISAPIFilter -source WindowsFeatures
    choco install IIS-ISAPIExtensions -source WindowsFeatures

    #Enable ASP.NET on win 2012/8
    choco install IIS-NetFxExtensibility45 -source WindowsFeatures
    choco install NetFx4Extended-ASPNET45 -source WindowsFeatures
    choco install IIS-ASPNet45 -source WindowsFeatures

    #Enable ASP.NET on win 7/2008R2
    ."$env:windir\microsoft.net\framework\v4.0.30319\aspnet_regiis.exe" -i

    #clean and create application
    Remove-Item c:\web\NugetServer -Recurse -Force -ErrorAction SilentlyContinue
    Mkdir c:\web\NugetServer -ErrorAction SilentlyContinue
    Copy-Item "$(Join-Path (Get-PackageRoot $MyInvocation ) NugetServer)\*" c:\web\NugetServer -Recurse -Force
    Import-Module WebAdministration
    Remove-WebSite -Name "Default Web Site" -ErrorAction SilentlyContinue
    Remove-WebSite -Name NugetServer -ErrorAction SilentlyContinue
    New-WebSite -ID 1 -Name NugetServer -Port 80 -PhysicalPath c:\web\NugetServer -Force

    #Client SKUs need to enable firewall
    netsh advfirewall firewall add rule name="Open Port 80" dir=in action=allow protocol=TCP localport=80

    Write-ChocolateySuccess 'NugetServer'
} catch {
  Write-ChocolateyFailure 'NugetServer' $($_.Exception.ToString())
  throw
}
```

This script enables the minimum IIS features for the application to run, copies the application to the computer installing the package, creates a web application in IIS to point to the application directory and allows http traffic through the firewall.

## Step 6

### Build the package

We need to build our package script and application files into a single .nupkg file. Run:

Invoke-BoxstarterBuild -Name NugetServer

## Step 7

### Ensure PowerShell Remoting is enabled on the target machine

In many cases this step is unnecessary. If you are using either the builtin administrator account or a domain account with admin privileges on a server operating system, remoting is on by default. Otherwise, run the following command on the machine with a user who has admin privileges:

Enable-PSRemoting -Force

## Step 8

### Install the package on the target

Now we are ready to enable IIS services and deploy the application. We will create a PSCredential object and invoke the installation:

```powershell
$Creds = Get-Credential administrator
Install-BoxstarterPackage -ComputerName MyMachine -PackageName NugetServer -Credential $creds -Force
```

## Step 9

### Run the application

Now we should be able to browse to our NuGet server application at http://MyServer

![Running instance of NuGet.Server](https://img.chocolatey.org/boxstarter/NugetServerWeb.png)
