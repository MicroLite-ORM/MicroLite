$scriptPath = Split-Path $MyInvocation.InvocationName

Import-Module (Join-Path $scriptPath tools\psake\psake.psm1) -ErrorAction SilentlyContinue

Invoke-psake (Join-Path $scriptPath default.ps1)

Remove-Module psake -ErrorAction SilentlyContinue