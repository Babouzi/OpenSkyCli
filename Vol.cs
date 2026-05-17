namespace OpenSkyCli;

public class Vol
{
    public string? Callsign { get; set; }
    public string? OriginCountry { get; set; }
    public float? Altitude { get; set; } // Le fameux float nullable
    public bool IsOnGround { get; set; }
}
