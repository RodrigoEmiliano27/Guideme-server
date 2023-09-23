namespace GuideMeServerMVC.TO
{
    public class LoginRequestTO
    {
        public LoginRequestTO()
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }
}
