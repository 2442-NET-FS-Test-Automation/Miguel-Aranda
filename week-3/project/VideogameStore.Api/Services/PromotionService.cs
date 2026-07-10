using Microsoft.EntityFrameworkCore;
using VideogameStore.Data;
using VideogameStore.Data.Entities;
using Serilog;
using System.Runtime.CompilerServices;
namespace VideogameStore.Api.Services;

public interface IPromotionService
{
    public Task<PromotionValidation> ValidateAndApplyAsync(string PromoCode, int CustomerId, decimal currentSubtotal, CancellationToken ct);
}

public record PromotionValidation(bool IsValid, string Message, int? PromotionId, decimal Percentage);

public class PromotionService : IPromotionService
{
    private readonly IDbContextFactory<VideogameStoreDbContext> _factory;
    
    public PromotionService(IDbContextFactory<VideogameStoreDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<PromotionValidation> ValidateAndApplyAsync(string PromoCode, int CustomerId, decimal currentSubtotal, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(PromoCode))
        {
            return new PromotionValidation(false, "There wasn't any promotion applied", null, 0);
        }

        await using var db = await _factory.CreateDbContextAsync(ct);

        // 1. Check if the promotion exists or if it's valid
        var promo = await db.Promotions
        .FirstOrDefaultAsync(p => p.PromoCode == PromoCode && p.IsValid && p.ExpirationDate > DateTime.UtcNow);

        if (promo == null)
        {
            Log.Warning("the Coupon {Code} is not applicable or it's invalid", PromoCode);
            return new PromotionValidation(false, "The Coupon is invalid or doesn't apply", null, 0);
        }

        // 2. check if the client has used the promotion before
        var AlreadyUsed = await db.C_Promotions
        .AnyAsync(cp => cp.CustomerId == CustomerId && cp.PromotionId == promo.PromotionId, ct);

        if (AlreadyUsed)
        {
            Log.Warning("The Coupon {Code} has already been used", PromoCode);
            return new PromotionValidation(false, "The Coupon has been used", null, 0);
        }

        // 3. as nothing goes wrong, we calculate the discount
        decimal discountAmount = currentSubtotal * (promo.Percentage / 100);

        Log.Information("Coupon {Code} applied successfully to the Customer {CustomerId}. Discount {discount}", PromoCode, CustomerId, discountAmount);

        return new PromotionValidation(
            IsValid: true, 
            Message: "The Coupon has been applied", 
            PromotionId: promo.PromotionId,
            Percentage: discountAmount);
    }
}