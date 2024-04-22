using Godot;
using System;
using System.Linq;

namespace Gameplay.Mods
{
	public abstract class Mod : IEquatable<Mod>
	{
		public abstract string Name { get; }
		public abstract string Acronym { get; }
		public abstract string Description { get; }
		public abstract double ScoreMultiplier { get; }
		public abstract double PerformanceMultiplier { get; }
		public abstract Type[] IncompatibleMods { get; }
		public bool IncompatableCompatibleWith(Mod other)
		{
			return IncompatibleMods.Any(t => t == other.GetType()) || other.IncompatibleMods.Any(t => t == GetType());
		}
		public virtual ModType Type { get; } = ModType.Misc;
		public bool Equals(Mod other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return GetType() == other.GetType();
		}
		public override string ToString()
		{
			return Acronym;
		}
	}

	public enum ModType
	{
		Buff,
		Nerf,
		Misc
	}
}