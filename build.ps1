$scriptPath = Split-Path $MyInvocation.InvocationName

Import-Module (Join-Path $scriptPath tools\psake\psake.psm1) -ErrorAction SilentlyContinue

Invoke-psake (Join-Path $scriptPath default.ps1) -framework '4.0'

Remove-Module psake -ErrorAction SilentlyContinue