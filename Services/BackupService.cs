using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace RichIZ.Services
{
    public class BackupService
    {
        private readonly string _appDataPath;
        private readonly string _backupPath;

        public BackupService()
        {
            _appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RichIZ"
            );
            _backupPath = Path.Combine(_appDataPath, "Backups");
            Directory.CreateDirectory(_backupPath);
        }

        /// <summary>
        /// 데이터베이스 백업
        /// </summary>
        public string CreateBackup()
        {
            var dbPath = Path.Combine(_appDataPath, "richiz.db");
            if (!File.Exists(dbPath))
                throw new FileNotFoundException("데이터베이스 파일을 찾을 수 없습니다.");

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"RichIZ_Backup_{timestamp}.zip";
            var backupFilePath = Path.Combine(_backupPath, backupFileName);

            using (var archive = ZipFile.Open(backupFilePath, ZipArchiveMode.Create))
            {
                archive.CreateEntryFromFile(dbPath, "richiz.db");
            }

            // 오래된 백업 삭제 (30일 이상)
            CleanOldBackups(30);

            return backupFilePath;
        }

        /// <summary>
        /// 백업 복원
        /// </summary>
        public bool RestoreBackup(string backupFilePath)
        {
            if (!File.Exists(backupFilePath))
                return false;

            var dbPath = Path.Combine(_appDataPath, "richiz.db");

            // 현재 DB 백업
            var currentBackup = Path.Combine(_appDataPath, $"richiz_before_restore_{DateTime.Now:yyyyMMddHHmmss}.db");
            if (File.Exists(dbPath))
            {
                File.Copy(dbPath, currentBackup, true);
            }

            try
            {
                using (var archive = ZipFile.OpenRead(backupFilePath))
                {
                    var entry = archive.GetEntry("richiz.db");
                    if (entry != null)
                    {
                        entry.ExtractToFile(dbPath, true);
                        return true;
                    }
                }
            }
            catch
            {
                // 복원 실패 시 원래 DB 복구
                if (File.Exists(currentBackup))
                {
                    File.Copy(currentBackup, dbPath, true);
                }
                return false;
            }

            return false;
        }

        /// <summary>
        /// 자동 백업 (앱 시작 시 실행)
        /// </summary>
        public void AutoBackup()
        {
            try
            {
                var lastBackup = GetLatestBackup();

                // 마지막 백업이 없거나 1일 이상 지난 경우
                if (lastBackup == null || (DateTime.Now - File.GetCreationTime(lastBackup)).Days >= 1)
                {
                    CreateBackup();
                }
            }
            catch
            {
                // 자동 백업 실패는 무시
            }
        }

        /// <summary>
        /// 백업 목록 조회
        /// </summary>
        public string[] GetBackupList()
        {
            return Directory.GetFiles(_backupPath, "RichIZ_Backup_*.zip")
                           .OrderByDescending(f => File.GetCreationTime(f))
                           .ToArray();
        }

        /// <summary>
        /// 최신 백업 파일 조회
        /// </summary>
        public string? GetLatestBackup()
        {
            return GetBackupList().FirstOrDefault();
        }

        /// <summary>
        /// 오래된 백업 파일 삭제
        /// </summary>
        private void CleanOldBackups(int daysToKeep)
        {
            var backups = GetBackupList();
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var backup in backups)
            {
                if (File.GetCreationTime(backup) < cutoffDate)
                {
                    try
                    {
                        File.Delete(backup);
                    }
                    catch { }
                }
            }
        }
    }
}
