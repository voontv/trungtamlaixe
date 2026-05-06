namespace Ttlaixe.DTO.response
{
    public class GiaoVienResponse
    {
        /// <summary>
        /// Mã Giáo viên = &lt;MaCSDT&gt;&lt;Số tự tăng có giá trị từ 001-999&gt;
        /// </summary>
        public string MaGv { get; set; }


        /// <summary>
        /// Họ và tên đệm của Giáo viên. 
        /// </summary>
        public string HoTenDem { get; set; }

        /// <summary>
        /// Tên của Giáo viên
        /// </summary>
        public string TenGv { get; set; }

    }
}
