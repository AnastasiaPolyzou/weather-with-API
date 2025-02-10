using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class WeatherApp
{
    private static readonly string apiKey = "Place your API key";  // Βαζω το API key 

    public static async Task GetWeatherAsync(string city)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        using (HttpClient client = new HttpClient())  // Δημιουργία HttpClient
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(responseBody);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("main", out JsonElement main) && root.TryGetProperty("weather", out JsonElement weatherArray) && weatherArray.GetArrayLength() > 0)
                {
                    float temp = main.GetProperty("temp").GetSingle();
                    string description = weatherArray[0].GetProperty("description").GetString();
                    Console.WriteLine($"Weather in {city}: {temp}°C, {description}");
                }
                else
                {
                    Console.WriteLine("Unexpected response format.");
                }
            }
            else
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Could not retrieve weather data. Response: {responseBody}");
            }
        }
    }

    static async Task Main()
    {
        bool continueSearching = true;

        while (continueSearching)
        {
            // Ζητάμε από τον χρήστη να εισάγει το όνομα της πόλης
            Console.WriteLine("Enter city name (e.g., Athens): ");
            string city = Console.ReadLine().Trim();

            // Καλούμε τη συνάρτηση για να εμφανίσουμε τον καιρό για την πόλη
            await GetWeatherAsync(city);

            // Ρωτάω τον χρήστη αν θέλει να συνεχίσει ή να βγει από την εφαρμογή
            Console.WriteLine("\nDo you want to search for another city? (Y/y to continue, E/e to exit): ");
            string userInput = Console.ReadLine().Trim().ToLower();

            if (userInput == "e")
            {
                continueSearching = false;
                Console.WriteLine("Exiting the application...");
            }
            else if (userInput != "y")
            {
                // Αν η είσοδος δεν είναι ούτε "y" ούτε "e", ρωτάμε ξανά
                Console.WriteLine("Invalid input. Please enter 'y' to continue or 'e' to exit.");
            }
        }
    }
}