using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnapScraper;

[Table("Card")]
public class DbCard
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Required]
    public int Id { get; set; }

    [Required]
    public int CardId { get; set; }
    public int VariantId { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    [Required] public int Cost { get; set; }
    [Required] public int Power { get; set; }
    public string Ability { get; set; } = string.Empty;
    public string Flavor { get; set; } = string.Empty;
    public string ArtUrl { get; set; } = string.Empty;
    public string AlternateArt { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string CardSlug { get; set; } = string.Empty;
    public string LocalImagePath { get; set; } = string.Empty;
}