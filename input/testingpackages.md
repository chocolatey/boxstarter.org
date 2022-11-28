---
Order: 100
Title: Testing Packages
---

# Testing Packages

Boxstarter can detect which packages have changed and test their installations on one or more deployment targets. Boxstarter can automate publishing successful packages to their feed.

The Boxstarter TestRunner module can test [Chocolatey](https://chocolatey.org) package installs on one or more deployment targets and evaluate whether the installation was successful. The TestRunner module can also be used to automate the publishing of Chocolatey packages to a [NuGet](https://nuget.org) feed. That may be the public Chocolatey community feed or a private feed.

The TestRunner module can be downloaded and installed via Chocolatey by running:

```powershell
choco install Boxstarter.TestRunner
```

## Configuring the Local Boxstarter Repository

Boxstarter needs to know where your repository of Chocolatey packages is located. The Boxstarter repository is a single directory on disk that contains one subdirectory per package. By default, Boxstarter maintains this directory in a directory called `BuildPackages` inside the same root directory where the Boxstarter modules reside. See [Creating Packages](creatingpackages) for detailed information on using Boxstarter commands to simplify package creation. All Boxstarter commands that create and build packages act within this directory. The repository location can be found using the `$boxstarter` settings variable in `$Boxstarter.LocalRepo`.

![Boxstarter repository directory](/assets/images/repoDir.png)

This location can be changed using the `Set-BoxstarterConfig` command.

```powershell
Set-BoxStarterConfig -LocalRepo "c:\dev\Chocolatey-Packages"
```

By changing the location of the boxstarter repository with `Set-BoxstarterConfig` all future interactions with the Boxstarter repository will act on this location. To temporarily change the repository location just for the current PowerShell session, one can change the value of `$Boxstarter.LocalRepo` directly.

```powershell
$Boxstarter.LocalRepo = "c:\dev\Chocolatey-Packages"
```

### Configuring Boxstarter Testing Options

Boxstarter tests Chocolatey packages by deploying and installing the package to one or more deployment targets. The deployment options include settings that control what computers to use to test the packages, the credentials to use, VM checkpoints to restore as well as NuGet feed and API key for publishing successful packages.

To read the current settings for these options, use `Get-BoxstarterDeploymentOptions`. To set these options use `Set-BoxstarterDeploymentOptions`.

```powershell
$cred=Get-Credential Admin
Set-BoxstarterDeployOptions -DeploymentTargetCredentials $cred `
  -DeploymentTargetNames "testVM1","testVM2" `
  -DeploymentVMProvider Azure -DeploymentCloudServiceName ServiceName `
  -RestoreCheckpoint clean `
  -DefaultNugetFeed https://www.myget.org/F/mywackyfeed/api/v2 `
  -DefaultFeedAPIKey 5cbc38d9-1a94-430d-8361-685a9080a6b8
```

This configures package deployments for Azure VMs testVM1 and testVM2 hosted in the ServiceName cloud service using the Admin credential. Prior to testing a package install, the VM will be restored to the clean checkpoint. If packages are published using `Publish-BoxstarterPackage` and are not associated with a NuGet feed, they will publish to the mywackyfeed on [myget.org](https://myget.org) using API Key 5cbc38d9-1a94-430d-8361-685a9080a6b8.

Currently the available Boxstarter VM providers are Azure and HyperV. Any VM regardless of hypervisor implementation can be used by passing in the names of the computers as the deployment targets. However, use of Boxstarter VM providers are necessary in order to take advantage of checkpoints.

The default deployment options deploy locally using the credentials of the current user and publish to the public Chocolatey community feed.

Individual packages in a Boxstarter repository can be published to different feeds. The NuGet feed and API Key configured using `Set-BoxstarterDeploymentOptions` are default settings used if a package has not been individually assigned to its own feed.

To assign a package to an individual feed, use `Set-BoxstarterPackageNugetFeed`.

```powershell
Set-BoxstarterPackageNugetFeed -PackageName MyPackage -NugetFeed https://www.myget.org/F/myotherfeed/api/v2
```

When using multiple feeds, if one needs to use the `Publish-BoxstarterPackage` command to publish the package to its feed, an API key for each feed must be set. This can be accomplished by using the `Set-BoxstarterFeedAPIKey`.

```powershell
Set-BoxstarterFeedAPIKey -NugetFeed https://www.myget.org/F/myotherfeed/api/v2 -APIKey 5cbc38d9-1a94-430d-8361-685a9080a6b8
```

## Testing Chocolatey Packages

`Test-BoxstarterPackage` can be called with an array of packages.

```powershell
Test-BoxstarterPackage -PackageName "MyPackage1","MyPackage2"
```

Boxstarter will build their .nupkg files and attempt to install them on the deployment targets specified with `Set-BoxstarterDeploymentOptions`. Boxstarter will use the credentials provided in the deployment options. You can provide several targets to `Set-BoxstarterDeploymentOptions`. One may wish to supply different machines running different versions of windows. If a package install runs to completion with no exceptions or returned error codes, Boxstarter considers the install a PASSED test. If `Test-BoxstarterPackage` is called with no packages specified, Boxstarter will iterate over each package in its local repository. It will build the nupkg and compare its version to the version on the package published feed. If the version in the repo is greater than the published version, Boxstarter will initiate a test on the deployment targets otherwise the package test will be skipped.

![Testing Chocolatey packages console output](/assets/images/CI.png)

## Publishing Successful Packages

`Test-BoxstarterPackage` will return a set of test results. One can then use `Select-BoxstarterResultsToPublish` to consume these results and return the package IDs of the packages who had all deployment targets pass the package install. These IDs can then be passed to `Publish-BoxstarterPackage` to publish those packages to their associated feeds.

```powershell
Test-BoxstarterPackage | Select-BoxstarterResultsToPublish | Publish-BoxstarterPackage
```

## Conserving Cloud Resources

If the Azure Boxstarter module is used as a VM provider and any of the Azure VMs are not running when the tests begin, Boxstarter will shut down each VM that was not initially running once the testing is completed.

## Including Testing and Publishing in Continuous Integration

The Boxstarter TestRunner can be plugged into modern Build systems and respond to source control commits. The Boxstarter local repo can be setup under a source control system like GIT, Mercurial, TFS, SVN etc. Build servers like TeamCity, TFS, Jenkins and others can be configured to listen for changes to the Boxstarter repo. For instance, if the repository is under git source control and pushed to a remote server monitored by a build server, the build server can execute the Boxstarter TestRunner commands to test the packages that have been changed and publish them to their feeds if the tests pass.

The TestRunner module includes an Install-BoxstarterScripts command that embeds a MSBuild file, a bootstrapper and a PowerShell script into a repository that can run the tests and publish successful packages.

```powershell
PS C:\dev> git clone https://github.com/mwrock/Chocolatey-Packages.git
Cloning into 'Chocolatey-Packages'...
remote: Reusing existing pack: 2963, done.
Receiving objects: 100% (2963/2963), 3.93 MiB |
Resolving deltas: 100% (961/961), done.
Checking connectivity... done.
Checking out files: 100% (2491/2491), done.
PS C:\dev> Install-BoxstarterScripts -RepoRootPath .\Chocolatey-Packages
Boxstarter: Copying Boxstarter TestRunner scripts to .\Chocolatey-Packages\BoxstarterScripts
PS C:\dev> dir .\Chocolatey-Packages\BoxstarterScripts
    Directory: C:\dev\Chocolatey-Packages\BoxstarterScripts
Mode                LastWriteTime     Length Name
----                -------------     ------ ----
-a---         3/30/2014  11:02 PM         32 .gitignore
-a---         3/30/2014  10:53 PM       3471 bootstrap.ps1
-a---         3/30/2014  10:53 PM        887 boxstarter.proj
-a---         3/30/2014  10:53 PM       1305 BoxstarterBuild.ps1
```

There are two primary usage patterns for running tests from a build server:

### Dedicated Build Server

Here, tests are run in a build environment on a server that is under your direct ownership and full control. The key differentiator here is that you can set your deployment options on this machine and be confident that they will remain on every build from build to build.

### Hosted or Shared Build Server

This is often a build server that you do not own or control outside of your individual builds. If you set your deployment options on the server, they may likely be gone on your next build. The Visual Studio Online build services would land in this category.

## Configuring a Dedicated Build Server

If you administer your own build server, there is some one time setup that you need to perform to integrate Boxstarter TestRunner tests and automatic package publishing. This must be performed on the build machine that will run the builds (a remote PowerShell session is acceptable). In many build systems, this may be a different machine from the central server, in TFS it would be the build "controller" or in TeamCity the build "agent." Also, these steps must be performed using the credentials of the SAME USER ACCOUNT that your build server runs under.

1. Install the Boxstarter TestRunner module on the build machine. You can use Chocolatey and run `choco install Boxstarter.TestRunner` or launch https://boxstarter.org/package/nr/boxstarter.testrunner from IE.
1. Although many of the deployment options are stored in a file inside the repository and will be available when your build server gets the latest repository code, the "secret" options are not kept in source control to prevent this information from being publicly disclosed. These secrets include the deployment target credentials and feed API keys. So you can use Set-BoxstarterDeployOptions and Set-BoxstarterFeedAPIKey to set these secrets. The Credential passwords are encrypted and can only be unencrypted on the same build server by the same user account setting the options. You may also add these settings as msbuild parameters in step 4 described below.

    ```powershell
    $cred=Get-Credential Admin
    Set-BoxstarterDeployOptions -DeploymentTargetCredentials $cred `
      -DefaultFeedAPIKey 5cbc38d9-1a94-430d-8361-685a9080a6b8
    ```  
    
1. If you will be performing the package test installations on Azure VMs, you will need to configure your Azure subscription. Run
    
    ```powershell
    Get-AzurePublishSettingsFile
    ```

    to download your subscription file from Azure and then
    
    ```powershell
    Import-AzurePublishSettingsFile -PublishSettingsFile "C:\saved\subscription\file.publishsettings"
    ```

    You may also add the Azure management key as a MSBuild parameter described next. See [these instructions](vmintegration#azure) for setting up an Azure subscription for more details.
1. Configure the build configuration of your build server to call the Boxstarter.proj MSBuild file as a step in your build. This file is located in the BoxstarterScripts directory of your repo which Boxstarter creates when you run Install-BoxstarterScripts. This script will invoke a bootstrapper which will install everything needed for the TestRunner to run including the TestRunner itself if not present. On dedicated build servers, where the TestRunner is preinstalled, the bootstrapper will be skipped.
    
    The script can take the following parameters:
    
    - `PublishSuccesfulPackages` - Set this to true if you would like successful packages published.
    - `DeploymentTargetUserName` - Username to use when initiating connections to the deployment targets.
    - `DeploymentTargetPassword` - Password to use when initiating connections to the deployment targets.
    - `FeedAPIKey` - API key to use when publishing packages. AzureSubscriptionName - Name of the azure subscription to use when using Azure VMs as deployment targets.
    - `AzureSubscriptionId` - The Azure subscription ID Guid to use when using Azure VMs as deployment targets.
    - `AzureSubscriptionCertificate` - The Azure subscription certificate to use when using Azure VMs as deployment targets. This is the Base64 encoded content of the certificate and can be found in the ManagementCertificate attribute of your Azure publish settings file.
    
    ![Configuring a dedicated build server](/assets/images/tfs.png)
    
    > :choco-info: **NOTE**
    >
    > All of these MSBuild script parameters are optional for dedicated builds as long as the values were provided in steps 2 and 3 above which may be preferable over having plain text parameter values fed to your build.
    

## Configuring a Hosted or Shared Build Server

If your build server environment is shared and may be built from scratch on every build then you cannot preinstall the TestRunner and configure necessary settings up front. Instead the runner must be installed during the build process and the settings passed via MSBuild parameters. When you run `Install-BoxstarterScripts` on your Chocolatey repository, a bootstrapper is added to your repo that can take care of installing the Test Runner and its dependencies. Additionally, you can pass the following MSBuild parameters to the BoxstarterBuild.proj script:

- `PublishSuccesfulPackages` - Set this to true if you would like successful packages published.
- `DeploymentTargetUserName` - Username to use when initiating connections to the deployment targets.
- `DeploymentTargetPassword` - Password to use when initiating connections to the deployment targets.
- `FeedAPIKey` - API key to use when publishing packages. AzureSubscriptionName - Name of the azure subscription to use when using Azure VMs as deployment targets.
- `AzureSubscriptionId` - The Azure subscription ID Guid to use when using Azure VMs as deployment targets.
- `AzureSubscriptionCertificate` - The Azure subscription certificate to use when using Azure VMs as deployment targets. This is the Base64 encoded content of the certificate and can be found in the ManagementCertificate attribute of your Azure publish settings file.