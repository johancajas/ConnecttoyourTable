using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

public class IndexBase : ComponentBase
{
    [Inject]
    protected HttpClient Http { get; set; } = default!;

    // UI only knows about DTOs
    protected List<CharacterDTO> Characters = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCharacters();
    }

    protected async Task LoadCharacters()
    {
        Characters = await Http.GetFromJsonAsync<List<CharacterDTO>>("api/characters");
    }
}