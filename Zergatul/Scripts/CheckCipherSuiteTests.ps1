Add-Type -Path '../bin/Debug/Zergatul.dll'
Add-Type -Path '../../Zergatul.Tls.Tests/bin/Debug/Zergatul.Tls.Tests.dll'

$supported = [Zergatul.Network.Tls.TlsStream]::SupportedCipherSuites
$testInstance = New-Object Zergatul.Tls.Tests.CipherSuiteTests
$tests = Get-Member -InputObject $testInstance -MemberType Methods

foreach ($cs in $supported)
{
    $methods = $tests | Where-Object { $_.Name -eq $cs.ToString() }
    if ($methods -eq $null)
    {
        Write-Host $cs
    }
}