---
Order: 90
Title: Installing Packages On A Virtual Machine
---

# Installing packages on a Virtual Machine

Boxstarter provides convenient integration with both [Hyper-V](vmintegration#HyperV) and [Windows Azure](vmintegration#azure) Virtual Machines with support for auto configuration of remote connectivity as well as setting and restoring checkpoints. Of course Boxstarter can run on any VM guest as long as PowerShell remoting is enabled.

## Hyper-V

### Limitations and Prerequisites

Boxstarter's Hyper-V support is limited to:

- PowerShell Version 3 Hyper-V Hosts and above. This is automatically installed on Windows 8, 8.1 and Server 2012 and 2012 R2. Windows Server 2008 R2 can be upgraded to Version 3 (or 4) by installing version 3 or 4 of the [Windows Management Framework](https://www.microsoft.com/en-us/download/details.aspx?id=34595).
- Microsoft's Hyper-V virtualization technology comes with Windows Server and also the Windows client operating systems 8 and 8.1 professional and enterprise. It cannot be installed on Home editions of Windows 8 or on any version of Windows 7.
- If Boxstarter determines that configuration is necessary for PowerShell Remoting, the VHD file of the VM must be accessible and writable to the user issuing the `Enable-BoxstarterVM` command.
- Boxstarter's Hyper-V commands must be run on the Hyper-V host machine.
- Hyper-V Guests are subject to the same limitations as any other Boxstarter installation environment: PowerShell v2, .net 4, and OS's Windows 7/Server 2008 R2 and higher.

### Automatic configuration of Hyper-V guest instances

Boxstarter will configure PowerShell Remoting on the Hyper-V guest if it finds that it is not enabled already. The amount of configuration depends on the environment. If web services management (WSMAN) or WMI ports are already enabled and a built-in administrator or domain credential is being used, Boxstarter should not need to perform any special configuration above that done by `Install-BoxstarterPackage`. Boxstarter can enable PowerShell Remoting from a WMI method call. If WSMAN or WMI are not open (by default they are not enabled on client versions of windows) or if the credential being used is a local user other than the built-in administrator, Boxstarter will shut down the VM, mount its VHD, and make a couple small changes to the Windows registry to either enable WMI, enable the LocalAccountTokenFilterPolicy, or both.

```powershell
$cred=Get-Credential domain\username
Enable-BoxstarterVM -VMName myVM -Credential $cred
```

This will perform any of the configuration mentioned above if necessary and return a `BoxstarterConnectionConfig` object holding the DNS name of the guest machine and the Credential passed to the command.

This object can be piped to Boxstarter's Install-BoxstarterPackage command.

```powershell
$cred=Get-Credential domain\username
"myVM1","myVM2" |
  Enable-BoxstarterVM -Credential $cred |
    Install-BoxstarterPackage -PackageName myPackage
```

This installs myPackage on myVM1 and myVM2 after ensuring that both are properly configured for a remote install.

### Checkpoints

The `Enable-BoxstarterVM` command can take a CheckpointName parameter. If this checkpoint exists, Boxstarter will restore the VM to this checkpoint before performing any action. If the checkpoint does not exist, Boxstarter will create a new Checkpoint after any configurations have been made.

This is particularly useful when you want to restore a machine to a known state before performing an installation.

```powershell
$cred=Get-Credential domain\username
Enable-BoxstarterVM -VMName myVM1 -Credential $cred -CheckPointName BareOS |
    Install-BoxstarterPackage -PackageName myPackage
```

This resets the VM to its BareOS state before Installing the myPackage package.

### What about creating New VMs with Boxstarter?

I considered adding a New-BoxstarterVM command but the fact is that Microsoft's own Hyper-V PowerShell module does a fine job of making this easy. This is typically a task that is not done nearly as frequently as tearing down and resetting an existing VM so it did not seem like a feature much worth the effort. To demonstrate how you would setup a New Hyper-V VM. You could use a script like this:

```powershell
New-VMSwitch -NetAdapterInterfaceDescription "Intel(R) Centrino(R) Advanced-N 6205" -Name MySwitch
New-VM -Name "myVM" -MemoryStartupBytes 1GB -SwitchName MySwitch -VHDPath "C:\Users\Matt\Downloads\Windows Server 2008 R2 Enterprise Evaluation (Full Edition).vhd"
Start-VM "myVM"
```

That's pretty simple. This creates a virtual switch that binds to our network adapter. Your adapter might be named differently. Run `Get-NetAdapter` to get a list of your adapters. Then create the VM and attach it to a VHD and the switch you just created. Now you have a VM with Windows Server 2008 R2 connected to the Internet. There are a ton of other useful commands. Run `Get-Command -Module Hyper-V` to see them all. They are all well documented. For example, to view the documentation on the `Add-VMDvdDrive` command, run:

```powershell
Get-Help Add-VMDvdDrive -Full
```

If for some reason you cannot find the Hyper-V module on your machine (it's supported with Windows PowerShell 4), you may need to enable the feature:

```powershell
Install-WindowsFeature -name hyper-v -IncludeManagementTools
```

### Download a free evaluation edition Windows VHD

You don't need a purchased copy of Windows or a fancy shmancy MSDN subscription to test out Boxstarter Installs on a VM. Microsoft provides evaluation copies of many of its OS versions on VHD that can be used for a limited number of days (usually 90). You can legally reinstall a new copy when the evaluation expires. While this is unfit for a production environment, its a great way to test your Boxstarter or Chocolatey installs.

Here are links to free Windows evaluation VHDs:

- [Windows 8.1](https://technet.microsoft.com/en-US/evalcenter/evaluate-windows-8-1-enterprise)
- [Windows 2008 R2](https://www.microsoft.com/en-us/download/details.aspx?id=16572)
- [Windows 2012 R2](https://technet.microsoft.com/en-us/evalcenter/evaluate-windows-server-2012-r2)

## Windows Azure VMs

### Prerequisites

Boxstarter's Windows Azure VM support is limited to:

- PowerShell Version 3 or above. Boxstarter leverages the Windows Azure PowerShell toolkit which requires at least PowerShell version 3. This is automatically installed on Windows 8, 8.1 and Server 2012 and 2012 R2. Windows 7 and Server 2008 R2 can be upgraded to Version 3 (or 4) by installing version 3 or 4 of the [Windows Management Framework](https://www.microsoft.com/en-us/download/details.aspx?id=34595).
- The default PowerShell remoting and endpoints that Azure creates on new VMs mus be present. By default, Windows Azure enables PowerShell remoting on all Windows VMs and it creates an endpoint that maps to the WINRM secure port.

### Installing the Boxstarter.Azure module

The Boxstarter.Azure module does not automatically install with the main Boxstarter install package, but just a single Chocolatey command can accomplish this.

```powershell
choco install Boxstarter.Azure
```

This will install the Boxstarter.Azure module and all of its prerequisites: Windows Azure PowerShell, Windows Azure SDK for .Net Libraries, and the .Net Framework v4.5.

### One time configuration of Azure subscription information

Before Windows Azure PowerShell can begin to manage your VMs, it needs your account information as well as authentication verification. This is incredibly easy, **only needs to be performed once** and involves just three commands:

```powershell
Get-AzurePublishSettingsFile
```

This command will launch your default browser and initiate a Publisher Settings download. First you will land on the Windows Azure sign in page and as soon as you successfully authenticate, the download will begin.

![Windows Azure download page](/assets/images/publishAzure.png)

Now simply import the file just downloaded:

```powershell
Import-AzurePublishSettingsFile -PublishSettingsFile C:\Users\Matt\Downloads\Subscription-1-1-19-2014-credentials.publishsettings
```

Finally, specify the name of the storage account you want to use. You can run `Get-AzureStorageAccount` for a list of all of your storage accounts.

```powershell
Set-AzureSubscription -SubscriptionName MySubscription -CurrentStorageAccountName MyStorageAccount
```

> :choco-info: **NOTE**
>
> Boxstarter will attempt to set your Current Storage Account for you if it has not been specified. However, you will need to run the command yourself if you need to run other Windows Azure PowerShell commands prior to using Boxstarter.

That's it. You can now use the Windows Azure PowerShell and Boxstarter commands to provision VMs in Azure.

### Creating an Azure VM

Creating Azure VMs is simple. You can either use the Azure portal or PowerShell. Since the Boxstarter.Azure module also loads the Windows Azure PowerShell module, all of the Azure PowerShell commands are available in the Boxstarter Shell. The easiest way to create new VMs using PowerShell is with the `New-AzureQuickVM` command:

```powershell
$cred=Get-Credential AzureAdmin
New-AzureQuickVM -ServiceName MyService -Windows -Name MyVM `
  -ImageName 3a50f22b388a4ff7ab41029918570fa6__Windows-Server-2012-Essentials-20131217-enus `
  -Password $cred.GetNetworkCredential().Password -AdminUsername $cred.UserName `
  -Location "West US" -WaitForBoot
```

This creates a small Windows Server 2012 R2 VM located in the West US region and will wait for the VM to complete its boot sequence. Note: If the Service Name already exists, do not include the `-Location` argument since the VM will be created in the location that service is configured for. To find out what all of the Location options are, use the `Get-AzureLocation` command.

While this example uses a Windows Server 2012 R2 image, you can use any of the images in the Azure gallery or even images you have created yourself. Use `Get-AzureVMImage` for information on all of the images available.

The Azure PowerShell commands are very useful. Use `Get-Command -Module Azure` to find all commands available in the Windows Azure PowerShell module. All commands include the customary PowerShell command line help. Simply run `Get-Help [Command Name] -Full` to view the full documentation of a command.

### Provisioning your Windows Azure VM

Boxstarter will locate the VMs PowerShell remoting Connection URI and install the VMs certificate on the local machine which will enable a SSL connection to be established so that Boxstarter can install your package on the VM. Here is an example:

```powershell
$cred=Get-Credential AzureAdmin
"myVM1","myVM2" |
  Enable-BoxstarterVM -provider Azure -CloudServiceName MyService -Credential $cred |
    Install-BoxstarterPackage -PackageName myPackage
```

This installs myPackage on myVM1 and myVM2.

### Checkpoints

Just like with the HyperV provider, the `Enable-BoxstarterVM` command can take a CheckpointName parameter. If this checkpoint exists, Boxstarter will restore the VM to this checkpoint before performing any action. If the checkpoint does not exist, Boxstarter will create a new Checkpoint before installing any packages.

This is particularly useful when you want to restore a machine to a known state before performing an installation.

```powershell
$cred=Get-Credential domain\username
Enable-BoxstarterVM -Provider azure -CloudServiceName MyService `
  -VMName myVM1 -Credential $cred -CheckPointName BareOS |
    Install-BoxstarterPackage -PackageName myPackage
```

This resets the VM to its BareOS state before Installing the myPackage package.

Boxstarter leverages Azure Blob snapshots to support this check-pointing functionality. Note that the checkpoints are not visible in the Azure portal. However, if you would like to manage these checkpoints, Boxstarter exposes the following commands:

- Get-AzureVMCheckpoint
- Set-AzureVMCheckpoint
- Restore-AzureVMCheckpoint
- Remove-AzureVMCheckpoint

All commands take a `-VM` and a `-CheckpointName` parameter.

For example, to create or reset a checkpoint:

```powershell
$vm=Get-AzureVM -ServiceName MyService -Name MyVM
Set-AzureVMCheckpoint -VM $vm -CheckpointName BareOS
```

Now the BareOS checkpoint resembles the current state of the MyVM VM.

To list all checkpoints of a VM, you can omit the CheckpointName parameter of `Get-AzureVMCheckpoint`:

```powershell
$vm=Get-AzureVM -ServiceName MyService -Name MyVM
Get-AzureVMCheckpoint -VM $vm
```

## Working with other virtualization technologies

Boxstarter will very soon provide support for AWS VMs. However, Boxstarter can work with any VM provider using Install-BoxstarterPackage as long as the VM has PowerShell remoting enabled and the VM's DNS name is accessible on the network. For "On Premise" VM providers, if they support the VHD file format, you can use the `Enable-BoxstarterVHD` command to enable WMI, LocalAccountTokenFilterPolicy and retrieve the computer name of the VM running the VHD. The VHD must be accessible and writable to the user issuing the command and it must contain the system volume (the drive with the Windows folder) of the VM.

```powershell
Enable-BoxstarterVHD -VHDPath "C:\Virtual hard disks\win81diff_F2AE6535-2532-455F-8E5A-0211595221F8.avhdx"
```

This will mount the VHD provided in the VHDPath, enable the WMI Firewall ports and enable LocalAccountTokenFilterPolicy in the Windows registry stored in the VHD.

> :choco-info: **NOTE**
>
> Although The parameter is explicitly referred to as `VHDPath`, any valid VHD file extension is accepted. The above example uses a differencing disk, but is valid since that is the primary hard drive of a guest VM.

### Troubleshooting Boxstarter VM Installs

#### General Connectivity

Boxstarter makes a best effort attempt to test and configure your VM for remote installation. However, every environment is different and may include slightly tweaked configurations that Boxstarter cannot anticipate. Especially if you are inside a domain environment, there may be various group policies in place that edit various settings and permission levels that will cause a Boxstarter install to fail. Almost all of these scenarios can be corrected for and over time, Boxstarter should improve at compensating for these nuances.

The first thing to do if Boxstarter is unable to reach the VM is to run:

```powershell
Enable-PSRemoting -Force
```

on the VM. This will most often put the machine in a state where it can accept remote sessions from Boxstarter.

#### Have you lost your domain trust relationship?

In a domain environment, it is possible that over time a VM may lose its trust relationship with the Domain Controller especially if you revert to a snapshot that has an expired machine password. When this happens things stop authenticating correctly and often remote communication sessions are denied. It is not always obvious that this is the problem. Usually you can tell by examining the Network Adapter on the VM and it states that it is Unauthenticated. The most reliable fix for this is to unjoin the domain and then rejoin.