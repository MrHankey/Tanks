﻿using System;
using CryEngine;
using CryGameCode.Projectiles;

namespace CryGameCode.Tanks
{
	[Entity(Category = "Tanks")]
	public class RocketTank : TankTurret
	{
		public RocketTank(Tank tank) : base(tank) { }

		public override string Model { get { return "objects/tanks/turret_rocket.chr"; } }
		public override string LeftHelper { get { return "turret_term_2"; } }
		public override string RightHelper { get { return "turret_term_1"; } }
		public override float TimeBetweenShots { get { return 0.3f; } }
		public override Type ProjectileType { get { return typeof(Rocket); } }
	}
}