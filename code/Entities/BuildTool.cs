using System;
using Fortnite.Entities.Building;
using Sandbox;

namespace Fortnite.Entities
{
	public partial class BuildTool : BaseCarriable
	{
		public enum BuildType
		{
			Floor,
			Stairs,
			Wall
		}

		[Net] public BuildType CurrentBuildType { get; set; } = BuildType.Wall;
		[Net] private Angles CurrentBuildRot { get; set; } = Angles.Zero;
		public virtual float PlaceRate => 1 / 0.05f;
		
		// Ghost entity should be client-only
		private ModelEntity CurrentGhostEntity { get; set; }

		public BuildTool()
		{
			// CurrentBuildType = BuildType.Wall;
		}

		public override void Spawn()
		{
			base.Spawn();

			CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
			SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it
		}

		[Net, Predicted]
		public TimeSince TimeSincePrimaryAttack { get; set; }

		public override void Simulate( Client player )
		{
			if ( player.Input.Pressed( InputButton.Reload ) )
			{
				Rotate();
			}

			if ( CanPlace() )
			{
				TimeSincePrimaryAttack = 0;
				Place();
			}
			
			DebugOverlay.ScreenText(Vector3.One * 100f, CurrentBuildType.ToString());
		}

		public virtual bool CanPlace()
		{
			if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

			var rate = PlaceRate;
			if ( rate <= 0 ) return true;

			return TimeSincePrimaryAttack > (1 / rate);
		}

		private Vector3 GridOffset => new Vector3( 0, 0, -78 );
		private Vector3 SnapToGrid( Vector3 vec, float gridSize = 160f, float gridHeight = 120f ) =>
			new Vector3( MathF.Round( vec.x / gridSize ) * gridSize, 
				MathF.Round( vec.y / gridSize ) * gridSize, 
				MathF.Round( vec.z / gridHeight ) * gridHeight ) + GridOffset;

		// We can run this on the client and the server in order to get the next place position
		private bool GetPlacePos( out Vector3 placePos, out Rotation placeRot, bool isGhost = false )
		{
			var traceResult = Trace.Ray( Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * 100 )
				.Ignore( Owner )
				.Ignore( CurrentGhostEntity )
				.Run();
			
			if ( traceResult.Entity is BuildingEntity )
			{
				if ( isGhost )
				{
					placePos = SnapToGrid( traceResult.Entity.Position );
					placeRot = traceResult.Entity.Rotation;
				}
				else
				{
					placePos = default;
					placeRot = default;
				}
				
				return false;
			}

			placePos = SnapToGrid( traceResult.EndPos );
			placeRot = Rotation.From( CurrentBuildRot );
			
			return true;
		}

		public override void FrameSimulate( Client cl )
		{
			base.FrameSimulate( cl );
			
			if ( IsServer ) return; // This function is client-side but we'll check just in case
			
			if ( !CurrentGhostEntity.IsValid() )
				return;
			
			// Move ghost
			if ( GetPlacePos(out var ghostPos, out var ghostRot, true ) )
			{				
				CurrentGhostEntity.Position = ghostPos;
				CurrentGhostEntity.Rotation = ghostRot;
			}
			else
			{
				CurrentGhostEntity.Position = Vector3.One * -1000; // Move super far out of bounds rather than deleting
			}
			
			DebugOverlay.ScreenText(Vector3.One * 120f, CurrentBuildType.ToString());
		}

		private ModelEntity GetCurrentBuildingType() =>
			CurrentBuildType switch
			{
				BuildType.Floor => new Floor(),
				BuildType.Stairs => new Stairs(),
				BuildType.Wall => new Wall(),
				_ => new Floor()
			};

		public void Place()
		{
			if ( GetPlacePos( out var placePos, out var placeRot ) )
			{
				var entity = GetCurrentBuildingType();
				entity.Position = placePos;
				entity.Rotation = placeRot;
				entity.SetupPhysicsFromModel( PhysicsMotionType.Static );
			}
		}
		
		public void Rotate()
		{
			CurrentBuildRot = CurrentBuildRot + new Angles( 0, 90f, 0);
		}
		
		#region "Ghost entity"

		private void CreateGhostEntity()
		{
			if ( IsServer ) return; // Only create the entity client-side

			CurrentGhostEntity = GetCurrentBuildingType();
			CurrentGhostEntity.SetMaterialGroup( 1 );
			CurrentGhostEntity.EnableAllCollisions = false;
			CurrentGhostEntity.PhysicsEnabled = false;
			CurrentGhostEntity.ClearCollisionLayers();
		}

		private void DestroyGhostEntity()
		{
			if ( IsServer ) return; // Only destroy the entity client-side

			CurrentGhostEntity?.Delete();
		}
		
		#endregion

		public override void ActiveStart( Entity ent )
		{
			base.ActiveStart( ent );
			CreateGhostEntity();
			// Place();
		}

		public override void ActiveEnd( Entity ent, bool dropped )
		{
			base.ActiveEnd( ent, dropped );
			DestroyGhostEntity();
		}
	}
}
