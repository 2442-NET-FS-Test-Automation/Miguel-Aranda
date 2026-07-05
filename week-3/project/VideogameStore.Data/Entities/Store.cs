namespace VideogameStore.Data.Entities;
public class Store{
    public int StoreId {get; set;}
    public string StoreName {get; set;}
    public string Address {get; set;}
    public int EmployeeId {get; set;} // FK
    public Employee Employee {get; set;}
}