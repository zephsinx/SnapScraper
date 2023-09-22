using Microsoft.EntityFrameworkCore;

namespace SnapScraper;

public static class CardService
{
    public static bool CreateCard(SnapScraperDbContext dbContext, DbCard card)
    {
        DbCard? existingCard = dbContext.Cards.FirstOrDefault(c => c.CardId == card.CardId && c.VariantId == card.VariantId);
        if (existingCard is not null)
            return false;

        dbContext.Cards.Add(card);
        dbContext.SaveChanges();
        return true;
    }

    public static bool UpsertCard(SnapScraperDbContext dbContext, DbCard card, out string action)
    {
        DbCard? existingCard = dbContext.Cards.FirstOrDefault(c => c.CardId == card.CardId && c.VariantId == card.VariantId);

        action = string.Empty;

        if (existingCard is null)
        {
            action = "Created";
            dbContext.Cards.Add(card);
        }
        else if (!CardUtils.CardsAreEqual(existingCard, newCard: card))
        {
            action = "Updated";
            existingCard.Name = card.Name;
            existingCard.Type = card.Type;
            existingCard.Cost = card.Cost;
            existingCard.Power = card.Power;
            existingCard.Ability = card.Ability;
            existingCard.Flavor = card.Flavor;
            existingCard.ArtUrl = card.ArtUrl;
            existingCard.AlternateArt = card.AlternateArt;
            existingCard.Url = card.Url;
            existingCard.Status = card.Status;
            existingCard.Source = card.Source;
            existingCard.Rarity = card.Rarity;
            existingCard.Difficulty = card.Difficulty;
        }
        else
        {
            return false;
        }

        dbContext.SaveChanges();
        return true;
    }

    public static DbCard? GetCard(SnapScraperDbContext dbContext, int cardId, int variantId = 0)
    {
        return dbContext.Cards.AsNoTracking().FirstOrDefault(c => c.CardId == cardId && c.VariantId == variantId);
    }

    public static IEnumerable<DbCard> GetCards(SnapScraperDbContext dbContext, bool tracking = false)
    {
        return tracking ? dbContext.Cards : dbContext.Cards.AsNoTracking();
    }

    public static IEnumerable<DbCard> GetCards(SnapScraperDbContext dbContext, string name)
    {
        return dbContext.Cards.AsNoTracking().Where(c => c.Name == name);
    }

    public static IEnumerable<DbCard> GetCardVariants(SnapScraperDbContext dbContext, int cardId)
    {
        return dbContext.Cards.AsNoTracking().Where(c => c.CardId == cardId && c.VariantId > 0);
    }

    public static IEnumerable<DbCard> GetCardVariants(SnapScraperDbContext dbContext, string name)
    {
        return dbContext.Cards.AsNoTracking().Where(c => c.Name == name && c.VariantId > 0);
    }

    public static bool UpdateCard(SnapScraperDbContext dbContext, DbCard card)
    {
        DbCard? existingCard = dbContext.Cards.FirstOrDefault(c => c.CardId == card.CardId && c.VariantId == card.VariantId);
        if (existingCard is null)
            return false;

        existingCard.Name = card.Name;
        existingCard.Type = card.Type;
        existingCard.Cost = card.Cost;
        existingCard.Power = card.Power;
        existingCard.Ability = card.Ability;
        existingCard.Flavor = card.Flavor;
        existingCard.ArtUrl = card.ArtUrl;
        existingCard.AlternateArt = card.AlternateArt;
        existingCard.Url = card.Url;
        existingCard.Status = card.Status;
        existingCard.Source = card.Source;
        existingCard.Rarity = card.Rarity;
        existingCard.Difficulty = card.Difficulty;
        existingCard.LocalImagePath = card.LocalImagePath;

        dbContext.SaveChanges();
        return true;
    }

    public static IEnumerable<DbCard> GetCardsWithMissingImages(SnapScraperDbContext dbContext)
    {
        return dbContext.Cards.AsNoTracking().Where(c => string.IsNullOrEmpty(c.LocalImagePath));
    }

    /// <summary>
    /// Downloads the image stream from the specified URL.
    /// </summary>
    /// <param name="attachmentUrl"></param>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<(bool, string)> DownloadAndSaveImageAsync(string attachmentUrl, string filePath)
    {
        try
        {
            await using Stream stream = await new HttpClient().GetStreamAsync(attachmentUrl);

            await using FileStream fileStream = File.Create(filePath);
            await stream.CopyToAsync(fileStream);
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Delete Card from DB
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="dbCard"></param>
    public static void RemoveCard(SnapScraperDbContext dbContext, DbCard dbCard)
    {
        dbContext.Remove(dbCard);
        dbContext.SaveChanges();
    }
}