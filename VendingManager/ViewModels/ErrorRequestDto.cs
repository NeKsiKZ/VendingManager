namespace VendingManager.ViewModels
{
    public class ErrorRequestDto
    {
        public string ErrorCode { get; set; } = "UNKNOWN";
        public string Message { get; set; } = "Brak szczegółów błędu.";
    }
}
