namespace ClassLibrary.DTOs
{
    //
    // DTO ROLE
    // ---------
    // DTOs define the SHAPE of data sent to or received from clients.
    //

    public class FuntestDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }
    }
}
