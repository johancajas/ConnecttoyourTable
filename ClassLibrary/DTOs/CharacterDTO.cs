namespace ClassLibrary.DTOs
{
    //
    // DTO ROLE
    // ---------
    // DTOs define the SHAPE of data sent to or received from clients.
    //
    // DTOs:
    // - Do NOT reference EF Core
    // - Do NOT match the database exactly
    // - Protect internal fields from being exposed(easier to hack if they are known)
    //
    // Think of DTOs as your API contract.
    //

    public class CharacterDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Class { get; set; } = string.Empty;

        public int Level { get; set; }

        public int Health { get; set; }

        public int Mana { get; set; }
    }
}
