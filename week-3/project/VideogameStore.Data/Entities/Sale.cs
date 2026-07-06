namespace VideogameStore.Data.Entities;
public class Sale
{
    public int SaleId {get; set;}
    public int StoreId {get; set;} // FK
    public Store Store {get; set;} = default!;
    public int PaymentMethodId {get; set;} // FK
    public PaymentMethod PaymentMethod {get; set;} = default!;
    public int EmployeeId {get; set;} // FK
    public Employee Employee {get; set;} = default!;
    public int CustomerId {get; set;} // FK
    public Customer Customer {get; set;} = default!;
    public SaleFormat Format {get; set;} // enums: every game could be either in digital format or physical
    public DateTime SaleDate {get; set;} = DateTime.UtcNow;
}