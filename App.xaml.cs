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
            // XAML이 로드되기 전에 데이터베이스 초기화
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    context.Database.EnsureCreated();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터베이스 초기화 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 데이터베이스가 초기화되었으므로 이제 MainWindow를 생성
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
