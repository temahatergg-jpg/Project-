namespace WindowsFormsApp1
{
    public class UserItem
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public bool IsAdmin { get; set; }
    }
}