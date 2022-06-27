using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface ICurrencyViewRepository : IRepository<CurrencyView>
    {
        IEnumerable<CurrencyView> GetAll(Func<CurrencyView, bool> func);
        CurrencyView Get(long id);
    }
}
