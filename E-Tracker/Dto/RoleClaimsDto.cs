using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Tracker.Dto
{
    public class RoleClaimsDto
    {
        public RoleClaimsDto()
        {
            Claims = new List<RoleClaim>();
        }

        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsRoleActive { get; set; }
        public List<RoleClaim> Claims { get; set; }
    }

}
