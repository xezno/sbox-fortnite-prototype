using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Fortnite
{
	/// <summary>
	/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
	/// via RootPanel on this entity, or Local.Hud.
	/// </summary>
	public partial class FortniteHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public FortniteHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/FortniteHUD.html" );

				RootPanel.AddChild<ChatBox>();
				RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
				RootPanel.AddChild<NameTags>();
			}
		}
	}

}
