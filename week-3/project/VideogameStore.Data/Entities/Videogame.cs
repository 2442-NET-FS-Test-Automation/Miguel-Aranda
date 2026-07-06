using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;

public class Videogame
{
    public int VideogameId {get; set;}
    public string VideogameName {get; set;} = default!;
    [Required, MaxLength(50)]
    public string Genre {get; set;} = default!;
    [MaxLength(20)]
    public string Clasification {get; set;} = default!;
    public int Stock {get; set;}
    public byte[] RowVersion {get; set;} = default!; 
}