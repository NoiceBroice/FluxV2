using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Gameplay.Mods
{
	public class ModList : List<Mod>
	{
		public new void Add(Mod mod)
		{
			foreach (Mod other in this.ToList().Where(m => m.GetType() == mod.GetType()))
				this.Remove(other);
			foreach (Mod other in this.ToList().Where(m => mod.IncompatableCompatibleWith(m)))
				this.Remove(other);
			base.Add(mod);
		}
		public new string ToString()
		{
			return Count == 0 ? "None" : string.Join(", ", this.Select(x => x.ToString()));
		}

		public void ApplyAll(Game game)
		{
			foreach(IApplicableToGame m in this) m.ApplyToGame(game);
		}
	}
}