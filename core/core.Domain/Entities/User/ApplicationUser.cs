using Microsoft.AspNetCore.Identity;
using System;

namespace core.Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int ClusterId { get; set; }
        public string Name { get; set; }
        public string Response { get; set; }
        public bool Submitted { get; set; }
    }
}
