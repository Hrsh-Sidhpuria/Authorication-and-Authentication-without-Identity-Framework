using System.Security.Claims;

namespace Test3.Account.ClaimManager
{
    public interface IClaimAction
    {
        public List<Claim> GetRoleClaims(string roleId);
        public bool addRoleClaim(string RoleId, string Role);
        public bool addUserClaim(string UserId, string Email);
        public string getUserClaim(string UserId);
    }
}
