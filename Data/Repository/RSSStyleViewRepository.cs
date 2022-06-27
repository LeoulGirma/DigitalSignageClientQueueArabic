using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class RSSStyleViewRepository : Repository<RSSStyleView>, IRSSStyleViewRepository
    {
        public RSSStyleViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public RSSStyleView Get(long id)
        {
            return _context.RSSStyleViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<RSSStyleView> GetAll(Func<RSSStyleView, bool> func)
        {
            return _context.RSSStyleViews.Where(func);
        }
    }
}
