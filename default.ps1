properties {
  $projectName = "MicroLite"
  $base_dir = Resolve-Path .
}

Task Default -depends RunTests

Task RunTests -Depends Build {
  Write-Host "Running $projectName.Tests" -ForegroundColor Green
  Exec {  & $base_dir\tools\nunit\nunit-console-x86.exe "$base_dir\$projectName.Tests\bin\Release\$projectName.Tests.dll" /nologo /nodots /noxml }
}

Task Build -Depends Clean {
  Write-Host "Building $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Build /p:Configuration=Release /v:quiet }
}

Task Clean {
  Write-Host "Cleaning $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Clean /p:Configuration=Release /v:quiet }
}