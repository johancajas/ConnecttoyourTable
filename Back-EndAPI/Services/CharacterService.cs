using ClassLibrary.DTOs;
using ClassLibrary.Entities;
using Microsoft.EntityFrameworkCore;

//
// SERVICE ROLE
// -------------
// Services contain BUSINESS LOGIC and DATA ACCESS.
// Controllers should never talk directly to the database.
//
// Services:
// - Decide WHAT data to fetch
// - Decide HOW data is shaped
// - Return DTOs (safe for UI)
//
// This keeps controllers simple and testable.
//

public class CharacterService
{
    // Database context injected via Dependency Injection
    private readonly AppDbContext _db;

    public CharacterService(AppDbContext db)
    {
        _db = db;
    }

    // Returns characters as DTOs (not entities)
    public async Task<List<CharacterDTO>> GetCharactersAsync()
    {
        // Query the database and PROJECT directly into DTOs
        // EF Core generates optimized SQL that selects only needed columns
        return await _db.Characters
            .Select(e => new CharacterDTO
            {
                Id = e.Id,
                Name = e.Name,
                Class = e.Class,
                Level = e.Level,
                Health = e.Health,
                Mana = e.Mana
            })
            .ToListAsync();
    }

    // ========== READ (Get By ID) ==========
    // Returns a single character by ID
    public async Task<CharacterDTO?> GetCharacterByIdAsync(int id)
    {
        return await _db.Characters
            .Where(e => e.Id == id)
            .Select(e => new CharacterDTO
            {
                Id = e.Id,
                Name = e.Name,
                Class = e.Class,
                Level = e.Level,
                Health = e.Health,
                Mana = e.Mana
            })
            .FirstOrDefaultAsync();
    }

    // ========== CREATE ==========
    // Adds a new character to the database
    public async Task<CharacterDTO> CreateCharacterAsync(CharacterDTO dto)
    {
        var entity = new CharacterEntity
        {
            Name = dto.Name,
            Class = dto.Class,
            Level = dto.Level,
            Health = dto.Health,
            Mana = dto.Mana,
            CreatedAt = DateTime.UtcNow
        };

        _db.Characters.Add(entity);
        await _db.SaveChangesAsync();

        // Return the created character with its new ID
        dto.Id = entity.Id;
        return dto;
    }

    // Updates an existing character
    public async Task<CharacterDTO?> UpdateCharacterAsync(int id, CharacterDTO dto)
    {
        var entity = await _db.Characters.FindAsync(id);
        if (entity == null)
            return null;

        // Update properties
        entity.Name = dto.Name;
        entity.Class = dto.Class;
        entity.Level = dto.Level;
        entity.Health = dto.Health;
        entity.Mana = dto.Mana;

        await _db.SaveChangesAsync();

        // Return updated DTO
        return new CharacterDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Class = entity.Class,
            Level = entity.Level,
            Health = entity.Health,
            Mana = entity.Mana
        };
    }

    // ========== DELETE ==========
    // Deletes a character by ID
    public async Task<bool> DeleteCharacterAsync(int id)
    {
        var entity = await _db.Characters.FindAsync(id);
        if (entity == null)
            return false;

        _db.Characters.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    // Example: SAME DATA, but using raw SQL instead of EF LINQ
    // This is for learning / reference purposes
    public async Task<List<CharacterDTO>> GetCharactersWithSqlAsync()
    {
        var results = new List<CharacterDTO>();

        // Get the raw database connection EF is using
        using var conn = _db.Database.GetDbConnection();
        await conn.OpenAsync();

        // Create a SQL command
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT character_id, name, class, level, health, mana
            FROM character
        """;

        // Execute query and read results
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new CharacterDTO
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Class = reader.GetString(2),
                Level = reader.GetInt32(3),
                Health = reader.GetInt32(4),
                Mana = reader.GetInt32(5)
            });
        }

        return results;
    }
}
