using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Models
{
    public class TokenResponse
    {
        public TokenResponse()
        {
            tokens = new List<Ticket>();
        }
        public List<Ticket> tokens { get;  }
    }

    public class ServiceForAndroid
    {
        public List<ServiceClient> service;
        public ServiceForAndroid()
        {
            service = new List<ServiceClient>();
        }
    }

    public class GroupForAndroid
    {
        public List<GroupClient> group;
        public GroupForAndroid()
        {
            group = new List<GroupClient>();
        }
    }

    public class Ticket
    {
        public string token;
    }

    public class ServiceClient
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public long GroupId { get; set; }
    }

    public class GroupClient
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
