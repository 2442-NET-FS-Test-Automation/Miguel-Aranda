namespace VideogameStore.Data.Entities;
public class Store{
    public int StoreId {get; set;}
    public string StoreName {get; set;} = default!;
    public string Address {get; set;} = default!;
    public int EmployeeId {get; set;} // FK
    public Employee Employee {get; set;} = default!;
}