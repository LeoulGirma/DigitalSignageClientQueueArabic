using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class RSSNewsViewRepository : Repository<RSSNewsView>, IRSSNewsViewRepository
    {
        public RSSNewsViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public RSSNewsView Get(long id)
        {
            return _context.RSSNewsViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<RSSNewsView> GetAll(Func<RSSNewsView, bool> func)
        {
            return _context.RSSNewsViews.Where(func);
        }
    }
}
