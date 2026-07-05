using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;

public class Videogame
{
    public int VideogameId {get; set;}
    [Required, MaxLength(50)]
    public string Genre {get; set;}
    [MaxLength(20)]
    public string Clasification {get; set;}
    public int Stock {get; set;}
}