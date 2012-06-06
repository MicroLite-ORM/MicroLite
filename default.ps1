properties {
  $baseDir = Resolve-Path .
  $buildDir = "$baseDir\build"
  $buildDir40 = "$buildDir\4.0\"
  $projectName = "MicroLite"
}

Task Default -depends RunTests

Task RunTests -Depends Build {
  Write-Host "Running $projectName.Tests" -ForegroundColor Green
  Exec {  & $baseDir\tools\nunit\nunit-console-x86.exe "$buildDir40\$projectName.Tests.dll" /nologo /nodots /noxml }
}

Task Build -Depends Clean {
  Remove-Item -force -recurse $buildDir -ErrorAction SilentlyContinue
 
  Write-Host "Building $projectName.sln for .net 4.0" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /target:Build "/property:Configuration=Release;OutDir=$buildDir40;TargetFrameworkVersion=v4.0" /verbosity:quiet }
}

Task Clean {
  Write-Host "Cleaning $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Clean /p:Configuration=Release /v:quiet }
}