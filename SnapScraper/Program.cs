using System.Text.Json;
using SnapScraper;

const bool verboseLogs = true;
bool cleanupCards = false;

if (File.Exists("snap-cards.db"))
{
    Console.Write("Do you want to check for and delete any cards that no longer exist? [y/N] (default: N): ");
    string cleanupCardsInput = Console.ReadLine() ?? string.Empty;
    cleanupCards = cleanupCardsInput.Equals("y", StringComparison.InvariantCultureIgnoreCase);
}

// Call Marvel Snap site URL to get JSON data
const string url = "https://marvelsnapzone.com/getinfo/?searchtype=cards&searchcardstype=true";
string projectRoot = string.Empty;

await using SnapScraperDbContext dbContext = new(Path.Combine(projectRoot, "snap-cards.db"));
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("Checking marvelsnapzone.com for new cards...");
HttpClient client = new();
string json = await client.GetStringAsync(url);

SnapResponse response =  JsonSerializer.Deserialize<SnapResponse>(json) ?? new SnapResponse();
List<Card> cards = response.Success.Cards;
int variantCount = cards.Sum(card => card.Variants.Count);

Console.WriteLine($"Total cards found: {cards.Count} cards. {variantCount} variants.");

HashSet<string> websiteCardSet = new();

foreach (Card card in cards)
{
    websiteCardSet.Add($"{card.CardId}-{card.VariantId}");

    DbCard dbCard = card.ToDbCard();
    bool cardUpserted = CardService.UpsertCard(dbContext, dbCard, out string action);

    if (cardUpserted)
        Console.WriteLine($"{action} card {card.Name}");

    foreach (DbCard variantDbCard in card.Variants.Select(variantCard => variantCard.ToDbCard()))
    {
        websiteCardSet.Add($"{variantDbCard.CardId}-{variantDbCard.VariantId}");

        variantDbCard.Name = card.Name;
        variantDbCard.Type = card.Type;
        variantDbCard.Cost = card.Cost;
        variantDbCard.Power = card.Power;
        variantDbCard.Ability = card.Ability;
        variantDbCard.Flavor = card.Flavor;
        variantDbCard.Source = card.Source;
        bool variantUpserted = CardService.UpsertCard(dbContext, variantDbCard, out action);

        if (variantUpserted)
            Console.WriteLine($"{action} variant {variantDbCard.Name}-{variantDbCard.VariantId})");
    }
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
            if (!websiteCardSet.Contains($"{card.CardId}-{card.VariantId}"))
            {
                Console.WriteLine($"Removing card '{card.Name}'");
                if (File.Exists(card.LocalImagePath))
                    File.Delete(card.LocalImagePath);
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
IEnumerable<DbCard> cardsWithNoImage = CardService.GetCardsWithMissingImages(dbContext);

// Create images folder
Directory.CreateDirectory("images");

// Download image files for cards with no image
foreach (DbCard card in cardsWithNoImage)
{
    // Get file name from end of URL, minus the query parameters
    string fileName = card.ArtUrl.Split('/').Last().Split('?').First();
    string filePath = Path.Combine(projectRoot, "images", card.CardId.ToString(), fileName);
    string directoryPath = Path.GetDirectoryName(filePath) ?? string.Empty;
    Directory.CreateDirectory(directoryPath);

    bool haveCardImageFile;

    if (!File.Exists(filePath))
    {
        (haveCardImageFile, string error) = await CardService.DownloadAndSaveImageAsync(card.ArtUrl, filePath);

        if (haveCardImageFile)
            Console.WriteLine($"Downloaded image with filename {fileName} to {directoryPath}");
        else if (verboseLogs)
            Console.WriteLine($"Error downloading image with filename {fileName}. Error: {error}");
    }
    else
    {
        haveCardImageFile = true;
    }

    if (haveCardImageFile)
    {
        card.LocalImagePath = filePath;
        CardService.UpdateCard(dbContext, card);
    }
}

List<DbCard> allCards = CardService.GetCards(dbContext).ToList();
const string cardFileName = "snap-cards.json";
Console.WriteLine($"Writing cards to {cardFileName}");
string fileContents = JsonSerializer.Serialize(allCards);
File.WriteAllText(cardFileName, fileContents);

Console.WriteLine("Done!");