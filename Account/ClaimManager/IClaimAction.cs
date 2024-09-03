using System.Security.Claims;

namespace Authorization_Authentication.Account.ClaimManager
{
    public interface IClaimAction
    {
        public Task<ClaimsPrincipal> setClaim(string Username,String Role);
        public List<Claim> GetRoleClaims(string roleId);
        public bool addRoleClaim(string RoleId, string Role);
        public bool addUserClaim(string UserId, string Email);
        public string getUserClaim(string UserId);
    }
}
