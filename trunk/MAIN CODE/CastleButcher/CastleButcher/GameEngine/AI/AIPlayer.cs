using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine.AI
{
    class AIPlayer : Player, IUpdateable
    {

        GenericTimer eventTimer = new GenericTimer();
        //SteeringDevice2 sdev;
        public AIPlayer(string name, CharacterClass characterClass)
            : base(name, characterClass)
        {
            //sdev = new SteeringDevice2(this.CurrentShip);
        }


        public override void OnDestroyed(DestroyableObj destroyedObj)
        {
            base.OnDestroyed(destroyedObj);
            eventTimer.Reset();
            eventTimer.Start();
        }

        public override void OnRespawned()
        {
            base.OnRespawned();

            this.CurrentCharacter.CharacterController.Reset(this.CurrentCharacter);
            this.CurrentCharacter.CharacterController.Velocity = 1f;
            this.CurrentCharacter.CharacterController.TurnX = 0f;


        }
        public override void OnMissileHit(IMissile missile, Framework.Physics.CollisionParameters parameters)
        {
            base.OnMissileHit(missile, parameters);
        }

        public override void OnCharacterCollision(Character otherChar, Framework.Physics.CollisionParameters parameters)
        {
            base.OnCharacterCollision(otherChar, parameters);
        }

        public override void OnStaticCollision(IGameObject staticObject, Framework.Physics.CollisionParameters parameters)
        {
            base.OnStaticCollision(staticObject, parameters);
            if(parameters.CollisionNormal.Y<0.1f)
                this.CharacterController.Velocity *= -1;
        }

        public override void OnWeaponPickup(CastleButcher.GameEngine.Weapons.WeaponPickup weapon)
        {
            base.OnWeaponPickup(weapon);
        }

        public override void OnEnemyDestroyed(Player enemy)
        {
            base.OnEnemyDestroyed(enemy);
        }





        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            eventTimer.Update();
            if (IsAlive == false && eventTimer.RunningTime > 1)
            {
                //World.Instance.RespawnPlayer(this);
                eventTimer.Stop();
            }

            if (IsAlive)
            {
                this.CurrentCharacter.CharacterController.Update(timeElapsed);
            }

            return true;
        }

        #endregion
    }
}
