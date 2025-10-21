using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RichIZ.Models;
using RichIZ.Services;
using System;
using System.Windows;

namespace RichIZ.ViewModels
{
    public partial class LicenseViewModel : ObservableObject
    {
        private readonly LicenseService _licenseService = new();

        [ObservableProperty]
        private string licenseKey = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string userName = string.Empty;

        [ObservableProperty]
        private LicenseType selectedLicenseType = LicenseType.Monthly;

        [ObservableProperty]
        private string? currentLicenseInfo;

        [ObservableProperty]
        private bool isLicenseValid;

        [ObservableProperty]
        private int remainingDays;

        public LicenseViewModel()
        {
            CheckCurrentLicense();
        }

        [RelayCommand]
        private void GenerateLicense()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                MessageBox.Show("이메일을 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var license = _licenseService.IssueLicense(Email, UserName, SelectedLicenseType);
            LicenseKey = license.LicenseKey;

            MessageBox.Show($"라이센스 키가 생성되었습니다:\n\n{license.LicenseKey}\n\n이메일: {license.UserEmail}\n유형: {license.Type}\n만료일: {license.ExpiryDate:yyyy-MM-dd}",
                "라이센스 생성 완료", MessageBoxButton.OK, MessageBoxImage.Information);

            CheckCurrentLicense();
        }

        [RelayCommand]
        private void ActivateLicense()
        {
            if (string.IsNullOrWhiteSpace(LicenseKey))
            {
                MessageBox.Show("라이센스 키를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var success = _licenseService.ActivateLicense(LicenseKey);

            if (success)
            {
                MessageBox.Show("라이센스가 성공적으로 활성화되었습니다!", "활성화 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                CheckCurrentLicense();
            }
            else
            {
                MessageBox.Show("라이센스 활성화에 실패했습니다.\n\n- 올바른 라이센스 키인지 확인해주세요\n- 만료되지 않은 라이센스인지 확인해주세요\n- 다른 컴퓨터에서 이미 활성화된 키는 사용할 수 없습니다",
                    "활성화 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void CheckCurrentLicense()
        {
            var license = _licenseService.GetCurrentLicense();

            if (license != null)
            {
                IsLicenseValid = !license.IsExpired;
                RemainingDays = _licenseService.GetRemainingDays();
                CurrentLicenseInfo = $"✅ 활성화됨\n\n" +
                    $"유형: {license.Type}\n" +
                    $"발급일: {license.IssuedDate:yyyy-MM-dd}\n" +
                    $"만료일: {license.ExpiryDate:yyyy-MM-dd}\n" +
                    $"남은 기간: {RemainingDays}일\n" +
                    $"이메일: {license.UserEmail}";
            }
            else
            {
                IsLicenseValid = false;
                RemainingDays = 0;
                CurrentLicenseInfo = "⚠️ 활성화된 라이센스가 없습니다.\n\n라이센스를 발급받거나 활성화해주세요.";
            }
        }
    }
}
