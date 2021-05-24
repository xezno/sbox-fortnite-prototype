using Sandbox;

namespace Fortnite.Entities.Building
{
	[Library( "ent_floor" )]
	public class Floor : BuildingEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "Models/Building/Floor.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}
	}
	
	[Library( "ent_wall" )]
	public class Wall : BuildingEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "Models/Building/Wall.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}
	}
	
	[Library( "ent_stairs" )]
	public class Stairs : BuildingEntity
	{
		public override void Spawn()
		{
			base.Spawn();
			SetModel( "Models/Building/Stairs.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}
	}
}
