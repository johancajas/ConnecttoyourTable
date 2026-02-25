using ClassLibrary.DTOs;
using ClassLibrary.Entities;
using ClassLibrary.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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
// - ENFORCE BUSINESS RULES AND VALIDATION
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
            .Where(e => !e.IsDeleted)
            .Select(e => new CharacterDTO
            {
                Id = e.Id,
                Name = e.Name,
                Class = e.Class,
                Level = e.Level,
                Health = e.Health,
                Mana = e.Mana,
                Gold = e.Gold
            })
            .ToListAsync();
    }

    // ========== READ (Get By ID) ==========
    // Returns a single character by ID
    public async Task<CharacterDTO?> GetCharacterByIdAsync(int id)
    {
        return await _db.Characters
            .Where(e => e.Id == id && !e.IsDeleted)
            .Select(e => new CharacterDTO
            {
                Id = e.Id,
                Name = e.Name,
                Class = e.Class,
                Level = e.Level,
                Health = e.Health,
                Mana = e.Mana,
                Gold = e.Gold
            })
            .FirstOrDefaultAsync();
    }

    // ========== CREATE WITH VALIDATION ==========
    // Creates a new character with full server-side validation
    // Returns tuple: (success, errorMessage, characterDTO)
    public async Task<(bool Success, string? ErrorMessage, CharacterDTO? Data)> CreateCharacterWithValidationAsync(CreateCharacterRequest request)
    {
        // STEP 1: NORMALIZE DATA (clean before validating)
        var cleanedName = request.Name?.Trim() ?? string.Empty;
        var cleanedClass = request.Class?.Trim() ?? string.Empty;

        // STEP 2: VALIDATE NAME
        // Guard clause: Name required
        if (string.IsNullOrWhiteSpace(cleanedName))
        {
            return (false, "Name is required and cannot be empty.", null);
        }

        // Guard clause: Name max length
        if (cleanedName.Length > 20)
        {
            return (false, "Name cannot exceed 20 characters.", null);
        }

        // Guard clause: Name alphanumeric only
        if (!Regex.IsMatch(cleanedName, @"^[a-zA-Z0-9]+$"))
        {
            return (false, "Name can only contain letters and numbers.", null);
        }

        // STEP 3: VALIDATE CLASS (Enum)
        // Parse class string to enum (case insensitive)
        if (!Enum.TryParse<CharacterClass>(cleanedClass, ignoreCase: true, out var characterClass))
        {
            return (false, $"Invalid class. Must be one of: {string.Join(", ", Enum.GetNames<CharacterClass>())}.", null);
        }

        // STEP 4: VALIDATE LEVEL
        // Guard clause: Level range
        if (request.Level < 1 || request.Level > 50)
        {
            return (false, "Level must be between 1 and 50.", null);
        }

        // STEP 5: VALIDATE GOLD
        // Guard clause: Gold range
        if (request.Gold < 0 || request.Gold > 10000)
        {
            return (false, "Gold must be between 0 and 10,000.", null);
        }

        // STEP 6: CREATE ENTITY (set safe defaults)
        var entity = new CharacterEntity
        {
            Name = cleanedName,
            Class = characterClass.ToString(), // Store normalized enum value
            Level = request.Level,
            Gold = request.Gold,
            Health = 100, // Default health
            Mana = 100,   // Default mana
            IsAdmin = false, // SECURITY: Never trust client
            IsDeleted = false, // SECURITY: Never trust client
            CreatedAt = DateTime.UtcNow
        };

        _db.Characters.Add(entity);
        await _db.SaveChangesAsync();

        // STEP 7: Return cleaned DTO
        var dto = new CharacterDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Class = entity.Class,
            Level = entity.Level,
            Health = entity.Health,
            Mana = entity.Mana,
            Gold = entity.Gold
        };

        return (true, null, dto);
    }

    // ========== LEGACY CREATE (for backward compatibility) ==========
    public async Task<CharacterDTO> CreateCharacterAsync(CharacterDTO dto)
    {
        var entity = new CharacterEntity
        {
            Name = dto.Name,
            Class = dto.Class,
            Level = dto.Level,
            Health = dto.Health,
            Mana = dto.Mana,
            Gold = dto.Gold,
            IsAdmin = false,
            IsDeleted = false,
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
        entity.Gold = dto.Gold;

        await _db.SaveChangesAsync();

        // Return updated DTO
        return new CharacterDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Class = entity.Class,
            Level = entity.Level,
            Health = entity.Health,
            Mana = entity.Mana,
            Gold = entity.Gold
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
            SELECT character_id, name, class, level, health, mana, gold
            FROM character
            WHERE is_deleted = false
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
                Mana = reader.GetInt32(5),
                Gold = reader.GetInt32(6)
            });
        }

        return results;
    }
}
