using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;
public class Customer
{
    public int CustomerId {get; set;}
    public string Name {get; set;}
    public string SurName {get; set;}
    [Required]
    public string Email {get; set;}
    public string Address {get; set;}
    public string City {get; set;}
}