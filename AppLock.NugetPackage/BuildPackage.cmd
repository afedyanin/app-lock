@ECHO OFF

SET MSBUILD="C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

ECHO %DATE% %TIME:~0,8% %1 build started... 
ECHO. 

CALL %MSBUILD% AppLock.NugetPackage.csproj /v:n /m /p:Configuration=Release
CALL ..\.nuget\Nuget.exe pack AppLock.nuspec

ECHO. 
ECHO %DATE% %TIME:~0,8% %1 build finished.

