namespace Users.API.Entities;

[Serializable]
public class User
{
    public User(string id, string name, string licensePlate)
    {
        Id = id;
        Name = name;
        LicensePlate = licensePlate;
    }

    public string Id { get;  }
    public string Name { get;  }
    public string LicensePlate { get; }
}