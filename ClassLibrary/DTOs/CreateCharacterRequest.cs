namespace ClassLibrary.DTOs
{
    // Request DTO - used for incoming POST requests
    // This is separate from the response DTO to prevent overposting
    public class CreateCharacterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Gold { get; set; }
    }
}
