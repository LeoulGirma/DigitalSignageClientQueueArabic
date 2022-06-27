using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class CurrencyViewRepository : Repository<CurrencyView>, ICurrencyViewRepository
    {
        public CurrencyViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public CurrencyView Get(long id)
        {
            return _context.CurrencyViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<CurrencyView> GetAll(Func<CurrencyView, bool> func)
        {
            return _context.CurrencyViews.Where(func);
        }
    }
}
