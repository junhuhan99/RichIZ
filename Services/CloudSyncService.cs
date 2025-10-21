using System;
using System.IO;
using System.Threading.Tasks;

namespace RichIZ.Services
{
    public class CloudSyncService
    {
        private readonly string _cloudPath;
        private readonly BackupService _backupService;

        public CloudSyncService()
        {
            // OneDrive 경로 찾기
            var oneDrivePath = Environment.GetEnvironmentVariable("OneDrive");
            if (string.IsNullOrEmpty(oneDrivePath))
            {
                // OneDrive가 없으면 기본 Documents 폴더 사용
                oneDrivePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            _cloudPath = Path.Combine(oneDrivePath, "RichIZ_Cloud");
            Directory.CreateDirectory(_cloudPath);

            _backupService = new BackupService();
        }

        /// <summary>
        /// 클라우드로 동기화 (백업 업로드)
        /// </summary>
        public async Task<bool> SyncToCloud()
        {
            try
            {
                // 백업 생성
                var backupFile = _backupService.CreateBackup();

                // 클라우드 폴더로 복사
                var cloudBackupFile = Path.Combine(_cloudPath, Path.GetFileName(backupFile));
                await Task.Run(() => File.Copy(backupFile, cloudBackupFile, true));

                // 메타데이터 저장
                var metadataFile = Path.Combine(_cloudPath, "last_sync.txt");
                await File.WriteAllTextAsync(metadataFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 클라우드에서 복원
        /// </summary>
        public async Task<bool> RestoreFromCloud()
        {
            try
            {
                // 최신 백업 파일 찾기
                var backupFiles = Directory.GetFiles(_cloudPath, "RichIZ_Backup_*.zip");
                if (backupFiles.Length == 0)
                    return false;

                Array.Sort(backupFiles);
                var latestBackup = backupFiles[^1]; // 가장 최근 파일

                // 복원
                await Task.Run(() => _backupService.RestoreBackup(latestBackup));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 자동 동기화 (설정 시간마다)
        /// </summary>
        public async Task AutoSync(int intervalHours = 24)
        {
            while (true)
            {
                try
                {
                    await SyncToCloud();
                }
                catch
                {
                    // 자동 동기화 실패는 무시
                }

                await Task.Delay(TimeSpan.FromHours(intervalHours));
            }
        }

        /// <summary>
        /// 마지막 동기화 시간 조회
        /// </summary>
        public DateTime? GetLastSyncTime()
        {
            try
            {
                var metadataFile = Path.Combine(_cloudPath, "last_sync.txt");
                if (!File.Exists(metadataFile))
                    return null;

                var content = File.ReadAllText(metadataFile);
                return DateTime.Parse(content);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 클라우드 경로 가져오기
        /// </summary>
        public string GetCloudPath() => _cloudPath;
    }
}
