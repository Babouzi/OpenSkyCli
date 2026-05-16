using System.Net.Http;

Console.WriteLine("Démarrage du radar OpenSky...");

//  Initialisation du client HTTP
using HttpClient client = new HttpClient();

string url = "https://opensky-network.org/api/states/all";
Console.WriteLine($"Connexion à l'API : {url}");

// Exécution de la requête (Asynchrone)
HttpResponseMessage response = await client.GetAsync(url);

// Vérification du statut de la réponse
response.EnsureSuccessStatusCode();

// Lecture du résultat sous forme de texte 
string jsonBrut = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Succès ! J'ai récupéré un JSON de {jsonBrut.Length} caractères.");

