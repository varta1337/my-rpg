using System;

namespace MyDiscordBot
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Vector2Int Position { get; protected set; }
        public bool IsActive { get; protected set; }

        protected Entity(string name, Vector2Int position)
        {
            Name = name;
            Position = position;
            IsActive = true;
        }

        public virtual void Update()
        {
            // Base update logic
        }

        public virtual bool ShouldDespawn()
        {
            return !IsActive;
        }

        public virtual void SetPosition(Vector2Int newPosition)
        {
            Position = newPosition;
        }
    }
} 