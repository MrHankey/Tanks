﻿using CryEngine;
using CryGameCode.Entities;

namespace CryGameCode.Projectiles
{
	[Entity(Flags = EntityClassFlags.Invisible)]
	public class ChaingunBullet : Projectile
	{
		public override string Model { get { return "objects/projectiles/shell.cgf"; } }
		public override float Mass { get { return 20; } }
		public override float Speed { get { return 1400; } }
		public override string Effect { get { return "explosions.explosive_bullet.default"; } }
		public override float EffectScale { get { return 0.3f; } }
		public override float Damage { get { return 10; } }
		public override DamageType DamageType { get { return DamageType.Explosive; } }
		public override bool ShouldExplode { get { return false; } }
	}
}
