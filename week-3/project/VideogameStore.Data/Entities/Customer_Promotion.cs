using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;
public class Customer_Promotion
{
    public int Customer_PromotionId {get; set;}
    public int CustomerId {get; set;}
    public Customer Customer {get; set;} = default!;
    public int PromotionId {get; set;}
    public Promotion Promotion {get; set;} = default!;
    public DateTime AlreadyUsed {get; set;} = DateTime.UtcNow;
}