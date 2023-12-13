namespace JurayKV.Application.Queries.UserManagerQueries
{
    public class UserManagerDetailsDto
    {
        public Guid Id { get; set; }
        public string Fullname { get; set; } 
        public string Surname { get; set; } 
        public string Firstname { get; set; } 
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } 
        public bool IsDisabled { get; set; }
        public DateTime CreationUTC { get; set; }

        public DateTime? LastLoggedInAtUtc { get; set; }

        public decimal WalletBalance { get;set; }
        public bool DisableEmailNotification { get; set; }
        public bool IsCompany {  get; set; }
        public string RefferedBy { get; set; }
    }
}
