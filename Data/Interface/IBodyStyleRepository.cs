using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IBodyStyleViewRepository : IRepository<BodyStyleView>
    {
        IEnumerable<BodyStyleView> GetAll(Func<BodyStyleView, bool> func);
        BodyStyleView Get(long id);
    }
}
