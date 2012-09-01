param(
  [string]$version,
  [bool]$push
)

$projectName = "MicroLite"

$scriptPath = Split-Path $MyInvocation.InvocationName
$buildDir = "$scriptPath\build"
$nuGetExe = "$scriptPath\.nuget\NuGet.exe"
$nuSpec = "$scriptPath\$projectName.nuspec"
$nuGetPackage = "$buildDir\$projectName.$version.nupkg"

# Run the psake build script to create the release binaries
Import-Module (Join-Path $scriptPath tools\psake\psake.psm1) -ErrorAction SilentlyContinue

Invoke-psake (Join-Path $scriptPath default.ps1)

Remove-Module psake -ErrorAction SilentlyContinue

if ($version)
{
	Write-Host "Update NuGet.exe" -ForegroundColor Green
	& $nuGetExe Update -self

	if (Test-Path "$nuGetExe.old")
	{
  		Remove-Item -force "$nuGetExe.old" -ErrorAction SilentlyContinue
	}

	Write-Host "Pack $nuSpec -> $nuGetPackage" -ForegroundColor Green
	& $nuGetExe Pack $nuSpec -Version $version -OutputDirectory $buildDir -BasePath $buildDir

	if($push)
	{
		Write-Host "Push $nuGetPackage -> http://nuget.org" -ForegroundColor Green
		& $nuGetExe Push $nuGetPackage
	}
}