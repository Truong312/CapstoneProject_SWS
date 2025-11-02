namespace SWS.Services.Shared
{
    public static class ValidationMessages
    {
        public static ArgumentException Invalid(string formatHint)
            => new ArgumentException($"Invalid format. Expected {formatHint}.");
    }
}
