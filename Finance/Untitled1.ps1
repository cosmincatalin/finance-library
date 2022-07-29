[xml]$Data=Get-Content .nuspec
Write-Output $Data.package.metadata.version