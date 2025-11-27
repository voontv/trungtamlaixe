using System;

namespace Ttlaixe.DTO.response
{
    public class SysUserResponse
    {

        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string UserPassword { get; set; }

        public bool Suspend { get; set; }

        public string UserPosition { get; set; }

        public bool? AllowChangePassword { get; set; }

        public int? ExpirationDays { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool EncryptionPassword { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string FaxNumber { get; set; }

        public DateTime? CreationDate { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LoginTime { get; set; }

        public string Token { get; set; }
    }
}
