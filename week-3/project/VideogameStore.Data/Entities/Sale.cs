namespace VideogameStore.Data.Entities;
public class Sale
{
    public int SaleId {get; set;}
    public int StoreId {get; set;} // FK
    public Store Store {get; set;}
    public int PaymentMethodId {get; set;} // FK
    public PaymentMethod PaymentMethod {get; set;}
    public int EmployeeId {get; set;} // FK
    public Employee Employee {get; set;}
    public int CustomerId {get; set;} // FK
    public Customer Customer {get; set;}
    public SaleFormat Format {get; set;} // enums: every game could be either in digital format or physical
    public DateTime SaleDate {get; set;} = DateTime.UtcNow;
}