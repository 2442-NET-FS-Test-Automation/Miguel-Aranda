namespace VideogameStore.Data.Entities;
public class Sale_Detail
{
    public int Sale_DetailId {get; set;}
    public int Quantity {get; set;}
    public decimal UnitPrice {get; set;}
    public int VideogameId {get; set;} // FK
    public Videogame Videogame {get; set;}
    public int SaleId {get; set;} // FK
    public Sale Sale {get; set;}

}