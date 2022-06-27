using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalSignageClient.Data.Repository
{
    public class TokenRepository : Repository<Token>, ITokenRepository
    {
        public TokenRepository(ClientDSDbContext context) : base(context) { }

        public Token Get(long id)
        {
            return _context.Tokens.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Token> GetAll(Func<Token, bool> func)
        {
            return _context.Tokens.Where(func);
        }
    }
}
