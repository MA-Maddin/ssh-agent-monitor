# SSH Agent Monitor

This repository contains the source code of the SSH Agent Monitor project (C# .NET 6 worker service) and a Wix 3 project to build the Windows Service installer for it.

If you just want to use the software, follow **Setup Prerequisites** below and then use the [latest released *.msi package](https://github.com/MA-Maddin/SSHAgentMonitor/releases) to install SSH Agent Monitor as a Windows Service.  
You may customize the monitoring behavior of the service by editing the `"Monitoring"` section of **appsettings.json** (can be found in install folder `C:\Program Files (x86)\SSH Agent Monitor` [default location]). See [MonitoringOptions.cs](https://github.com/MA-Maddin/SSHAgentMonitor/blob/main/SSHAgentMonitor/MonitoringOptions.cs) for documentation.

If you want to touch the code and build it yourself, see **Development/Build Requirements** below.

## Setup Prerequisites
- .NET Desktop Runtime 6.0 installed. [Download (6.0.9)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.9-windows-x86-installer)    
Or latest version from here: [Download](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (".NET Desktop Runtime", Windows, x86).

## Development/Build Requirements
### For Worker Service project 
- Visual Studio 2022 (Community or better). [Download](https://visualstudio.microsoft.com/downloads/)
### For Setup project
- Wix Toolset build tools (3.11). [Download](https://wixtoolset.org/releases/)
- Wix Toolset Visual Studio 2022 Extension. [Download](https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2022Extension)
