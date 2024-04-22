using Godot;
using System;

namespace Gameplay.Mods
{
	public class ModSpeed : Mod, IApplicableToSyncManager
	{
		public override string Name => "Speed";
		public override string Acronym => "S";
		public override string Description => "Speed up or down the map";
		public override double ScoreMultiplier => 1;
		public override double PerformanceMultiplier => 1;
		public override Type[] IncompatibleMods => new Type[0];
		public override ModType Type => ModType.Misc;
        public float SpeedMultiplier = 1f;
        public void ApplyToSyncManager(SyncManager syncManager)
        {
            syncManager.Speed = SpeedMultiplier;
        }

        public override string ToString()
        {
            return "S" + (SpeedMultiplier * 100).ToString();
        }
    }
}