namespace Dashboard.Models.Item;

public class User(string name, string fullName, string email)
{
    public string Name { get; private set; } = name;
    public string FullName { get; private set; } = fullName;
    public string Email { get; private set; } = email;
}
