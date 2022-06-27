using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class NavStyleViewRepository : Repository<NavStyleView>, INavStyleViewRepository
    {
        public NavStyleViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public NavStyleView Get(long id)
        {
            return _context.NavStyleViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<NavStyleView> GetAll(Func<NavStyleView, bool> func)
        {
            return _context.NavStyleViews.Where(func);
        }

    }
}
