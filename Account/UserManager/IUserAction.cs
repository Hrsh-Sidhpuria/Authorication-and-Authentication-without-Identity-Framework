namespace Authorization_Authentication.Account.UserManager
{
    public interface IUserAction
    {
        public bool createUser(string Username, string Password, string Role, string Email);

        public string getEmail(string Username);
        public string getIdByName(string Username);

        public bool LoginUser(string Username, string Password);
        public bool updateUser(string Id,string Username,string Email,string role);

        public bool UpdatePassword(string Username, string currentPass , string Newpass);

        public string getPassword(string Username);
        public bool deleteUser(string username, string password);

        public int accessFailedCount(string Username, int totalfailedCount);

        public bool resetFailCount(string Username);

        public bool setLockAccountdate(string Username);

        public bool isLockAccount(string Username);

        public bool getLockAccountdate(string Username);

    }
}
