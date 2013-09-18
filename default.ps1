properties {
  $projectName = "MicroLite"
  $baseDir = Resolve-Path .
  $buildDir = "$baseDir\build"
  $buildDir35 = "$buildDir\3.5\"
  $buildDir40 = "$buildDir\4.0\"
  $buildDir45 = "$buildDir\4.5\"
  $helpDir = "$buildDir\help\"
}

Task Default -depends RunTests, Build35, Build40, Build45, BuildHelp

Task BuildHelp {
  Remove-Item -force -recurse $helpDir -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.shfbproj" -ForegroundColor Green
  Exec { msbuild "$projectName.shfbproj" }  
}

Task Build35 {
  Remove-Item -force -recurse $buildDir35 -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.csproj for .net 3.5" -ForegroundColor Green
  Exec { msbuild "$projectName\$projectName.csproj" /target:Rebuild "/property:Configuration=Release;DefineConstants=NET_3_5;OutDir=$buildDir35;TargetFrameworkVersion=v3.5;TargetFrameworkProfile=Client" /verbosity:quiet }
}

Task Build40 {
  Remove-Item -force -recurse $buildDir40 -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.csproj for .net 4.0" -ForegroundColor Green
  Exec { msbuild "$projectName\$projectName.csproj" /target:Rebuild "/property:Configuration=Release;DefineConstants=NET_4_0;OutDir=$buildDir40;TargetFrameworkVersion=v4.0;TargetFrameworkProfile=Client" /verbosity:quiet }
}

Task Build45 {
  Remove-Item -force -recurse $buildDir45 -ErrorAction SilentlyContinue
  
  Write-Host "Building $projectName.csproj for .net 4.5" -ForegroundColor Green
  Exec { msbuild "$projectName\$projectName.csproj" /target:Rebuild "/property:Configuration=Release;DefineConstants=NET_4_5;OutDir=$buildDir45;TargetFrameworkVersion=v4.5" /verbosity:quiet }
}

Task RunTests -Depends Build {
  Write-Host "Running $projectName.Tests" -ForegroundColor Green
  Exec {  & $baseDir\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe "$baseDir\$projectName.Tests\bin\Release\$projectName.Tests.dll" }
}

Task Build -Depends Clean {
  Write-Host "Building $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /target:Build /property:Configuration=Release /verbosity:quiet }  
}

Task Clean {
  Write-Host "Cleaning $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Clean /p:Configuration=Release /v:quiet }
}