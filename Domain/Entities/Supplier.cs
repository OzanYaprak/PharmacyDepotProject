namespace Domain.Entities;

public class Supplier : BaseEntity<Guid>
{
    public Supplier() { }
    public Supplier(Guid id, string name, string contactPerson, string phone, string email, string address)
    {
        Id = id;
        Name = name;
        ContactPerson = contactPerson;
        Phone = phone;
        Email = email;
        Address = address;
    }

    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
}
