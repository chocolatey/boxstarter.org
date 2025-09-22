# Boxstarter Docs

This repository contains the source files for the documentation site that can be found here:

https://boxstarter.org

This site is built using [Statiq](https://statiq.dev/).

## Thanks

The original template that was used to create this docs site came from the work that was done by [Patrik Svensson](https://github.com/patriksvensson), on his [Spectre.Console](https://spectresystems.github.io/spectre.console/) and [Spectre.Cli](https://spectresystems.github.io/spectre.cli/) docs sites. Huge thank you to Patrik for all his help!

## Building the site

### Setup

There are a number or pre-requisites that are needed before you will be able to build the website locally.  These include:

* .NET Core SDK
* Node.js
* Yarn

There is a `.\setup.ps1` file in the root of this repository that can be used to install all necessary packages, and which will be kept up to date as these pre-requisites change.

### Building the site

To build the site locally on your machine, either run the `.\build.ps1` or the `build.sh` file (depending on your system).  This will compile the site, and all generated output file be placed into the `output` folder.

### Previewing the site

To preview the site locally on your machine, either run the `.\preview.ps1` or the `preview.sh` file (depending on your system).  Once completed, you should be able to open a browser on your machine to `http://localhost:5080` and the site will be loaded.  Once running, any changes made to the files within the `input` folder will cause the site to be rebuilt with the new content.

## Build Status

[![GitHub Actions Build Status](https://github.com/chocolatey/boxstarter.org/workflows/Publish%20Documentation/badge.svg)](https://github.com/chocolatey/boxstarter.org/actions?query=workflow%3A%22Build+Pull+Request%22)

## Chat Room
Come join in the conversation about Chocolatey in our [Community Chat Room](https://ch0.co/community).

Please make sure you've read over and agree with the [etiquette regarding communication](https://github.com/chocolatey/choco/blob/master/README.md#etiquette-regarding-communication).

## Images

All images are located in the [Chocolatey Image repository](https://github.com/chocolatey/img/) along with the following information:

1. [How to add images to the image repository](https://github.com/chocolatey/img?tab=readme#adding-images-to-this-repository).
1. [Where to find stock images](https://github.com/chocolatey/img?tab=readme#where-to-find-stock-images).
1. [Using images in source code](https://github.com/chocolatey/img?tab=readme#using-images-in-source-code).