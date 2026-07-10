using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;
public class Promotion
{
    public int PromotionId {get; set;}
    public string PromoCode {get; set;} = default!;
    public decimal Percentage {get; set;}
    public bool IsValid {get; set;}
    public DateTime ExpirationDate {get; set;}
}