

using Microsoft.EntityFrameworkCore;
using Music.Model;

using var db = new MusicContext();


while (true)
{

    Console.WriteLine("=== Music App ===");
    Console.WriteLine("1. Lista playlists");
    Console.WriteLine("2. Skapa playlist");
    Console.WriteLine("3. Ändra playlist");
    Console.WriteLine("4. Ta bort playlist");
    Console.WriteLine("0. Avsluta");

    string choice = Console.ReadLine();
    if (!int.TryParse(choice, out int answer))
    {
        Console.WriteLine("Skriv ett giltigt nummer!");
        continue; 
    }

    switch (answer)
    {
        case 1:

            var playlists = db.Playlists.ToList();

            foreach (var playlist in playlists)
            {
                Console.WriteLine($"\nPlaylist: {playlist.Name}");

                var tracks = db.PlaylistTracks
                    .Where(pt => pt.PlaylistId == playlist.PlaylistId)
                    .Select(pt => pt.Track.Name)
                    .ToList();

                foreach (var trackName in tracks)
                {
                    Console.WriteLine($"  - {trackName}");
                }
                
                Console.WriteLine("\nTryck på valfri tangent för att se nästa spellista");
                Console.ReadKey();
            }

            break;

        case 2:
            
            break;

        case 3:
            break;

        case 4:
            break;

        case 0:
            return;
    }
}
