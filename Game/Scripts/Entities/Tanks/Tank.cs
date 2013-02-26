﻿using CryEngine;
using CryGameCode.Entities;

namespace CryGameCode.Tanks
{
	public partial class Tank : DamageableActor
	{
        public override void OnSpawn()
        {
            Reset(true);
        }

        protected override void PostSerialize()
        {
            Reset(true);
        }

		public void OnRevive()
		{
			ZoomLevel = 1;

            m_tankInput = new TankInput(this);

			OnDestroyed += (e) =>
			{
                m_tankInput.Destroy();

				if(Turret != null)
				{
					Turret.Destroy();
					Turret = null;
				}

				ReceiveUpdates = false;
			};

			Reset(true);

            if (Network.IsMultiplayer)
                Entity.Spawn<Cursor>("Cursor", null, null, null, true, EntityFlags.CastShadow | EntityFlags.ClientOnly);
            else
                Entity.Spawn<Cursor>("Cursor");
		}

		protected override void OnEditorReset(bool enteringGame)
		{
			Reset(enteringGame);
		}

        [RemoteInvocation]
        void NetReset(bool enteringGame, string turretTypeName)
        {
            if (enteringGame)
            {
                var turretType = System.Type.GetType(turretTypeName);
                if (turretType == null)
                {
                    Turret = new Autocannon(this);
                    Debug.LogAlways("Turret type {0} could not be located", turretTypeName);
                }
                else
                    Turret = System.Activator.CreateInstance(turretType, this) as TankTurret;
            }
            else
            {
                Turret.Destroy();
                Turret = null;
            }
        }

		private void Reset(bool enteringGame)
		{
            LoadObject("objects/tanks/tank_generic_" + Team + ".cdf");

            if(Network.IsServer)
            {
                string turretType;

                if (string.IsNullOrEmpty(GameCVars.ForceTankType))
                    turretType = GameCVars.TurretTypes[SinglePlayer.Selector.Next(GameCVars.TurretTypes.Count)].FullName;
                else
                    turretType = "CryGameCode.Tanks." + GameCVars.ForceTankType;

                if (IsLocalClient)
                    NetReset(enteringGame, turretType);

                RemoteInvocation(NetReset, NetworkTarget.ToAllClients | NetworkTarget.NoLocalCalls, enteringGame, turretType);
            }

			m_leftTrack = GetAttachment("track_left");
			m_rightTrack = GetAttachment("track_right");

			// Unhide just in case
			Hide(false);

			Physics.AutoUpdate = false;
			Physics.Type = PhysicalizationType.Living;
			Physics.Mass = 500;
			Physics.HeightCollider = 1.2f;
			Physics.Slot = 0;
			Physics.UseCapsule = false;
			Physics.SizeCollider = new Vec3(2.2f, 2.2f, 0.2f);
			Physics.FlagsOR = PhysicalizationFlags.MonitorPostStep;
            Physics.MaxClimbAngle = MathHelpers.DegreesToRadians(30);
			Physics.AirControl = 0.0f;
			Physics.Save();

            m_acceleration = new Vec2();

			InitHealth(100);

			ReceiveUpdates = true;
		}

        protected override void NetSerialize(CryEngine.Serialization.CrySerialize serialize, int aspect, byte profile, int flags)
        {
            serialize.BeginGroup("Tank");



            serialize.EndGroup();
        }

		public override void OnUpdate()
		{
			if(Turret != null)
				Turret.Update();

            if (Physics.Status != null)
            {
                float blend = MathHelpers.Clamp(Time.DeltaTime / 0.15f, 0, 1.0f);
                GroundNormal = (GroundNormal + blend * (Physics.Status.Living.GroundNormal - GroundNormal));
            }
		}

		string team;
		[EditorProperty]
		public string Team
		{
			get { return team ?? "red"; }
			set
			{
				var gameRules = GameRules.Current as SinglePlayer;
                if (gameRules != null && gameRules.IsTeamValid(value))
                {
                    team = value;
                }
			}
		}

        private TankInput m_tankInput;

        public TankTurret Turret { get; set; }
		private Attachment m_leftTrack;
		private Attachment m_rightTrack;

        public Vec3 GroundNormal { get; set; }
	}
}
