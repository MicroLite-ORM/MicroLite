param(
	[string]$version = $(throw "version is a required parameter.")
)

$scriptPath = Split-Path $MyInvocation.InvocationName
$buildDir = "$scriptPath\build"
$nuGetExe = "$scriptPath\tools\NuGet.exe"
$nuspec = "$scriptPath\MicroLite.nuspec"

& $nuGetExe Update -self

if (Test-Path "$nuGetExe.old")
{
  Remove-Item -force "$nuGetExe.old" -ErrorAction SilentlyContinue
}

& $nuGetExe pack $nuspec -Version $version -OutputDirectory $buildDir -BasePath $buildDir