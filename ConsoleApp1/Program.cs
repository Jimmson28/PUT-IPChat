using ChatLogic;
using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("IPCHAT");
        Console.WriteLine("Wybierz tryb z poziomu klawiatury:");
        Console.WriteLine("[S](Kliknij S) - Serwer");
        Console.WriteLine("[C](Kliknij C) - Klient");
        Console.WriteLine("Serwer zostanie domyslnie uruchomiony na porcie 5000");
        var key = Console.ReadKey(true).Key;
        Console.WriteLine();

        if (key == ConsoleKey.S)
        {          
            Server server = new Server();
            server.start(5000);
            Console.WriteLine("Uruchomiono SERWER na porcie 5000...");
        }
        else if (key == ConsoleKey.C)
        {           
            RunClient();
        }
        else
        {
            Console.WriteLine("Wybier S lub C");
        }
    }

    public static void RunClient()
    {
        Console.WriteLine("Uruchamiany KLIENT...");
        Client client = new Client();        
        client.OnLog += (msg) => Console.WriteLine($"[LOG]: {msg}");
        client.OnMessageReceived += (msg) => Console.WriteLine(msg);    
        client.Connect("192.168.18.13", 5000);

        Console.WriteLine("Polaczono! [T]=TCP wiadomosc, [U]=UDP wiadomosc, [X]=Wyjscie");
        Console.WriteLine("Aby wyslac wiadomosc poprzez TCP kliknij na klawiaturze T i zacznij pisac swoja wiadomosc");
        Console.WriteLine("Aby wyslac wiadomosc poprzez UDP kliknij na klawiaturze U i zacznij pisac swoja wiadomosc");
        bool running = true;
        while (running)
        {
            var inputKey = Console.ReadKey(true);

            if (inputKey.Key == ConsoleKey.X)
            {
                running = false;
                client.Disconnect();
            }
            else if (inputKey.Key == ConsoleKey.T)
            {
                Console.Write("TCP > ");
                string msg = Console.ReadLine();
                client.SendTcp(msg);
            }
            else if (inputKey.Key == ConsoleKey.U)
            {
                Console.Write("UDP > ");
                string msg = Console.ReadLine();
                client.SendUdp(msg);
            }
        }
    }
}