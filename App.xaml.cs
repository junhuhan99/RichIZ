using System.Windows;
using RichIZ.Data;
using RichIZ.Services;

namespace RichIZ
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 데이터베이스 초기화
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
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
