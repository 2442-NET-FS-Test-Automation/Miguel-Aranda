namespace VideogameStore.Data.Entities;
public class Store{
    public int StoreId {get; set;}
    public string StoreName {get; set;} = default!;
    public string Address {get; set;} = default!;
    // public int EmployeeId {get; set;} // FK
    // public Employee Employee {get; set;} = default!;
    public ICollection<Employee> Employees {get;} = new List<Employee>(); // One to many relationship 1:N
}