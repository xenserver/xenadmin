@powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin
chocolatey feature enable -n allowGlobalConfirmation
choco install cyg-get windows-sdk-8.1 nunit 7zip.commandline
choco install visualstudioexpress2010 -version 10.0.30319.1
choco install notepadplusplus sourcetree beyondcompare | echo "optional devtools install failed"
cyg-get.bat wget curl git unzip zip mc mercurial unzip openssh patch cygrunsrv mkisofs
git config --global push.default simple
