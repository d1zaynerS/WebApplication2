using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


string apiKey = "defe1947bfae45e4830134635261408 "; 

app.Run(async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    string html = await File.ReadAllTextAsync("html/index.html");


    string city = context.Request.Query["city"];

    string weatherResult = "";

    if (!string.IsNullOrEmpty(city))
    {
        using var client = new HttpClient();
        string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&lang=uk";

        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            var json = JsonNode.Parse(jsonString);

            string cityName = json["location"]["name"]?.ToString();
            string country = json["location"]["country"]?.ToString();
            string temp = json["current"]["temp_c"]?.ToString();
            string condition = json["current"]["condition"]["text"]?.ToString();
            string icon = json["current"]["condition"]["icon"]?.ToString();

            weatherResult = $@"
                <div style='background-color: #f0f4f8; padding: 15px; border-radius: 8px; width: 300px;'>
                    <h2>{cityName}, {country}</h2>
                    <p><img src='https:{icon}' alt='icon'/> <strong>{condition}</strong></p>
                    <p>Температура: <strong>{temp} °C</strong></p>
                </div>";
        }
        else
        {
            weatherResult = $"<p style='color: red;'><strong>Помилка: Місто '{city}' не знайдено!</strong></p>";
        }
    }


    html = html.Replace("{{WEATHER}}", weatherResult);
    await context.Response.WriteAsync(html);
});

app.Run();