using System;
using System.Windows;
using RichIZ.Data;
using RichIZ.Services;

namespace RichIZ
{
    public partial class App : Application
    {
        public App()
        {
            // JSON 파일 기반 저장소 사용 - 별도 초기화 불필요
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 데이터베이스가 초기화되었으므로 이제 MainWindow를 생성
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // 라이센스 검증 (강화된 라이센스 시스템) - MainWindow 생성 후 체크
            try
            {
                var licenseService = new LicenseService();
                if (!licenseService.ValidateLicense())
                {
                    // 라이센스가 없으면 체험판 자동 발급 (1회만)
                    var currentLicense = licenseService.GetCurrentLicense();
                    if (currentLicense == null)
                    {
                        // 첫 실행: 3일 체험판 자동 발급
                        licenseService.IssueLicense(
                            email: "trial@richiz.local",
                            userName: Environment.UserName,
                            type: Models.LicenseType.Trial
                        );
                        licenseService.ActivateLicense(licenseService.GetCurrentLicense()?.LicenseKey ?? "");

                        MessageBox.Show(
                            "RichIZ에 오신 것을 환영합니다!\n\n3일 무료 체험이 시작되었습니다.\n체험 기간이 끝나면 정식 라이센스가 필요합니다.",
                            "체험판 활성화",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        // 라이센스 만료됨
                        MessageBox.Show(
                            "라이센스가 만료되었습니다.\n\n정식 라이센스를 구매하여 계속 사용하세요.\n라이센스 메뉴에서 활성화할 수 있습니다.",
                            "라이센스 만료",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        Shutdown();
                        return;
                    }
                }
            }
            catch
            {
                // 라이센스 체크 실패 시 무시하고 계속 진행 (첫 실행일 가능성)
            }

            // 자동 백업 실행
            try
            {
                var backupService = new BackupService();
                backupService.AutoBackup();
            }
            catch
            {
                // 백업 실패는 무시
            }
        }
    }
}
