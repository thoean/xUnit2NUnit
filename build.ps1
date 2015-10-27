$ErrorActionPreference = "Stop"

$dnvm = Join-Path $env:USERPROFILE -ChildPath "\.dnx\bin\"
$dnx = Join-Path $env:USERPROFILE -ChildPath "\.dnx\runtimes\dnx-clr-win-x64.1.0.0-beta8\bin\"
$env:Path += ";" + $dnvm + ";" + $dnx

function Install-Environment {
	&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}
    dnvm install 1.0.0-beta8 -arch x64 -runtime clr
    dnvm list
}

function Restore-Dependencies {
    dnu restore src
}

function Run-UnitTests {
    # todo
}

function Run-ServiceTests {
    # start the service in the background
    $cwd = Get-Location
    Start-Job -Name ConversionService -ScriptBlock ({param($path)
        Set-Location -Path $path
        .\tmp\service\approot\web.cmd
    }) -ArgumentList $cwd

    # wait until the service is started; we could probably ping the service to ensure it's started
    Start-Sleep -Seconds 5

    # run the service tests
    dnx -p .\test\Service\ServiceTests\project.json test

    # stop the job and also write the output
    Get-Job -Name ConversionService | Stop-Job
    Get-Job -Name ConversionService | Receive-Job
}

function Invoke-Build {
    dnu publish .\src\Service\project.json --out .\tmp\service --no-source --runtime dnx-clr-win-x64.1.0.0-beta8 --configuration Release --quiet
}

function Create-TempDirectories {
    if (Test-Path tmp) {
        Remove-Item tmp -Force -Recurse
    }
    mkdir tmp
    mkdir tmp\input
    mkdir tmp\output
}

Install-Environment
Restore-Dependencies
Create-TempDirectories
Invoke-Build
Run-UnitTests
Run-ServiceTests