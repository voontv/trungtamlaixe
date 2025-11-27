using System;
using System.Collections.Generic;
namespace Ttlaixe.LibsStartup
{
    public class BackupHistory
    {
        public string TenFolderNameBackup { get; set; }

        public List<int> ThuBackup { get; set; }

        public float GioBackupFileVib { get; set; }

        public float KhoangCachGioBackup { get; set; }

        public int KcNgayBackupVaHienTai { get; set; }

        public string? FolderGoc {  get; set; }
    }

    public class FileInforNeedSave
    {
        public DateTime CreationTime { get; set; }

        public DateTime LastWriteTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public string Name { get; set; }

        public long Length { get; set; }
    } 

}
