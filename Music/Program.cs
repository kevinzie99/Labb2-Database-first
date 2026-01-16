

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

            var allTracks  = db.Tracks.ToList();
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
                Console.WriteLine($"ID: {p.PlaylistId} - Namn: {p.Name}");
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
            Console.WriteLine("Ange nytt namn på spellistan (lämna tomt för att behålla)");
            string newName = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(newName))
            {
                playlistToEdit.Name = newName;
                db.SaveChanges();
                Console.WriteLine("Spellistans namn uppdaterad!");
            }

            var currentTracks = db.PlaylistTracks.Where(pt => pt.PlaylistId == playlistToEdit.PlaylistId).Include(pt => pt.Track).ToList();

            Console.WriteLine("\nNuvarande låtar i spellistan:");

            if (!currentTracks.Any())
            {
                Console.WriteLine("Inga låtar.");
            }
            else
            {
                foreach (var pt in currentTracks)
                {
                    Console.WriteLine($"ID: {pt.TrackId} - {pt.Track.Name}");
                }
            }

            Console.WriteLine("\nVill du ta bort någon låt? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                Console.WriteLine("Skriv in Track ID på låten du vill ta bort (eller flera ID separerad med komma)");
                string input = Console.ReadLine();

                var idsToRemove = input.Split(',').Select(s => int.TryParse(s.Trim(), out int id) ? id : -1).Where(id => currentTracks.Any(pt => pt.TrackId == id)).ToList();

                foreach (var id in idsToRemove)
                {
                    var ptRemove = currentTracks.First(pt => pt.TrackId == id);
                    db.PlaylistTracks.Remove(ptRemove);
                }
                db.SaveChanges();
                Console.WriteLine("Låtar borttagna.");
            }

            Console.WriteLine("\nVill du lägga till nya låtar? (y/n)");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                var anyTracks = db.Tracks.ToList();
                foreach (var track in anyTracks)
                {
                    Console.WriteLine($"ID: {track.TrackId} - {track.Name}");
                }

                Console.WriteLine("\nSkriv in Track ID på låten du vill lägga till (eller flera ID separerad med komma):");
                string addInput = Console.ReadLine();
                var idsToAdd = addInput.Split(',')
                    .Select(s => int.TryParse(s.Trim(), out int id) ? id : -1)
                    .Where(id => anyTracks.Any(t => t.TrackId == id))
                    .ToList();

                foreach (var id in idsToAdd)
                {
                    var ptAdd = new PlaylistTrack
                    {
                        PlaylistId = playlistToEdit.PlaylistId,
                        TrackId = id
                    };
                    db.PlaylistTracks.Add(ptAdd);
                }
                db.SaveChanges();
                Console.WriteLine("\nLåtar tillagda.");
            }

            Console.WriteLine("\nTryck på valfri tagent för att återgå till menyn...");
            Console.ReadKey();

            break;



        case 4:
            var playlistsToDelete = db.Playlists.ToList();
            
            if (playlistsToDelete.Count == 0)
            {
                Console.WriteLine("Inga spellistor att ta bort");
                break;
            }

            Console.WriteLine("\nSpellistor:");
            foreach (var p in playlistsToDelete)
            {
                Console.WriteLine($"ID: {p.PlaylistId} - Namn: {p.Name}");
            }

            Console.WriteLine("Skriv ID på spellista att ta bort (eller separerade med komma):");
            string inputDelete = Console.ReadLine();

            string[] idParts = inputDelete.Split(',');

            foreach (string part in idParts)
            {
                if (!int.TryParse(part.Trim(), out int deleteId))
                {
                    Console.WriteLine($"Ogiltigt ID: {part}");
                    continue;
                }

                var playlist = db.Playlists
                    .FirstOrDefault(p => p.PlaylistId == deleteId);

                if (playlist == null)
                {
                    Console.WriteLine($"Playlist med ID {deleteId} hittades inte.");
                    continue;
                }

                // Ta bort kopplingar först
                var playlistTracks = db.PlaylistTracks
                    .Where(pt => pt.PlaylistId == deleteId)
                    .ToList();

                db.PlaylistTracks.RemoveRange(playlistTracks);

                // Ta bort playlist
                db.Playlists.Remove(playlist);

                Console.WriteLine($"Playlist {playlist.Name} borttagen.");
            }

            db.SaveChanges();

            Console.WriteLine("Spellistan har tagits bort!");

            Console.WriteLine("\nTryck på valfri tagent för att återgå till menyn...");
            Console.ReadKey();
            break;


        case 0:
                   
            return;


    }
}