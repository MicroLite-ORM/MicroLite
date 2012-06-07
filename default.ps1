properties {
  $baseDir = Resolve-Path .
  $buildDir = "$baseDir\build"
  $buildDir35 = "$buildDir\3.5\"
  $buildDir40 = "$buildDir\4.0\"
  $projectName = "MicroLite"
}

Task Default -depends RunTests, Build35, Build40

Task Build35 {
  Remove-Item -force -recurse $buildDir35 -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.csproj for .net 3.5" -ForegroundColor Green
  Exec { msbuild "$projectName\$projectName.csproj" /target:Rebuild "/property:Configuration=Release;OutDir=$buildDir35;TargetFrameworkVersion=v3.5;TargetFrameworkProfile=Client" /verbosity:quiet }
}

Task Build40 {
  Remove-Item -force -recurse $buildDir40 -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.csproj for .net 4.0" -ForegroundColor Green
  Exec { msbuild "$projectName\$projectName.csproj" /target:Rebuild "/property:Configuration=Release;OutDir=$buildDir40;TargetFrameworkVersion=v4.0;TargetFrameworkProfile=Client" /verbosity:quiet }
}

Task RunTests -Depends Build {
  Write-Host "Running $projectName.Tests" -ForegroundColor Green
  Exec {  & $baseDir\tools\nunit\nunit-console-x86.exe "$baseDir\MicroLite.Tests\bin\Release\$projectName.Tests.dll" /nologo /nodots /noxml }
}

Task Build -Depends Clean {
  Write-Host "Building $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /target:Build /property:Configuration=Release /verbosity:quiet }  
}

Task Clean {
  Remove-Item -force -recurse $buildDir -ErrorAction SilentlyContinue

  Write-Host "Cleaning $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Clean /p:Configuration=Release /v:quiet }
}