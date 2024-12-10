namespace MinimalAPI.Models;
public class BaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public int CountData { get; set; }
    public string? Runtime { get; set; }
    public dynamic Data { get; set; }

    public string AdditionalData { get; set; }
}
