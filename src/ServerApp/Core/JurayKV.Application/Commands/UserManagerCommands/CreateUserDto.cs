namespace JurayKV.Application.Commands.UserManagerCommands
{
    public class CreateUserDto
    {
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
         public bool Comfirm { get; set; }
        public string Role { get; set; }
     }
}
