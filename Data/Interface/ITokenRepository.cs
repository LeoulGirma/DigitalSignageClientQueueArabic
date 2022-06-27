using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Interface
{
    public interface ITokenRepository : IRepository<Token>
    {
        IEnumerable<Token> GetAll(Func<Token, bool> func);
        Token Get(long id);
    }
}
