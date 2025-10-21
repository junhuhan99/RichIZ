using RichIZ.Data;
using RichIZ.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Management;

namespace RichIZ.Services
{
    public class LicenseService
    {
        private const string LICENSE_SECRET = "RichIZ_2025_SecretKey_FinancialApp";

        /// <summary>
        /// 새 라이센스 키 생성
        /// </summary>
        public string GenerateLicenseKey(string email, LicenseType type)
        {
            var timestamp = DateTime.Now.Ticks;
            var data = $"{email}|{type}|{timestamp}|{LICENSE_SECRET}";

            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            var licenseKey = Convert.ToBase64String(hash)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 25)
                .ToUpper();

            // 형식: XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
            return $"{licenseKey.Substring(0, 5)}-{licenseKey.Substring(5, 5)}-{licenseKey.Substring(10, 5)}-{licenseKey.Substring(15, 5)}-{licenseKey.Substring(20, 5)}";
        }

        /// <summary>
        /// 라이센스 발급
        /// </summary>
        public License IssueLicense(string email, string userName, LicenseType type)
        {
            var licenseKey = GenerateLicenseKey(email, type);
            var issuedDate = DateTime.Now;
            var expiryDate = type switch
            {
                LicenseType.Trial => issuedDate.AddDays(7),
                LicenseType.Monthly => issuedDate.AddMonths(1),
                LicenseType.Yearly => issuedDate.AddYears(1),
                LicenseType.Lifetime => issuedDate.AddYears(100),
                _ => issuedDate.AddDays(7)
            };

            var license = new License
            {
                LicenseKey = licenseKey,
                UserEmail = email,
                UserName = userName,
                Type = type,
                IssuedDate = issuedDate,
                ExpiryDate = expiryDate,
                IsActive = true
            };

            using var context = new AppDbContext();
            context.Licenses.Add(license);
            context.SaveChanges();

            return license;
        }

        /// <summary>
        /// 라이센스 활성화
        /// </summary>
        public bool ActivateLicense(string licenseKey)
        {
            using var context = new AppDbContext();
            var license = context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);

            if (license == null)
                return false;

            if (license.IsExpired)
                return false;

            if (license.ActivatedDate != null && license.MachineId != GetMachineId())
                return false; // 다른 기기에서 이미 활성화됨

            license.IsActive = true;
            license.ActivatedDate = DateTime.Now;
            license.MachineId = GetMachineId();
            context.SaveChanges();

            return true;
        }

        /// <summary>
        /// 라이센스 검증
        /// </summary>
        public bool ValidateLicense()
        {
            using var context = new AppDbContext();
            var machineId = GetMachineId();

            var license = context.Licenses
                .Where(l => l.MachineId == machineId && l.IsActive)
                .OrderByDescending(l => l.ExpiryDate)
                .FirstOrDefault();

            if (license == null)
                return false;

            if (license.IsExpired)
            {
                license.IsActive = false;
                context.SaveChanges();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 현재 라이센스 정보 조회
        /// </summary>
        public License? GetCurrentLicense()
        {
            using var context = new AppDbContext();
            var machineId = GetMachineId();

            return context.Licenses
                .Where(l => l.MachineId == machineId && l.IsActive)
                .OrderByDescending(l => l.ExpiryDate)
                .FirstOrDefault();
        }

        /// <summary>
        /// 머신 ID 생성 (하드웨어 기반)
        /// </summary>
        private string GetMachineId()
        {
            try
            {
                var cpuId = GetCpuId();
                var diskId = GetDiskId();
                var data = $"{cpuId}|{diskId}";

                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash).Substring(0, 32);
            }
            catch
            {
                // 머신 ID 생성 실패 시 환경 변수 기반 ID 사용
                return Environment.MachineName + Environment.UserName;
            }
        }

        private string GetCpuId()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                var mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    return mo["ProcessorId"]?.ToString() ?? "UNKNOWN";
                }
            }
            catch { }
            return "UNKNOWN";
        }

        private string GetDiskId()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia");
                var mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    return mo["SerialNumber"]?.ToString()?.Trim() ?? "UNKNOWN";
                }
            }
            catch { }
            return "UNKNOWN";
        }

        /// <summary>
        /// 남은 라이센스 일수
        /// </summary>
        public int GetRemainingDays()
        {
            var license = GetCurrentLicense();
            if (license == null) return 0;

            var remaining = (license.ExpiryDate - DateTime.Now).Days;
            return remaining > 0 ? remaining : 0;
        }
    }
}
