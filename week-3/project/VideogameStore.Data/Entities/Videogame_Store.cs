namespace VideogameStore.Data.Entities;
public class Videogame_Store
{
    public int Videogame_StoreId {get; set;}
    public int VideogameId {get; set;} // FK
    public Videogame Videogame {get; set;} = default!;
    public int StoreId {get; set;} // FK
    public Store Store {get; set;} = default!;
    public int Stock {get; set;}
    public byte[] RowVersion {get; set;} = default!; 
}