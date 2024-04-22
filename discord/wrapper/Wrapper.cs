using Godot;
using System;

namespace Discord
{
	public class DiscordW
	{
		public static long ClientId { get; private set; } = 1231849122336604171;
		public bool Disposed { get; private set; } = false;
		private Discord Discord;
		public DiscordW()
		{
			if (OS.HasFeature("pc"))
			{
				try
				{
					Discord = new Discord(ClientId, (ulong)CreateFlags.NoRequireDiscord);
				}
				catch (ResultException e)
				{
					GD.PrintErr($"No Discord | {e.Message}");
				}
			}
		}
		public void Dispose()
		{
			if (this.Disposed) return;
			if (Disposed) return;
			Disposed = true;
			this.Discord?.Dispose();
			this.Discord = null;
		}
		public void SetActivity(ActivityW activity)
		{
			if (this.Disposed) return;
			if (this.Discord == null) return;
			var activityManager = this.Discord.GetActivityManager();
			activityManager.UpdateActivity(activity.Activity, (result) => { });
		}
		public void RunCallbacks()
		{
			if (this.Disposed) return;
			if (this.Discord == null) return;
			this.Discord?.RunCallbacks();
		}
	}
	public class ActivityW
	{
		public Activity Activity { get; private set; }
		public DateTime? StartTimestamp
		{
			get
			{
				if (this.Activity.Timestamps.Start < 1) return null;
				return DateTimeOffset.FromUnixTimeSeconds(this.Activity.Timestamps.Start).DateTime;
			}
			set
			{
				var activity = this.Activity;
				if (value != null)
					activity.Timestamps.Start = ((DateTimeOffset)value).ToUnixTimeSeconds();
			}
		}
		public DateTime? EndTimestamp
		{
			get
			{
				if (this.Activity.Timestamps.End < 1) return null;
				return DateTimeOffset.FromUnixTimeSeconds(this.Activity.Timestamps.End).DateTime;
			}
			set
			{
				var activity = this.Activity;
				if (value != null)
					activity.Timestamps.End = ((DateTimeOffset)value).ToUnixTimeSeconds();
			}
		}
		public string State
		{
			get => this.Activity.State;
			set
			{
				var activity = this.Activity;
				activity.State = value;
			}
		}
		public string Details
		{
			get => this.Activity.Details;
			set
			{
				var activity = this.Activity;
				activity.Details = value;
			}
		}
		public ActivityW(string state = "Idle", string details = null, DateTime? startTimestamp = null, DateTime? endTimestamp = null)
		{
			var activity = new Activity();
			activity.ApplicationId = DiscordW.ClientId;
			activity.Assets.LargeImage = "fluxlogo";
			activity.Assets.LargeText = "Flux";
			activity.Assets.SmallImage = "fluxlogo";
			if (startTimestamp != null) activity.Timestamps.Start = ((DateTimeOffset)startTimestamp).ToUnixTimeSeconds();
			if (endTimestamp != null) activity.Timestamps.End = ((DateTimeOffset)endTimestamp).ToUnixTimeSeconds();
			activity.State = state;
			activity.Details = details;
			this.Activity = activity;
			this.StartTimestamp = startTimestamp;
			this.EndTimestamp = endTimestamp;
			this.State = state;
			this.Details = details;
		}
	}
}
