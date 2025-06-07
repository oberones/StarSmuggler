using System;

namespace StarSmuggler.Events
{
    public class GameEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Action to modify game state (e.g., Player, Port)
        public Action<PlayerData> ApplyEffect { get; set; }

        public GameEvent(string name, string description, Action<PlayerData> effect)
        {
            Name = name;
            Description = description;
            ApplyEffect = effect;
        }

        public void Execute(PlayerData player)
        {
            ApplyEffect?.Invoke(player);
        }
    }
}
