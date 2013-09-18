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
$date = Get-Date
$gitDir = $scriptPath + "\.git"
$commit = git --git-dir $gitDir log -1 --pretty=format:%h

function UpdateAssemblyInfoFiles ([string] $buildVersion)
{
	$assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
	$infoVersionPattern = 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}(.*)"\)'
	$copyrightPattern = 'AssemblyCopyright\(".+"\)'
	$assemblyVersion = 'AssemblyVersion("' + $buildVersion.SubString(0, 3) + '.0.0")';
	$fileVersion = 'AssemblyFileVersion("' + $buildVersion.SubString(0, 5) + '.0")';
	$infoVersion = 'AssemblyInformationalVersion("' + $buildVersion + ' (' + $commit + ')")';
	$copyright = 'AssemblyCopyright("Copyright 2012-' + $date.Year + ' MicroLite Project Contributors all rights reserved.")';
	
	Get-ChildItem $scriptPath -r -filter AssemblyInfo.cs | ForEach-Object {
		$filename = $_.Directory.ToString() + '\' + $_.Name
		$filename + ' -> ' + $buildVersion
			
		(Get-Content $filename) | ForEach-Object {
			% {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
			% {$_ -replace $fileVersionPattern, $fileVersion } |
			% {$_ -replace $infoVersionPattern, $infoVersion } |
			% {$_ -replace $copyrightPattern, $copyright }
		} | Set-Content $filename -Encoding UTF8
	}
}

if ($version)
{
	UpdateAssemblyInfoFiles($version)
}

# Run the psake build script to create the release binaries
Import-Module (Join-Path $scriptPath packages\psake.4.2.0.1\tools\psake.psm1) -ErrorAction SilentlyContinue

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