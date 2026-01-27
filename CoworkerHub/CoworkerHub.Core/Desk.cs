using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoworkerHub.Core
{
    public class Desk
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IsAvailable { get; set; }
        public bool WorkspaceId { get; set; }
    }
}
