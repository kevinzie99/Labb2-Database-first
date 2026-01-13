

using Microsoft.EntityFrameworkCore;
using Music.Model;
using System.Runtime.CompilerServices;

using var db = new MusicContext();


while (true)
{

    Console.WriteLine("\n=== Music App ===");
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

            string playlistName;

            while (true)
            {
                Console.WriteLine("\nAnge namn på ny spellista:");
                playlistName = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(playlistName))
                {
                    break;
                }

                Console.WriteLine("Namnet får inte vara tomt. Försök igen.");

            }


            int nextId = db.Playlists.Any() ? db.Playlists.Max(p => p.PlaylistId) + 1 : 1;

            var newPlaylist = new Playlist
            {
                PlaylistId = nextId,
                Name = playlistName
            };

            db.Playlists.Add(newPlaylist);
            db.SaveChanges();


            Console.WriteLine($"\nSpellistan {playlistName} är nu skapad! Nu kan du lägga till låtar.\n");

            var allTracks = db.Tracks.ToList();
            foreach (var track in allTracks)
            {
                Console.WriteLine($"ID: {track.TrackId} - {track.Name}");
            }

            Console.WriteLine("\nAlla låtar visas ovanför. Det är valfritt att söka på en låt.");


            while (true)
            {
                Console.WriteLine("\nSök på låt (eller tryck Enter för att avsluta):");
                string search = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(search))
                {
                    break;
                }

                var foundTracks = db.Tracks.Where(t => t.Name.Contains(search)).ToList();

                if (foundTracks.Count == 0)
                {
                    Console.WriteLine("Inga låtar hittades med den sökningen.");
                    continue;
                }

                Console.WriteLine("\nMatchande låtar:\n");
                foreach (var track in foundTracks)
                {
                    Console.WriteLine($"ID: {track.TrackId} - {track.Name}");
                }

                Console.WriteLine("\nSkriv in Track ID på låten du vill lägga till (eller flera ID separerad med komma)");
                string input = Console.ReadLine();

                var selectedTracks = input.Split(',').Select(s => int.TryParse(s.Trim(), out int id) ? id : -1).Where(id => foundTracks.Any(t => t.TrackId == id)).ToList();

                foreach (var selectedTrack in selectedTracks)
                {
                    var pt = new PlaylistTrack
                    {
                        PlaylistId = newPlaylist.PlaylistId,
                        TrackId = selectedTrack
                    };

                    db.PlaylistTracks.Add(pt);
                }

                db.SaveChanges();
                Console.WriteLine("\nLåtarna tillagda! Du kan fortsätta söka på låtar eller trycka Enter för avsluta");
            }

            Console.WriteLine("Spellistan är klar!");
            Console.WriteLine("Tryck på valfri tagent för att återgå till menyn...");
            Console.ReadKey();
            break;



        case 3:

            var playlistsToEdit = db.Playlists.ToList();

            if (!playlistsToEdit.Any())
            {

                Console.WriteLine("Inga spellistor finns att ändra");
                break;
            }

            Console.WriteLine("\nSpellistor:");
            foreach (var p in playlistsToEdit)
            {
                Console.WriteLine($"ID: {p.PlaylistId} - Name: {p.Name}");
            }

            Console.WriteLine("\nAnge ID på spellistan du vill ändra:");
            if (!int.TryParse(Console.ReadLine(), out int editId))
            {
                Console.WriteLine("Ogiltigt ID.");
                break;
            }

            var playlistToEdit = db.Playlists.FirstOrDefault(p => p.PlaylistId == editId);
            if (playlistToEdit == null)
            {
                Console.WriteLine("Spellistan hittades inte.");
                break;
            }

            Console.WriteLine($"\nNuvarande namn: {playlistToEdit.Name}");
            Console.WriteLine("ange nytt namn på spellistan (lämna tomt för att behålla)");
            string newName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                playlistToEdit.Name = newName;
                db.SaveChanges();
                Console.WriteLine("Spellistans namn uppdaterad!");
            }

            break;
        
        case 4:
        break;

        case 0:
         return;
                

    }
}