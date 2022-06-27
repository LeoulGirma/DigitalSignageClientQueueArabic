using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface INavStyleViewRepository : IRepository<NavStyleView>
    {
        IEnumerable<NavStyleView> GetAll(Func<NavStyleView, bool> func);
        NavStyleView Get(long id);
    }
}
