param(
  [string]$projectName = $(throw "project name is a required parameter."),
  [string]$version = $(throw "version is a required parameter.")
)

$scriptPath = Split-Path $MyInvocation.InvocationName
$buildDir = "$scriptPath\build"
$nuGetExe = "$scriptPath\tools\NuGet.exe"
$nuSpec = "$scriptPath\$projectName.nuspec"
$nuGetPackage = "$buildDir\$projectName.$version.nupkg"

# Run the standard build script to create the release binaries
Invoke-Expression "$scriptPath\build.ps1"

Write-Host "Update NuGet.exe" -ForegroundColor Green
& $nuGetExe Update -self

if (Test-Path "$nuGetExe.old")
{
  Remove-Item -force "$nuGetExe.old" -ErrorAction SilentlyContinue
}

Write-Host "Pack $nuSpec -> $nuGetPackage" -ForegroundColor Green
& $nuGetExe Pack $nuSpec -Version $version -OutputDirectory $buildDir -BasePath $buildDir

Write-Host "Push $nuGetPackage -> http://nuget.org" -ForegroundColor Green
& $nuGetExe Push $nuGetPackage