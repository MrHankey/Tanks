﻿using CryEngine;

namespace CryGameCode.Entities
{
	public abstract class DamageableEntity : Entity
	{
		public float Health { get; protected set; }
		public float MaxHealth { get; protected set; }
		public bool Dead { get; private set; }

		public void Damage(float damage, DamageType type)
		{
			Health = Math.Max(Health - damage, 0);
			OnDamage(damage, type);

			if(Health <= 0 && !Dead)
			{
				OnDeath();
				Dead = true;
			}
		}

		public void Heal(float amount)
		{
			Health = Math.Min(Health + amount, MaxHealth);
		}

		protected void InitHealth(float amount)
		{
			Health = amount;
			MaxHealth = amount;
			Dead = false;
		}

		protected virtual void OnDeath() { }
		protected virtual void OnDamage(float damage, DamageType type) { }
	}

	public enum DamageType
	{
		None,
		Bullet,
		Explosive,
		Laser
	}
}