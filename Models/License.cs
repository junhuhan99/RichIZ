using System;
using System.ComponentModel.DataAnnotations;

namespace RichIZ.Models
{
    public class License
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LicenseKey { get; set; } = string.Empty;

        [Required]
        public string UserEmail { get; set; } = string.Empty;

        public string? UserName { get; set; }

        [Required]
        public LicenseType Type { get; set; }

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime? ActivatedDate { get; set; }

        public string? MachineId { get; set; }

        public bool IsExpired => DateTime.Now > ExpiryDate;
    }

    public enum LicenseType
    {
        Trial,      // 7일 체험판
        Monthly,    // 월간 구독
        Yearly,     // 연간 구독
        Lifetime    // 평생 라이센스
    }
}
