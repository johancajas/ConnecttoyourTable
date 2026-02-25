using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;

//
// CONTROLLER ROLE
// ----------------
// Controllers are the "front door" of your backend.
// They receive HTTP requests, call services to do the work,
// and translate results into HTTP responses.
//
// Controllers should be THIN:
// - No database logic
// - No business rules
// - No data transformation logic
//

[ApiController] // Enables automatic model validation & API behavior
[Route("api/characters")] // Base route for this controller
public class CharacterController : ControllerBase
{
    // Service that contains the business/data logic
    private readonly CharacterService _characterService;

    // Constructor Injection:
    // ASP.NET gives us CharacterService automatically via Dependency Injection
    public CharacterController(CharacterService characterService)
    {
        _characterService = characterService;
    }

    // GET: api/characters
    // Returns a list of all characters
    [HttpGet]
    public async Task<ActionResult<List<CharacterDTO>>> GetCharacters()
    {
        // Ask the service for character data
        var characters = await _characterService.GetCharactersAsync();

        // Return HTTP 200 OK with JSON data
        return Ok(characters);
    }

    // GET: api/characters/{id}
    // Returns a single character by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterDTO>> GetCharacter(int id)
    {
        var character = await _characterService.GetCharacterByIdAsync(id);

        if (character == null)
            return NotFound(new { message = $"Character with ID {id} not found" });

        return Ok(character);
    }

    // POST: api/characters
    // Creates a new character with SERVER-SIDE VALIDATION
    // This endpoint demonstrates why frontend validation is NOT enough
    [HttpPost]
    public async Task<ActionResult<CharacterDTO>> CreateCharacter([FromBody] CreateCharacterRequest request)
    {
        // Call service with validation logic
        var (success, errorMessage, character) = await _characterService.CreateCharacterWithValidationAsync(request);

        // If validation failed, return 400 Bad Request with error message
        if (!success)
        {
            return BadRequest(new { error = errorMessage });
        }

        // Return 201 Created with the cleaned/normalized character
        return CreatedAtAction(nameof(GetCharacter), new { id = character!.Id }, character);
    }

    // PUT: api/characters/{id}
    // Updates an existing character
    [HttpPut("{id}")]
    public async Task<ActionResult<CharacterDTO>> UpdateCharacter(int id, [FromBody] CharacterDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _characterService.UpdateCharacterAsync(id, dto);

        if (updated == null)
            return NotFound(new { message = $"Character with ID {id} not found" });

        return Ok(updated);
    }

    // DELETE: api/characters/{id}
    // Deletes a character
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCharacter(int id)
    {
        var success = await _characterService.DeleteCharacterAsync(id);

        if (!success)
            return NotFound(new { message = $"Character with ID {id} not found" });

        return NoContent(); // HTTP 204 No Content
    }
}
