using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;
public class Customer
{
    public int CustomerId {get; set;}
    public string Name {get; set;} = default!;
    public string SurName {get; set;} = default!;
    [Required]
    public string Email {get; set;} = default!;
    public string Address {get; set;} = default!;
    public string City {get; set;} = default!;
}