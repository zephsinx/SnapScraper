namespace SnapScraper;

public static class CardUtils
{
    public static bool CardsAreEqual(DbCard existingCard, DbCard newCard)
    {
        return existingCard.CardId == newCard.CardId
               && existingCard.VariantId == newCard.VariantId
               && existingCard.Name == newCard.Name
               && existingCard.Type == newCard.Type
               && existingCard.Cost == newCard.Cost
               && existingCard.Power == newCard.Power
               && existingCard.Ability == newCard.Ability
               && existingCard.Flavor == newCard.Flavor
               && existingCard.ArtUrl == newCard.ArtUrl
               && existingCard.AlternateArt == newCard.AlternateArt
               && existingCard.Url == newCard.Url
               && existingCard.Status == newCard.Status
               && existingCard.Source == newCard.Source
               && existingCard.Rarity == newCard.Rarity
               && existingCard.Difficulty == newCard.Difficulty
               && existingCard.CardSlug == newCard.CardSlug;
    }
}