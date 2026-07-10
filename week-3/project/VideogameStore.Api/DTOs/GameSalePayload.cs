namespace VideogameStore.Api.DTOs;
public record GameSalePaylod(
    int CustomerId, 
    int StoreId,
    int EmployeeId,
    int PaymentMethodId,
    int Quantity, 
    decimal UnitPrice, 
    int VideogameId,
    string PromoCode,
    string CustomerEmail
    );