using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace StarSmuggler
{
    public static class SaveLoadManager
    {
        private static string SavePath => Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "StarSmugglerGame",
            "save.json"
        );

        public static void SaveGame(PlayerData player, GameState? stateToSave = null)
        {
            if (player?.CurrentPort == null)
            {
                System.Console.WriteLine("No active game to save.");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SavePath));

            var save = new SaveData
            {
                CurrentPortName = player.CurrentPort.Name,
                Credits = player.Credits,
                CargoLimit = player.CargoLimit,
                CargoHold = new Dictionary<string, int>(),
                Prices = player.CurrentPrices,
                SavedState = NormalizeSavedState(stateToSave ?? GameManager.Instance.CurrentState)
            };

            foreach (var item in player.CargoHold)
            {
                save.CargoHold[item.Key.Name] = item.Value;
            }

            string json = JsonSerializer.Serialize(save, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);
        }

        private static GameState NormalizeSavedState(GameState state)
        {
            return state switch
            {
                GameState.GameOver => GameState.GameOver,
                GameState.TravelScreen => GameState.TravelScreen,
                _ => GameState.PortOverview
            };
        }

        public static bool TryLoadGame(out SaveData data)
        {
            data = null;
            if (!File.Exists(SavePath)) return false;

            string json = File.ReadAllText(SavePath);
            data = JsonSerializer.Deserialize<SaveData>(json);
            return data != null;
        }
    }
}
