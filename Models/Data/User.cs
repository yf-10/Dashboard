namespace Dashboard.Models.Data;

public class User(string id, string name, string email) {
    public string Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
}
