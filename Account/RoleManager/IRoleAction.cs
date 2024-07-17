namespace Test3.Account.RoleManager
{
    public interface IRoleAction
    {
        public string GetRole(string username);
        public string GetRoleId(string RoleName);
        public void DeleteRole();
        public void UpdateRole();

        public bool deleteRoleById(string UserId);
    }
}
