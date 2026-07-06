using System.ComponentModel.DataAnnotations;
namespace VideogameStore.Data.Entities;

public class Videogame
{
    public int VideogameId {get; set;}
    public string Gamename {get; set;} = default!;
    public string Genre {get; set;} = default!;
    [MaxLength(20)]
    public Rating Rating {get; set;}
    public int Stock {get; set;}
    public byte[] RowVersion {get; set;} = default!; 
}