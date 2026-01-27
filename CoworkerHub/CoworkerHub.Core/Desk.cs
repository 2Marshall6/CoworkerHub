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
        public bool IsAvailable { get; set; }

        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
    }
}
