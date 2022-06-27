using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class BodyStyleViewRepository : Repository<BodyStyleView>, IBodyStyleViewRepository
    {
        public BodyStyleViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public BodyStyleView Get(long id)
        {
            return _context.BodyStyleViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<BodyStyleView> GetAll(Func<BodyStyleView, bool> func)
        {
            return _context.BodyStyleViews.Where(func);
        }
    }
}
