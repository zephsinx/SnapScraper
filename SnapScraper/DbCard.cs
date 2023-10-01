using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SnapScraper;

[Table("Card")]
public class DbCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public int Id { get; set; }

    [JsonPropertyName("cardId")]
    [Required]
    public int CardId { get; set; }

    [JsonPropertyName("variantId")] public int VariantId { get; set; }

    [JsonPropertyName("cardName")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("cardType")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("cost")] [Required] public int Cost { get; set; }
    [JsonPropertyName("power")] [Required] public int Power { get; set; }
    [JsonPropertyName("ability")] public string Ability { get; set; } = string.Empty;
    [JsonPropertyName("flavor")] public string Flavor { get; set; } = string.Empty;
    [JsonPropertyName("artUrl")] public string ArtUrl { get; set; } = string.Empty;
    [JsonPropertyName("alternateArt")] public string AlternateArt { get; set; } = string.Empty;
    [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
    [JsonPropertyName("cardStatus")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("cardSource")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("rarity")] public string Rarity { get; set; } = string.Empty;
    [JsonPropertyName("difficulty")] public string Difficulty { get; set; } = string.Empty;
    [JsonPropertyName("cardSlug")] public string CardSlug { get; set; } = string.Empty;
    [JsonPropertyName("imageLocalPath")] public string ImageLocalPath { get; set; } = string.Empty;
    [JsonPropertyName("blurredImageLocalPath")] public string BlurredImageLocalPath { get; set; } = string.Empty;
}