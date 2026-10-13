using System;
using System.Collections.Generic;

namespace Ttlaixe.DTO.request
{
    /// <summary>
    /// DTO cập nhật thủ công kết quả đào tạo.
    /// Semantics FULL UPDATE giống usp_NguoiLX_KetQuaDaoTao_CSDT_CapNhat.
    /// </summary>
    public class NguoiLxHoSoKetQuaUpdateRequest
    {
        public string MaDk { get; set; }
        public string MaKhoaHoc { get; set; }

        // Store nhận varchar(1): "1", "0" hoặc null/rỗng.
        public string KqLyThuyet { get; set; }
        public string KqThucHanh { get; set; }
        public string KetLuanCsdt { get; set; }

        public string TgBatDau { get; set; }
        public string TgKetThuc { get; set; }

        // SQL Server float -> dùng double? ở DTO.
        public double? DiemKqLyThuyet { get; set; }
        public double? DiemKqThucHanh { get; set; }
        public double? DiemKqMoPhong { get; set; }
        public double? DiemKqHinh { get; set; }
        public double? DiemKqTienLui { get; set; }

        public double? TgThucHanhDuong { get; set; }
        public double? TgThucHanhHinh { get; set; }
        public double? TongQdThucHanh { get; set; }
        public double? QdThucHanhHinh { get; set; }

        public string SoGiayCntn { get; set; }
        public DateTime? NgayRaQdtn { get; set; }
    }
}

namespace Ttlaixe.DTO.response
{
    public class BangDiemImportResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string SheetName { get; set; }
        public int HeaderRow { get; set; }
        public int DataStartRow { get; set; }
        public int TotalRows { get; set; }
        public int UpdatedRows { get; set; }
        public int SkippedRows { get; set; }
        public List<BangDiemImportIssue> Errors { get; set; } = new List<BangDiemImportIssue>();
        public List<BangDiemImportIssue> Warnings { get; set; } = new List<BangDiemImportIssue>();
    }

    public class BangDiemImportIssue
    {
        public int? ExcelRow { get; set; }
        public string MaDk { get; set; }
        public string Message { get; set; }
    }
}
