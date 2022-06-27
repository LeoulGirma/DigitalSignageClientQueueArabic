using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IRSSStyleViewRepository : IRepository<RSSStyleView>
    {
        IEnumerable<RSSStyleView> GetAll(Func<RSSStyleView, bool> func);
        RSSStyleView Get(long id);
    }
}
