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

            // MainWindow 생성 및 표시
            var mainWindow = new MainWindow();
            mainWindow.Show();

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
