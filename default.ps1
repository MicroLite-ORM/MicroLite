Task Default -depends Clean, Build

Task Build -Depends Clean {
  Write-Host "Building MicroLite.sln" -ForegroundColor Green
  Exec { msbuild "MicroLite.sln" /t:Build /p:Configuration=Release /v:quiet }
}

Task Clean {
  Write-Host "Cleaning MicroLite.sln" -ForegroundColor Green
  Exec { msbuild "MicroLite.sln" /t:Clean /p:Configuration=Release /v:quiet }
}