namespace VideogameStore.Data.Entities;

public class Employee
{
    public int EmployeeId {get; set;}
    public string Name {get; set;} = default!;
    public string SurName {get; set;} = default!;
    public string Email {get; set;} = default!;
    public string Address {get; set;} = default!;
    public int StoreId {get; set;}
    public Store store {get; set;} = default!;
}