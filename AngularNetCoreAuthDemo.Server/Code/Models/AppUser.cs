using Microsoft.AspNetCore.Identity;

namespace AngularNetCoreAuthDemo.Server.Code.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public int SubscriptionType { get; set; }
    }
}
