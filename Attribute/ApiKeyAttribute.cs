namespace MinimalAPI.Attribute;

public class ApiKeyAttribute
{
    private const string APIKEYNAME = "Api-Key";

    public bool IsValid(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
        {
            return false;
        }

        var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        var validApiKey = appSettings.GetValue<string>(APIKEYNAME);

        return extractedApiKey == validApiKey;
    }
}