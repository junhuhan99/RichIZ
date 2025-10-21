$LICENSE_SECRET = 'RichIZ_2025_SecretKey_FinancialApp'
$email = 'premium@richiz.com'
$type = 'Lifetime'
$timestamp = [DateTime]::Now.Ticks
$data = "$email|$type|$timestamp|$LICENSE_SECRET"
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$hash = $sha256.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($data))
$base64 = [Convert]::ToBase64String($hash)
$licenseKey = $base64.Replace('+', '').Replace('/', '').Replace('=', '').Substring(0, 25).ToUpper()
$formattedKey = $licenseKey.Substring(0,5) + '-' + $licenseKey.Substring(5,5) + '-' + $licenseKey.Substring(10,5) + '-' + $licenseKey.Substring(15,5) + '-' + $licenseKey.Substring(20,5)
Write-Host '==========================================='
Write-Host 'RichIZ Premium Lifetime License Key'
Write-Host '==========================================='
Write-Host "License Key: $formattedKey"
Write-Host "Email: $email"
Write-Host 'Type: Lifetime (영구 라이센스)'
$dateStr = [DateTime]::Now.ToString('yyyy-MM-dd HH:mm:ss')
Write-Host "Generated: $dateStr"
Write-Host '==========================================='
Write-Host ''
Write-Host "사용자님의 프리미엄 영구 라이센스 키: $formattedKey"
