using NotepadClone.Application.Interfaces;
using NotepadClone.Domain;
using System.IO;
using System.Text.Json;

namespace NotepadClone.Infrastructure.Services;

public class UiStateService : IUiStateService
{
    private readonly string _stateFilePath;

    public UiStateService()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NotepadClone");
        Directory.CreateDirectory(appDataPath);
        _stateFilePath = Path.Combine(appDataPath, "ui-state.json");
    }

    public UiState Load()
    {
        try
        {
            if (!File.Exists(_stateFilePath))
            {
                return new UiState();
            }

            var json = File.ReadAllText(_stateFilePath);
            return JsonSerializer.Deserialize<UiState>(json) ?? new UiState();
        }
        catch
        {
            return new UiState();
        }
    }

    public void Save(UiState state)
    {
        try
        {
            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_stateFilePath, json);
        }
        catch
        {
            // Ignore persistence failures so the app remains usable.
        }
    }
}
