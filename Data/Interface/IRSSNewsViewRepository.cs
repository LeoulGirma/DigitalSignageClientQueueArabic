using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IRSSNewsViewRepository : IRepository<RSSNewsView>
    {
        IEnumerable<RSSNewsView> GetAll(Func<RSSNewsView, bool> func);
        RSSNewsView Get(long id);
    }
}
