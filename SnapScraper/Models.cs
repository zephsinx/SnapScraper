using System.Text.Json.Serialization;

namespace SnapScraper;

[Serializable]
public class SnapResponse
{
    [JsonPropertyName("success")] public SuccessResponse Success { get; set; } = new();

    public class SuccessResponse
    {
        [JsonPropertyName("cards")] public List<Card> Cards { get; set; } = new();
    }
}

[Serializable]
public class Card
{
    [JsonPropertyName("cid")] public int CardId { get; set; }
    [JsonPropertyName("vid")] public int VariantId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("cost")] public int Cost { get; set; }
    [JsonPropertyName("power")] public int Power { get; set; }
    [JsonPropertyName("ability")] public string Ability { get; set; } = string.Empty;
    [JsonPropertyName("flavor")] public string Flavor { get; set; } = string.Empty;
    [JsonPropertyName("art")] public string Art { get; set; } = string.Empty;
    [JsonPropertyName("alternate_art")] public string AlternateArt { get; set; } = string.Empty;
    [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("variants")] public List<Card> Variants { get; set; } = new();
    [JsonPropertyName("source")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("rarity")] public string Rarity { get; set; } = string.Empty;
    [JsonPropertyName("difficulty")] public string Difficulty { get; set; } = string.Empty;
    [JsonPropertyName("card_slug")] public string CardSlug => $"{this.Name.ToLowerInvariant().Replace(" ", "")}-{this.VariantId}";

    public DbCard ToDbCard()
    {
        return new DbCard
        {
            CardId = this.CardId,
            VariantId = this.VariantId,
            Name = this.Name,
            Type = this.Type,
            Cost = this.Cost,
            Power = this.Power,
            Ability = this.Ability.Replace(oldValue: "<span>", newValue: "").Replace(oldValue: "</span>", newValue: ""),
            Flavor = this.Flavor,
            ArtUrl = this.Art.Split("?").FirstOrDefault() ?? string.Empty,
            AlternateArt = this.AlternateArt,
            Url = this.Url.Split("?").FirstOrDefault() ?? string.Empty,
            Status = this.Status,
            Source = this.Source,
            Rarity = this.Rarity,
            Difficulty = this.Difficulty,
            CardSlug = this.CardSlug,
        };
    }
}

[Serializable]
public class CardFileOutput
{
    [JsonPropertyName("cardId")] public int CardId { get; set; }
    [JsonPropertyName("cardName")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("cost")] public int Cost { get; set; }
    [JsonPropertyName("power")] public int Power { get; set; }
    [JsonPropertyName("ability")] public string Ability { get; set; } = string.Empty;
    [JsonPropertyName("flavor")] public string Flavor { get; set; } = string.Empty;
    [JsonPropertyName("artUrl")] public string ArtUrl { get; set; } = string.Empty;
    [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
    [JsonPropertyName("cardStatus")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("cardSource")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("rarity")] public string Rarity { get; set; } = string.Empty;
    [JsonPropertyName("difficulty")] public string Difficulty { get; set; } = string.Empty;
    [JsonPropertyName("cardSlug")] public string CardSlug { get; set; } = string.Empty;
}