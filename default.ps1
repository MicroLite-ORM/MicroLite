properties {
  $base_dir = Resolve-Path .
  $build_dir = "$base_dir\build\"
  $projectName = "MicroLite"
}

Task Default -depends RunTests

Task RunTests -Depends Build {
  Write-Host "Running $projectName.Tests" -ForegroundColor Green
  Exec {  & $base_dir\tools\nunit\nunit-console-x86.exe "$build_dir\$projectName.Tests.dll" /nologo /nodots /noxml }
}

Task Build -Depends Clean {
  Remove-Item -force -recurse $build_dir -ErrorAction SilentlyContinue

  Write-Host "Building $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Build /p:"Configuration=Release;OutDir=$build_dir" /v:quiet }
}

Task Clean {
  Write-Host "Cleaning $projectName.sln" -ForegroundColor Green
  Exec { msbuild "$projectName.sln" /t:Clean /p:Configuration=Release /v:quiet }
}