using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Toucan.Adapters;

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
