namespace Shop.Shared.Models
{
    public class ErrorResponse
    {
        public string Message { get; set; }
        public FieldError[] Errors { get; set; }
    }

    public class FieldError
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}
