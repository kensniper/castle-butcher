using System;
using System.Collections.Generic;
using System.Text;
using Framework;

namespace CastleButcher.GameEngine.AI
{
    //class AIPlayer:Player,IUpdateable
    //{

    //    GenericTimer eventTimer=new GenericTimer();
    //    //SteeringDevice2 sdev;
    //    public AIPlayer(string name, ShipClass ship)
    //        : base(name, ship)
    //    {
    //        //sdev = new SteeringDevice2(this.CurrentShip);
    //    }


    //    protected override void OnDestroyed(DestroyableObj destroyedObj)
    //    {
    //        base.OnDestroyed(destroyedObj);
    //        eventTimer.Reset();
    //        eventTimer.Start();
    //    }

    //    public override void OnRespawned()
    //    {
    //        base.OnRespawned();

    //        this.CurrentShip.SteeringDevice.Reset(this.CurrentShip);
    //        this.CurrentShip.SteeringDevice.Velocity = 1f;
    //        this.CurrentShip.SteeringDevice.TurnX = 0.1f;
    //        this.CurrentShip.SteeringDevice.Roll = 0f;


    //    }
    //    public override void OnMissileHit(IMissile missile, Framework.Physics.CollisionParameters parameters)
    //    {
    //        base.OnMissileHit(missile, parameters);
    //    }

    //    public override void OnShipCollision(Ship enemyShip, Framework.Physics.CollisionParameters parameters)
    //    {
    //        base.OnShipCollision(enemyShip, parameters);
    //    }

    //    public override void OnStaticCollision(IGameObject staticObject, Framework.Physics.CollisionParameters parameters)
    //    {
    //        base.OnStaticCollision(staticObject, parameters);
    //    }

    //    public override void OnWeaponPickup(CastleButcher.GameEngine.Weapons.FloatingWeapon weapon)
    //    {
    //        base.OnWeaponPickup(weapon);
    //    }

    //    public override void OnEnemyDestroyed(Player enemy)
    //    {
    //        base.OnEnemyDestroyed(enemy);
    //    }





    //    #region IUpdateable Members

    //    public bool Update(float timeElapsed)
    //    {
    //        eventTimer.Update();
    //        if (IsAlive == false && eventTimer.RunningTime > 1)
    //        {
    //            World.Instance.RespawnPlayer(this);
    //            eventTimer.Stop();
    //        }

    //        if (IsAlive)
    //        {
    //            this.CurrentShip.SteeringDevice.Update(timeElapsed);
    //        }
            
    //        return true;
    //    }

    //    #endregion
    //}
}
