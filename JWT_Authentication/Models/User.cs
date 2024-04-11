namespace JWT_Authentication.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid UserId => Guid.NewGuid();
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
