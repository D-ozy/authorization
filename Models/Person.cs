namespace authorization.Models
{
    public class Person
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public Person(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
