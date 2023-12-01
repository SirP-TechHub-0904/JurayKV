using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JurayKV.Application.Commands.UserManagerCommands
{
    public class UpdateUserDto
    {
        public string SurName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime? LastLoggedInAtUtc { get; set; }
        public DateTime CreationUTC { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool DisableEmailNotification { get; set; }

    }
}
