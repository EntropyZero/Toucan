using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Toucan.Core.Data;

namespace Toucan.Sample.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser, IDbEntity
    {
        object IDbEntity.Id
        {
            get
            {
                return Id;
            }
        }
    }
}
