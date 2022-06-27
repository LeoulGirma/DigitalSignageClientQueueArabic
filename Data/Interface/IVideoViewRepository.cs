using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface IVideoViewRepository : IRepository<VideoView>
    {
        IEnumerable<VideoView> GetAll(Func<VideoView, bool> func);
        VideoView Get(long id);
    }
}
