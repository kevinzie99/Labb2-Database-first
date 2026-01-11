

using Music.Model;

using var db = new MusicContext();

bool gameIsRunning = true;

while (gameIsRunning)
{

    Console.WriteLine("=== Music App ===");
    Console.WriteLine("1. Lista playlists");
    Console.WriteLine("2. Skapa playlist");
    Console.WriteLine("3. Ändra playlist");
    Console.WriteLine("4. Ta bort playlist");
    Console.WriteLine("0. Avsluta");

    string choice = Console.ReadLine();
    int answer = Int32.Parse(choice);

    switch (answer)
    {
        case 1:
            
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
