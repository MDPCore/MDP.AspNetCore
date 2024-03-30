using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    public interface AccessRecordRepository
    {
        // Methods
        void Add(AccessRecord accessRecord);

        AccessRecord FindByRoleId(string roleId, ResourceUri resourceUri);
    }
}
