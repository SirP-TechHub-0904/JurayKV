using System;
using Microsoft.AspNetCore.Identity;

namespace JurayKV.Domain.Aggregates.IdentityAggregate;

public class ApplicationUser : IdentityUser<Guid>
{
    public string SurName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public bool IsDisabled { get; set; }

    public DateTime? LastLoggedInAtUtc { get; set; }
    public DateTime CreationUTC { get; set; }
    
    public RefreshToken RefreshToken { get; set; }

    public string Xvalue { get; set; }
    public string XtxtGuid { get; set; }
    public DateTime XvalueDate { get; set; }

    public bool DisableEmailNotification { get; set; }
}
