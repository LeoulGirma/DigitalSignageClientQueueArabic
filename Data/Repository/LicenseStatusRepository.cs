using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class LicenseStatusRepository : Repository<LicenseStatus>, ILicenseStatusRepository
    {
        public LicenseStatusRepository(ClientDSDbContext context) : base(context)
        {

        }
    }
}
