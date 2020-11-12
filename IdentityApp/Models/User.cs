using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace IdentityApp.Models
{
    public class User : IdentityUser
    {
        
        public Int64 Pin { get; set; }
        
        public byte RegionId { get; set; }
        public Region Region { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
