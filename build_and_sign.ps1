$ErrorActionPreference = "Stop"

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$src  = Join-Path $here "BezpiecznyEgzamin.cs"
$ico  = Join-Path $here "kiosk-shield.ico"
$out  = Join-Path $here "..\..\public\assets\BezpiecznyEgzamin.exe"

$csc = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $csc)) {
    $csc = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"
}

Write-Host "1. Kompilacja BezpiecznyEgzamin.exe..." -ForegroundColor Cyan
if (Test-Path $ico) {
    & $csc /nologo /target:winexe /optimize+ "/win32icon:$ico" "/out:$out" $src
} else {
    & $csc /nologo /target:winexe /optimize+ "/out:$out" $src
}

Write-Host "2. Wyszukiwanie certyfikatu Code Signing (Certum SimplySign / Live Production)..." -ForegroundColor Cyan
# Najpierw szukamy oficjalnego certyfikatu Certum (wystawionego przez Certum Code Signing CA)
$cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert |
    Where-Object { $_.Issuer -match "Certum" -and $_.NotAfter -gt (Get-Date) } |
    Sort-Object NotAfter -Descending |
    Select-Object -First 1

if (-not $cert) {
    Write-Host "   Uwaga: Nie znaleziono aktywnego certyfikatu Certum SimplySign (upewnij sie, ze aplikacja SimplySign Desktop jest zalogowana)." -ForegroundColor Yellow
    $cert = Get-ChildItem Cert:\CurrentUser\My -CodeSigningCert |
        Where-Object { $_.Subject -match "Live Production|Karol Kowalski" -and $_.NotAfter -gt (Get-Date) } |
        Sort-Object NotAfter -Descending |
        Select-Object -First 1
}

if (-not $cert) {
    throw "Brak certyfikatu Code Signing w magazynie Cert:\CurrentUser\My!"
}

Write-Host "   Uzywany certyfikat: $($cert.Subject) (Wystawca: $($cert.Issuer))" -ForegroundColor Green

Write-Host "3. Podpisywanie cyfrowe SHA-256 ze znacznikiem czasu Certum RFC3161..." -ForegroundColor Cyan
$sig = Set-AuthenticodeSignature -FilePath $out -Certificate $cert -HashAlgorithm SHA256 -IncludeChain All -TimestampServer "http://time.certum.pl"
if ($sig.Status -ne "Valid" -and $sig.Status -ne "UnknownError") {
    $sig = Set-AuthenticodeSignature -FilePath $out -Certificate $cert -HashAlgorithm SHA256 -IncludeChain All -TimestampServer "http://timestamp.digicert.com"
}

Get-AuthenticodeSignature $out | Format-List Status, StatusMessage, SignerCertificate, TimeStamperCertificate
Write-Host "GOTOWE! Plik zapisany w: $out" -ForegroundColor Green
