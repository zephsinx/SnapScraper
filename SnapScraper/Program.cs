using System.Text.Json;
using SnapScraper;

const bool verboseLogs = true;
bool cleanupCards = false;

// Call Marvel Snap site URL to get JSON data
const string url = "https://marvelsnapzone.com/getinfo/?searchtype=cards&searchcardstype=true";
const string projectRoot = "";

string dbPath = Path.Combine(projectRoot, "snap-cards.db");

if (File.Exists(dbPath))
{
    // Console.Write("Do you want to check for and delete any cards that no longer exist? [y/N] (default: N): ");
    Console.WriteLine("Do you want to check for and delete any cards that no longer exist? [y/N] (default: N): ");
    // string cleanupCardsInput = Console.ReadLine() ?? string.Empty;
    string cleanupCardsInput = "n";
    cleanupCards = cleanupCardsInput.Equals("y", StringComparison.InvariantCultureIgnoreCase);
}

await using SnapScraperDbContext dbContext = new(dbPath);
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("Checking marvelsnapzone.com for new cards...");
HttpClient client = new();
string json = await client.GetStringAsync(url);

SnapResponse response = JsonSerializer.Deserialize<SnapResponse>(json) ?? new SnapResponse();
List<Card> cards = response.Success.Cards;
int variantCount = cards.Sum(card => card.Variants.Count);

Console.WriteLine($"Total cards found: {cards.Count} cards. {variantCount} variants.");

HashSet<string> websiteCardSet = new();
bool shouldSaveContext = false;

foreach (Card card in cards)
{
    DbCard dbCard = card.ToDbCard();
    websiteCardSet.Add($"{dbCard.CardSlug}");
    bool cardUpserted = CardService.UpsertCard(dbContext, dbCard, out string action, saveContext: false);

    if (cardUpserted)
    {
        shouldSaveContext = true;
        Console.WriteLine($"{action} card {card.Name}");
    }

    foreach (Card variant in card.Variants)
    {
        variant.Name = card.Name;
        DbCard variantDbCard = variant.ToDbCard();

        websiteCardSet.Add($"{variantDbCard.CardSlug}");

        variantDbCard.Type = dbCard.Type;
        variantDbCard.Cost = dbCard.Cost;
        variantDbCard.Power = dbCard.Power;
        variantDbCard.Ability = dbCard.Ability;
        variantDbCard.Flavor = dbCard.Flavor;
        variantDbCard.Source = dbCard.Source;
        bool variantUpserted = CardService.UpsertCard(dbContext, variantDbCard, out action, saveContext: false);

        if (variantUpserted)
        {
            shouldSaveContext = true;
            Console.WriteLine($"{action} variant {variantDbCard.CardSlug}");
        }
    }
}

if (shouldSaveContext)
{
    dbContext.SaveChanges();
    shouldSaveContext = false;
}

// Clean up DB cards if cleanupCards == true
if (cleanupCards)
{
    List<DbCard> cardsForCleanup = CardService.GetCards(dbContext).ToList();
    Console.WriteLine("Checking for no-longer-existing cards to remove");
    foreach (DbCard card in cardsForCleanup)
    {
        try
        {
            if (!websiteCardSet.Contains($"{card.CardSlug}"))
            {
                Console.WriteLine($"Removing card '{card.Name}'");
                if (File.Exists(card.ImageLocalPath))
                    File.Delete(card.ImageLocalPath);
                CardService.RemoveCard(dbContext, card);
            }
        }
        catch (Exception ex)
        {
            Console.Write($"Error removing card '{card.Name}. Error: ");
            Console.WriteLine(ex.ToString());
        }
    }
}

Console.WriteLine("Checking for images to download");
List<DbCard> cardsWithNoImage = CardService.GetCardsWithMissingImages(dbContext).OrderBy(c => c.VariantId).ThenBy(c => c.CardId).ToList();

// Create images folder
Directory.CreateDirectory("images");

// Download image files for cards with no image
foreach (DbCard card in cardsWithNoImage)
{
    // Get file name from end of URL, minus the query parameters
    string fileName = card.ArtUrl.Split('/').Last().Split('?').First();
    string blurredFileName = $"bl-{card.ArtUrl.Split('/').Last().Split('?').First()}";
    string filePath = Path.Combine(projectRoot, "images", card.CardId.ToString(), fileName);
    string blurredFilePath = Path.Combine(projectRoot, "images", card.CardId.ToString(), blurredFileName);
    string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
    Directory.CreateDirectory(directoryPath);

    bool haveCardImageFile;

    if (!File.Exists(filePath) || !File.Exists(blurredFilePath))
    {
        if (!File.Exists(filePath))
        {
            (haveCardImageFile, string error) = await CardService.DownloadAndSaveImageAsync(card.ArtUrl, filePath);
            if (haveCardImageFile)
                Console.WriteLine($"Downloaded image with filename {fileName} to {directoryPath}");
            else if (verboseLogs)
                Console.WriteLine($"Error downloading image with filename {fileName}. Error: {error}");
        }

        haveCardImageFile = true;
        (bool success, string placeholderError) = await CardService.GenerateBlurredImageAsync(filePath, blurredFilePath);
        if (!success)
            Console.WriteLine($"Error generating placeholder image with filename {blurredFileName}. Error: {placeholderError}");
    }
    else
    {
        haveCardImageFile = true;
    }

    if (haveCardImageFile)
    {
        card.ImageLocalPath = Path.Combine("images", card.CardId.ToString(), fileName);
        card.BlurredImageLocalPath = Path.Combine("images", card.CardId.ToString(), blurredFileName);
        CardService.UpdateCard(dbContext, card, saveContext: false);
        shouldSaveContext = true;
    }
}

if (shouldSaveContext)
    dbContext.SaveChanges();

IEnumerable<DbCard> allCards = CardService.GetCards(dbContext);
List<CardFileOutput> jsonCards = allCards.Where(c => c.VariantId == 0).OrderBy(c => c.CardSlug).Select(card => new CardFileOutput
{
    Ability = card.Ability,
    Cost = card.Cost,
    Power = card.Power,
    Difficulty = card.Difficulty,
    CardSlug = card.CardSlug,
    Flavor = card.Flavor,
    ArtUrl = card.ArtUrl,
    Name = card.Name,
    Source = card.Source,
    Rarity = card.Rarity,
    Status = card.Status,
    CardId = card.CardId,
    Url = card.Url,
}).ToList();

string cardFilePath = Path.Combine(projectRoot, "snap-cards.json");
Console.WriteLine($"Writing cards to {cardFilePath}");
string fileContents = JsonSerializer.Serialize(jsonCards);
File.WriteAllText(cardFilePath, fileContents);

Console.WriteLine("Done!");