﻿using System;

namespace CryBits.Entities
{
    [Serializable]
    public class Entity : IEquatable<Entity>
    {
        public Guid ID { get; }
        public string Name { get; set; } = string.Empty;

        public Entity()
        {
            ID = Guid.NewGuid();
        }

        public Entity(Guid id)
        {
            ID = id;
        }

        public override string ToString() => Name;

        public override int GetHashCode() => ID.GetHashCode();

        bool IEquatable<Entity>.Equals(Entity other) => other != null && other.ID.Equals(ID);
    }
}
