using Sandbox;
using System;
using System.Linq;
using Fortnite.Entities;

namespace Fortnite
{
	partial class FortnitePlayer : Player
	{
		public FortnitePlayer()
		{
			Inventory = new BaseInventory( this );
		}
		
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Inventory.Add( new BuildTool()
			{
				CurrentBuildType = BuildTool.BuildType.Floor
			}, true );
			Inventory.Add( new BuildTool()
			{
				CurrentBuildType = BuildTool.BuildType.Stairs
			});
			Inventory.Add( new BuildTool()
			{
				CurrentBuildType = BuildTool.BuildType.Wall
			});

			base.Respawn();
		}

		public override void OnKilled()
		{
			base.OnKilled();
			
			Inventory.DeleteContents();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			
			SimulateActiveChild( cl, ActiveChild );
			
			// gross
			if ( Input.Pressed( InputButton.Slot1 ) )
			{
				Inventory.SetActiveSlot( 0, true );
			}
			else if ( Input.Pressed( InputButton.Slot2 ) )
			{
				Inventory.SetActiveSlot( 1, true );
			}
			else if ( Input.Pressed( InputButton.Slot3 ) )
			{
				Inventory.SetActiveSlot( 2, true );
			}
			else if ( Input.Pressed( InputButton.Slot4 ) )
			{
				Inventory.SetActiveSlot( 3, true );
			}
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );
			
			ActiveChild?.FrameSimulate( cl );
		}
	}
}
