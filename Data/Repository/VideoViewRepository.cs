using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class VideoViewRepository : Repository<VideoView>, IVideoViewRepository
    {
        public VideoViewRepository(ClientDSDbContext context) : base(context)
        {

        }

        public VideoView Get(long id)
        {
            return _context.VideoViews.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<VideoView> GetAll(Func<VideoView, bool> func)
        {
            return _context.VideoViews.Where(func);
        }

    }
}
