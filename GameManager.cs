using System;
using System.Collections.Generic;
using StarSmuggler.Events;

namespace StarSmuggler
{
    public class GameManager
    {
        public static GameManager Instance { get; private set; } = new GameManager();

        public PlayerData Player { get; private set; }
        public GameState CurrentState { get; private set; }
        public Port CurrentPort => Player.CurrentPort;
        private GameEvent lastEvent;
        private GameState? previousState;


        private GameManager() { }

        public void LoadGame()
        {
            if (SaveLoadManager.TryLoadGame(out var data))
            {
                Player = new PlayerData(data.Credits, data.CargoLimit);

                var port = PortsDatabase.AllPorts.Find(p => p.Name == data.CurrentPortName);
                if (port == null) port = PortsDatabase.GetRandomInnerPort();

                Player.CurrentPort = port;

                foreach (var pair in data.CargoHold)
                {
                    var good = GoodsDatabase.AllGoods.Find(g => g.Name == pair.Key);
                    if (good != null)
                        Player.CargoHold[good] = pair.Value;
                }

                LoadGoodsForCurrentPort();
                SetGameState(GameState.PortOverview); 
            }
            else
            {
                StartNewGame();
            }
        }

        public void StartNewGame()
        {
            var startingPort = PortsDatabase.GetRandomInnerPort();
            Player = new PlayerData(startingCredits: 500, cargoLimit: 30);
            Player.CurrentPort = startingPort;
            LoadGoodsForCurrentPort();

            SetGameState(GameState.PortOverview); 
        }

        public void TravelToPort(Port destination, int cost)
        {
            if (Player.Credits >= cost)
            {
                Console.WriteLine($"Current port: {Player.CurrentPort.Name}");
                Player.Credits -= cost;
                Player.CurrentPort = destination;
                Console.WriteLine($"Destination port: {Player.CurrentPort.Name}");
                LoadGoodsForCurrentPort();
                SetGameState(GameState.PortOverview); 
                TriggerRandomEvent();
                SaveLoadManager.SaveGame(Player);
                // Autosave after successful travel

            }
            else
            {
                // Handle insufficient funds
                // (e.g., show warning or disable option in UI)
            }
        }

        public void LoadGoodsForCurrentPort()
        {
            var available = GoodsDatabase.GetCommonAndMidTier(6);
            Player.CurrentPort.AvailableGoods = available;
        }

        public void SetGameState(GameState newState)
        {
            if (CurrentState != GameState.MainMenu)
                previousState = CurrentState;
            CurrentState = newState;
            if (Game1.ScreenManagerRef != null)
                Game1.ScreenManagerRef.SetActive(newState);
                // SaveLoadManager.SaveGame(Player);
        }

        private void TriggerRandomEvent()
        {
            var rng = new Random();
            lastEvent = EventDatabase.AllEvents[rng.Next(EventDatabase.AllEvents.Count)];
            lastEvent.Execute(Player, Player.CurrentPort);
        }
        public GameEvent GetLastEvent()
        {
            return lastEvent;
        }

        public bool HasPreviousState()
        {
            return previousState.HasValue;
        }

        public void ReturnToPreviousState()
        {
            if (previousState.HasValue)
            {
                SetGameState(previousState.Value);
                previousState = null;
            }
        }
    }
}
