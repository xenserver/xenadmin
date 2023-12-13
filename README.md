XenCenter
=========

Please note that as of December 2023 this copy of the XenCenter repository is
considered archived. As such it will not reflect the latest state of XenCenter
development, and any pull requests will not be reviewed/merged. If you have any
feedback regarding XenCenter, please send it to feedback@xenserver.com.

---

Overview
--------

XenCenter is a Windows-based management tool for XenServer and Citrix Hypervisor
environments, which enables users to manage and monitor server and resource pools,
and to deploy, monitor, manage, and migrate virtual machines.

XenCenter is written in C#.

License
-------

This code is licensed under the BSD 2-Clause license. Please see the
[LICENSE](LICENSE) file for more information.

How to build XenCenter
----------------------

To build XenCenter, you need

* the source from xenadmin repository
* Visual Studio 2022
* .NET Framework 4.8

and also some libraries which we do not store in the source tree:

* CookComputing.XmlRpcV2.dll
* Newtonsoft.Json.dll
* DiscUtils.dll
* ICSharpCode.SharpZipLib.dll
* log4net.dll

You can find the source code of these libraries (along with some patches) in
[dotnet-packages](https://github.com/xenserver/dotnet-packages) repository.

To run the [NUnit](http://www.nunit.org/) tests you will need the following libraries:

* nunit.framework.dll
* Moq.dll

which can be obtained from <http://www.nuget.org/>.

Note that the build script assumes that you have added MSBuild's location (usually
`C:\Program Files\Microsoft Visual Studio\2022\<edition>\MSBuild\Current\Bin`)
to your `PATH` environment variable.
