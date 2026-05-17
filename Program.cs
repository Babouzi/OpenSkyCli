using OpenSkyCli;
using System.Net.Http;
using System.Text.Json;
// using System.Collections.Generic;


Console.WriteLine("Démarrage du radar OpenSky...");

//  Initialisation du client HTTP
using HttpClient client = new HttpClient();

string url = "https://opensky-network.org/api/states/all";
Console.WriteLine($"Connexion à l'API : {url}");

while (true)
{
    Console.Clear();
    Console.WriteLine($"Démarrage du radar Opensky... Mise à jour : {DateTime.Now.ToString("HH:mm:ss")}");
    Console.WriteLine($"Connexion à l'api {url}");
    try
{
    // Exécution de la requête (Asynchrone)
HttpResponseMessage response = await client.GetAsync(url);

// Vérification du statut de la réponse
response.EnsureSuccessStatusCode();

// Lecture du résultat sous forme de texte 
string jsonBrut = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Succès ! J'ai récupéré un JSON de {jsonBrut.Length} caractères.");

// N'oublie pas d'ajouter cette ligne tout en haut de Program.cs :
// 

Console.WriteLine("Analyse du JSON en cours...");


// On crée une liste vide pour stocker nos futurs objets Vol
// C'est l'équivalent de "vols = []" en Python ou "List<Vol> vols = new ArrayList<>();" en Java
List<OpenSkyCli.Vol> listeVols = new List<Vol>();

// On utilise 'using' car JsonDocument consomme de la mémoire pour construire son arbre
using JsonDocument doc = JsonDocument.Parse(jsonBrut);

// On va chercher la propriété "states" à la racine du JSON
JsonElement root = doc.RootElement;
JsonElement states = root.GetProperty("states");

// states est un tableau JSON. On le parcourt avec une boucle foreach.
foreach (JsonElement avionJson in states.EnumerateArray())
{
    // avionJson représente un avion (qui est lui-même un tableau de valeurs mélangées)
    // En C#, on peut extraire la valeur d'un index d'un tableau JSON et la convertir dans le bon type :
    // Exemple : string monTexte = avionJson[index].GetString();
    // Exemple : float? monNombre = avionJson[index].GetSingle(); // GetSingle() correspond au float
    // Exemple : bool monBool = avionJson[index].GetBoolean();

    string? callsign = avionJson[1].GetString();
    
    string? originCountry = avionJson[2].GetString();

    // Attention : comme ça peut être null, je te donne l'astuce pour celui-ci.
    // L'API peut envoyer un float, ou null, on vérifie donc d'abord le type de la donnée :
    float? altitudeTemp = null;
    if (avionJson[7].ValueKind != JsonValueKind.Null)
    {
        altitudeTemp = avionJson[7].GetSingle();
    }


    // TODO: Extraire le IsOnGround (Index 8)
    bool isOnGround = avionJson[8].GetBoolean();

    // On instancie notre objet et on le remplit
    Vol nouveauVol = new Vol
    {
        // TODO: Assigne tes variables extraites aux propriétés de l'objet
        Altitude = altitudeTemp,
        Callsign = callsign,
        OriginCountry = originCountry,
        IsOnGround = isOnGround,

    };

    // On ajoute l'avion à notre liste
    listeVols.Add(nouveauVol);
}

Console.WriteLine($"Extraction terminée ! Nous avons trouvé {listeVols.Count} avions.");

List<Vol> volsFiltres = listeVols
    .Where(v => v.OriginCountry == "France" && v.IsOnGround == false)
    .OrderByDescending(v => v.Altitude)
    .ToList();

Console.WriteLine($"Il y a {volsFiltres.Count} avions français en vol");

Console.WriteLine("\n---  TOP 10 DES VOLS FRANÇAIS LES PLUS HAUTS ---");

// Le .Take(10) est super pratique pour ne pas saturer l'affichage !
foreach (Vol v in volsFiltres.Take(10))
{
    // Le .Trim() permet d'enlever les espaces inutiles que l'API laisse parfois autour du Callsign
    Console.WriteLine($"Vol: {v.Callsign?.Trim()} | Pays: {v.OriginCountry} | Altitude: {v.Altitude} m");
}

Console.WriteLine("--------------------------------------------------");

    
} catch(HttpRequestException e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n ERREUR RADAR : Impossible de se connecter à OpenSky.");
    Console.WriteLine($"Détail : {e.Message}");
    Console.ResetColor(); // On remet la couleur normale
}
catch(Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n ERREUR CRITIQUE : {e.Message}");
    Console.ResetColor();
}

Console.WriteLine("\nAttente de 20 secondes avant le prochain scan...");
    await Task.Delay(20000);
    
}


