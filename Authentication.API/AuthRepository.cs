using System;
using System.Threading.Tasks;
using Lily.Authentication.API.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Lily.Authentication.API
{
    public class AuthRepository : IDisposable
    {
        private readonly AuthContext _ctx;

        private readonly UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            return await _userManager.FindAsync(userName, password);
        }

        public Client FindClient(string clientId)
        {
            return _ctx.Clients.Find(clientId);
        }

        //public async Task<bool> AddRefreshToken(RefreshToken token)
        //{

        //   var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

        //   if (existingToken != null)
        //   {
        //     var result = await RemoveRefreshToken(existingToken);
        //   }
          
        //    _ctx.RefreshTokens.Add(token);

        //    return await _ctx.SaveChangesAsync() > 0;
        //}

        //public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        //{
        //   var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

        //   if (refreshToken != null) {
        //       _ctx.RefreshTokens.Remove(refreshToken);
        //       return await _ctx.SaveChangesAsync() > 0;
        //   }

        //   return false;
        //}

        //public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        //{
        //    _ctx.RefreshTokens.Remove(refreshToken);
        //     return await _ctx.SaveChangesAsync() > 0;
        //}

        //public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        //{
        //    var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

        //    return refreshToken;
        //}

        //public List<RefreshToken> GetAllRefreshTokens()
        //{
        //     return  _ctx.RefreshTokens.ToList();
        //}

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            return await _userManager.FindAsync(loginInfo);
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            return await _userManager.CreateAsync(user);
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            return await _userManager.AddLoginAsync(userId, login);
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}