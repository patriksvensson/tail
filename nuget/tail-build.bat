echo off
cls

:: Build the project
call %windir%\Microsoft.NET\Framework\v4.0.30319\msbuild ../src/Tail.sln /p:Configuration=Release /p:Platform="Any CPU"

:: Remove the existing files
cd nuget
if exist tail.extensibility.1.0.0.nupkg del tail.extensibility.1.0.0.nupkg
if exist tail.extensibility.1.0.0\lib rmdir /S /Q tail.extensibility.1.0.0\lib
mkdir tail.extensibility.1.0.0\lib
mkdir tail.extensibility.1.0.0\lib\net40

:: Copy files
copy tail.nuspec tail.extensibility.1.0.0\
copy ..\src\Tail.Extensibility\bin\Release\Tail.Extensibility.dll tail.extensibility.1.0.0\lib\net40
copy ..\LICENSE tail.extensibility.1.0.0

:: Build the NuGet package
nuget pack tail.extensibility.1.0.0\tail.nuspec