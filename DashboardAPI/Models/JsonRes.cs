namespace DashboardAPI.Models
{
    public class JsonRes
    {
        public int Status { get; set; } = 200;
        public string StatusMessage { get; set; } = "Success";
        public object Data { get; set; } = string.Empty;
    }
}
