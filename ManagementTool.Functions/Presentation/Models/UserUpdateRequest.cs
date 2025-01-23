using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementTool.Functions.Presentation.Models
{
    public class UserUpdateRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Credits { get; set; }
    }
}
