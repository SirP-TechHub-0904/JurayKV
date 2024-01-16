using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.Application.Queries.UserManagerQueries
{
    public class UserManagerListDto
    {
        public Guid Id { get; set; }
        public string Fullname {  get; set; }
        public string Email { get;set; }
        public string PhoneNumber { get;set; }
        public AccountStatus AccountStatus { get; set; }
        public string? Role { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationUTC { get; set; }
        public bool Verified { get; set; }
        public DateTime? LastLoggedInAtUtc { get; set; }
        public string? VerificationCode { get; set; }

    }
}
