using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using System.Xml;
using Framework.Physics;
using Framework.MyMath;
using CastleButcher.GameEngine.Weapons;
using Microsoft.DirectX;

namespace CastleButcher.GameEngine
{

    public delegate void PlayerEventHandler(Player player);
    public delegate void CharacterEventHandler(Character character);
    public delegate void ObjectEventHandler(IGameObject obj);
    public delegate void GrenadeEventHandler(Grenade grenade, MyVector position);
    /// <summary>
    /// Klasa "�wiata" czyli pojemnika na obiekty. Ta klasa realizuje wszelkiego rodzaniu oddzia�ywania mi�dzy obiektami
    /// </summary>
    public class World : IDeviceRelated, IUpdateable
    {
        Graphics.Environment environment;

        public Graphics.Environment Environment
        {
            get { return environment; }
            set { environment = value; }
        }
        List<IGameObject> gameObjects;
        List<IUpdateable> updateableObjects;
        List<ITemporaryObject> temporaryObjects;
        //List<Ship> ships;
        List<Player> players;


        bool remote = false;

        public bool Remote
        {
            get { return remote; }
            set { remote = value; }
        }

        public List<Player> Players
        {
            get { return players; }
            set { players = value; }
        }
        List<Player> assassinPlayers;

        public List<Player> AssassinPlayers
        {
            get { return assassinPlayers; }
            set { assassinPlayers = value; }
        }
        List<Player> knightPlayers;

        public List<Player> KnightPlayers
        {
            get { return knightPlayers; }
            set { knightPlayers = value; }
        }
        List<RespawnPoint> respawnPoints;

        RigidBodySimulator physicsSimulator;



        public RigidBodySimulator PhysicsSimulator
        {
            get { return physicsSimulator; }
        }
        ICollisionDetector collisionDetector;

        private static World _instance;

        public static World Instance
        {
            get { return World._instance; }
        }

        private bool started = false;

        public bool Started
        {
            get { return started; }
        }



        public List<IGameObject> GameObjects
        {
            get { return gameObjects; }
        }

        public event PlayerEventHandler OnPlayerAdded;
        public event PlayerEventHandler OnPlayerRemoved;
        public event PlayerEventHandler OnPlayerKilled;
        public event PlayerEventHandler OnPlayerRespawned;

        public event CharacterEventHandler OnCharacterAdded;
        public event CharacterEventHandler OnCharacterRemoved;

        public event ObjectEventHandler OnObjectAdded;
        public event ObjectEventHandler OnObjectRemoved;

        public event GrenadeEventHandler OnGrenadeHit;


        public World()
        {
            gameObjects = new List<IGameObject>();
            respawnPoints = new List<RespawnPoint>();
            updateableObjects = new List<IUpdateable>();
            temporaryObjects = new List<ITemporaryObject>();
            players = new List<Player>();
            assassinPlayers = new List<Player>();
            knightPlayers = new List<Player>();
            physicsSimulator = new RigidBodySimulator();
            collisionDetector = physicsSimulator.CollisionDetector;
            collisionDetector.OnCollision += new CollisionHandler(this.OnCollision);

            physicsSimulator.Paused = true;

            physicsSimulator.DoAngularDrag = false;
            physicsSimulator.DoLinearDrag = false;
            physicsSimulator.DoGravity = false;

            if (_instance != null)
            {
                _instance.Close();
            }
            _instance = this;

        }

        #region IDeviceRelated Members

        public void OnCreateDevice(Microsoft.DirectX.Direct3D.Device device)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnResetDevice(Microsoft.DirectX.Direct3D.Device device)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnLostDevice(Microsoft.DirectX.Direct3D.Device device)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void OnDestroyDevice(Microsoft.DirectX.Direct3D.Device device)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IUpdateable Members

        public bool Update(float timeElapsed)
        {
            if (PhysicsSimulator.Paused == false)
            {
                PhysicsSimulator.AdvanceSimulation(timeElapsed);

                for (int i = 0; i < updateableObjects.Count; i++)
                {
                    updateableObjects[i].Update(timeElapsed);
                }
                for (int i = 0; i < temporaryObjects.Count; i++)
                {
                    if (temporaryObjects[i].RemainingTime < 0)
                    {
                        //
                        this.RemoveObject(temporaryObjects[i] as IGameObject);
                    }
                }

                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].IsAlive)
                    {
                        float compensationSpeed = 5 + (10 * players[i].CurrentCharacter.YMotionToCompensate);
                        players[i].CurrentCharacter.YMotionToCompensate -= compensationSpeed * timeElapsed;
                        if (players[i].CurrentCharacter.YMotionToCompensate < 0)
                            players[i].CurrentCharacter.YMotionToCompensate = 0;
                    }
                }


                return true;
            }
            return false;
        }

        #endregion

        public void AddObject(IGameObject obj)
        {
            gameObjects.Add(obj);

            if (obj.PhysicalData != null)
            {
                PhysicsSimulator.AddObject(obj.PhysicalData);
            }
            else if (obj is IPhysicalObject && (obj as IPhysicalObject).CollisionDataType != CollisionDataType.None)
            {
                collisionDetector.AddObject(obj as IPhysicalObject);
            }
            DestroyableObj dobj = obj as DestroyableObj;
            if (dobj != null)
            {
                dobj.OnDestroyed += new ObjectDestructionHandler(this.RemoveObject);
            }

            if (obj is IUpdateable)
            {
                this.updateableObjects.Add(obj as IUpdateable);
            }

            if (obj is ITemporaryObject)
            {
                this.temporaryObjects.Add(obj as ITemporaryObject);
            }

            if (obj is RespawnPoint)
            {
                respawnPoints.Add(obj as RespawnPoint);
            }

            if (OnObjectAdded != null)
            {
                OnObjectAdded(obj);
            }
            if (obj is Character)
            {
                if (OnCharacterAdded != null)
                    OnCharacterAdded(obj as Character);
            }

        }
        public void RemoveObject(IGameObject obj)
        {
            gameObjects.Remove(obj);

            if (obj.PhysicalData != null)
            {
                physicsSimulator.RemoveObject(obj.PhysicalData);
            }
            else if ((obj as IPhysicalObject).CollisionDataType != CollisionDataType.None)
            {
                collisionDetector.RemoveObject(obj as IPhysicalObject);

            }
            if (obj is IUpdateable)
            {
                updateableObjects.Remove(obj as IUpdateable);
            }
            if (obj is ITemporaryObject)
            {
                temporaryObjects.Remove(obj as ITemporaryObject);
            }
            if (obj is DestroyableObj)
            {
                (obj as DestroyableObj).OnDestroyed -= new ObjectDestructionHandler(this.RemoveObject);
            }

            //if (obj is Ship)
            //{
            //    PlayerKilled((obj as Ship).Pilot);
            //}

            if (obj is RespawnPoint)
            {
                respawnPoints.Remove(obj as RespawnPoint);
            }

            if (OnObjectRemoved != null)
            {
                OnObjectRemoved(obj);
            }
            if (obj is Character)
            {
                if (OnCharacterRemoved != null)
                    OnCharacterRemoved(obj as Character);
            }
        }

        public void AddPlayer(Player pilot)
        {
            players.Add(pilot);
            if (pilot.CharacterClass != null)
            {
                if (pilot.CharacterClass.GameTeam == GameTeam.Assassins)
                {
                    assassinPlayers.Add(pilot);
                }
                else if (pilot.CharacterClass.GameTeam == GameTeam.Knights)
                {
                    knightPlayers.Add(pilot);
                }
            }
            //RespawnPilot(pilot);
            if (OnPlayerAdded != null)
                OnPlayerAdded(pilot);

            if (Started)
                RespawnPlayer(pilot);

            if (pilot is IUpdateable)
            {
                GM.AppWindow.AddUpdateableItem(pilot as IUpdateable);
                this.updateableObjects.Add(pilot as IUpdateable);
            }
        }
        public void RemovePlayer(Player pilot)
        {
            players.Remove(pilot);
            if (pilot.CharacterClass != null)
            {
                if (pilot.CharacterClass.GameTeam == GameTeam.Assassins)
                    assassinPlayers.Remove(pilot);
                else
                    assassinPlayers.Remove(pilot);
            }
            if (pilot.CurrentCharacter != null)
            {
                RemoveObject(pilot.CurrentCharacter);
            }
            if (OnPlayerRemoved != null)
            {
                OnPlayerRemoved(pilot);
            }
        }
        public void ChangeTeam(Player player, GameTeam newTeam)
        {
            if (player.CharacterClass != null)
            {
                if (newTeam != player.CharacterClass.GameTeam)
                {
                    RemovePlayer(player);
                    if (newTeam == GameTeam.Assassins)
                    {
                        player.CharacterClass = ObjectCache.Instance.GetAssassinClass();
                    }
                    else if (newTeam == GameTeam.Knights)
                    {
                        player.CharacterClass = ObjectCache.Instance.GetKnightClass();
                    }
                    AddPlayer(player);
                }
            }
            else
            {
                if (newTeam == GameTeam.Assassins)
                {
                    player.CharacterClass = ObjectCache.Instance.GetAssassinClass();
                }
                else if (newTeam == GameTeam.Knights)
                {
                    player.CharacterClass = ObjectCache.Instance.GetKnightClass();
                }
                AddPlayer(player);
            }
        }




        public void Start()
        {
            physicsSimulator.SimulationTick = 0.03f;
            physicsSimulator.DoGravity = true;
            physicsSimulator.Gravity = new MyVector(0, -100, 0);
            physicsSimulator.MaxStepHeight = 3;

            PhysicsSimulator.Paused = false;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i] is DeadCharacter)
                {
                    RemoveObject(gameObjects[i]);
                    i--;
                }
            }

            started = true;
            //sort playetrs by id
            bool ok = false;
            while (!ok)
            {
                ok = true;
                for (int i = 1; i < players.Count; i++)
                {
                    if (players[i - 1].NetworkId > players[i].NetworkId)
                    {
                        Player t = players[i - 1];
                        players[i - 1] = players[i];
                        players[i] = t;
                        ok = false;
                    }
                }
            }
            foreach (Player p in players)
            {
                this.RespawnPlayer(p);
            }
            SoundSystem.SoundEngine.StopMusic();
            //SoundSystem.SoundEngine.PlayMusic(SoundSystem.Enums.MusicTypes.round1Music);

        }

        public bool Paused
        {
            get
            {
                return PhysicsSimulator.Paused;
            }
            set
            {
                PhysicsSimulator.Paused = value;
            }
        }
        public void RespawnPlayer(Player p)
        {
            if (Started == false) return;
            if (p.IsAlive == false)
            {
                foreach (RespawnPoint rpoint in respawnPoints)
                {
                    if (rpoint.Ready && rpoint.Team == p.CharacterClass.GameTeam)
                    {
                        //
                        p.IsAlive = true;
                        if (p.CurrentCharacter == null || p.CurrentCharacter is SpectatingCharacter || p.CurrentCharacter.CharacterClass == null ||
                            p.CurrentCharacter.CharacterClass != p.CharacterClass)
                        {
                            if (p.CurrentCharacter != null)
                                RemoveObject(p.CurrentCharacter);

                            Character character = new Character(p, p.CharacterClass, rpoint.Position, rpoint.Orientation);

                            p.CurrentCharacter = character;
                            this.AddObject(p.CurrentCharacter);

                            //GM.AppWindow.AddUpdateableItem(p.CurrentShip);
                        }
                        else
                        {
                            p.CurrentCharacter.Reset(rpoint.Position, rpoint.Orientation);
                            this.AddObject(p.CurrentCharacter);
                        }
                        physicsSimulator.EnableWalking(p.CurrentCharacter);
                        physicsSimulator.WalkData[p.CurrentCharacter] = p.CurrentCharacter.WalkingCollisionData;
                        rpoint.Reset();


                        PlayerRespawned(p);
                        return;
                    }
                }
            }
            else
            {
                foreach (RespawnPoint rpoint in respawnPoints)
                {
                    if (rpoint.Ready && rpoint.Team == p.CharacterClass.GameTeam)
                    {
                        p.CurrentCharacter.Reset(rpoint.Position, MyQuaternion.FromEulerAngles(0, 0, 0));

                        p.IsAlive = true;
                        physicsSimulator.EnableWalking(p.CurrentCharacter);
                        physicsSimulator.WalkData[p.CurrentCharacter] = p.CurrentCharacter.WalkingCollisionData;

                        rpoint.Reset();
                        PlayerRespawned(p);
                        return;
                    }
                }
            }

        }

        private void PlayerRespawned(Player p)
        {
            p.OnRespawned();
            //SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.fanfare1, (Vector3)p.CurrentCharacter.Position);
            if (OnPlayerRespawned != null)
                OnPlayerRespawned(p);
        }

        //public static World FromFile(string fileName)
        //{
        //    World world = new World();

        //    MapData data = MapData.FromFile(fileName);
        //    world.gameObjects.AddRange(data.GameObjects);

        //    foreach (GameObject obj in data.GameObjects)
        //    {
        //        world.AddObject(obj);
        //    }


        //    return world;
        //}

        public static void FromFileBkg(string fileName, ProgressReporter reporter)
        {
            World world = new World();


            MapData.FromFileBkg(fileName, reporter);
            if (reporter.Data == null)
                throw new Exception("nie udao sie wczytac mapy!!"); //zle poszlo
            //reporter.Complete = false;


            foreach (IGameObject obj in ((MapData)reporter.Data).GameObjects)
            {
                //if (obj is RespawnPoint)
                //{
                //    world.respawnPoints.Add(obj as RespawnPoint);
                //    //((RespawnPoint)obj).
                //}
                //else
                //{
                world.AddObject(obj);
                //}

                //prawdopodobnie zb�dne
                //if (obj is IUpdateable)
                //{
                //    GM.AppWindow.AddUpdateableItem(obj as IUpdateable);
                //}
            }
            world.environment = Graphics.Environment.FromFile(fileName);


            reporter.Data = world;
            reporter.Complete = true;
        }
        public static void InitializeTest()
        {
            World world = new World();
            CollisionMesh mesh = Resources.ResourceCache.Instance.GetCollisionMesh("castle.cm");

            CollisionOctree tree = CollisionOctree.FromMesh(mesh, 0, 0, 0, 30, 2, 200);
            StaticMesh worldMesh = new StaticMesh(tree, CollisionDataType.CollisionOctree,
                Resources.ResourceCache.Instance.GetRenderingData("castle2.x"), new MyVector(0, 0, 0));
            world.AddObject(worldMesh);


        }
        //private int ComputeDamage(CollisionParameters parameters)
        //{
        //    return parameters.RelativeVelocity.Length*(parameters.A.Ma
        //}
        private bool OnCollision(CollisionParameters parameters)
        {
            IPhysicalObject o1 = parameters.A;
            IPhysicalObject o2 = parameters.B;
            if (o1 is IMissile && o2 is DestroyableObj)
                return MissileToDestroyable(o1 as IMissile, o2 as DestroyableObj, parameters);
            else if (o2 is IMissile && o1 is DestroyableObj)
                return MissileToDestroyable(o2 as IMissile, o1 as DestroyableObj, parameters);
            else if (o1 is DestroyableObj && o2 is DestroyableObj)
                return DestroyableToDestroyable(o1 as DestroyableObj, o2 as DestroyableObj, parameters);
            else if (o1 is IGameObject && o2 is DestroyableObj)
                return DestroyableToStatic(o2 as DestroyableObj, o1 as IGameObject, parameters);
            else if (o2 is IGameObject && o1 is DestroyableObj)
                return DestroyableToStatic(o1 as DestroyableObj, o2 as IGameObject, parameters);
            else if (o1 is IMissile && o2 is IGameObject)
                return MissileToStatic(o1 as IMissile, o2 as IGameObject, parameters);
            else if (o2 is IMissile && o1 is IGameObject)
                return MissileToStatic(o2 as IMissile, o1 as IGameObject, parameters);
            return true;
        }

        private void GrenadeExplosion(Grenade grenade, CollisionParameters parameters)
        {
            float maxDistance = grenade.ImpactDamage/4;
            float impactDamage = grenade.ImpactDamage;
            if (OnGrenadeHit != null)
                OnGrenadeHit(grenade, parameters.CollisionPoint);
            RemoveObject((IGameObject)grenade);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].IsAlive)
                {
                    MyVector pos = players[i].CurrentCharacter.Position;

                    if ((pos - parameters.CollisionPoint).Length < maxDistance)
                    {
                        int damage = (int)(impactDamage * ((pos - parameters.CollisionPoint).Length / maxDistance));


                        players[i].OnMissileHit(grenade, parameters);
                        if (remote)
                            damage = 0;
                        if (players[i].CurrentCharacter.TakeDamage(damage))
                        {

                            ((Character)grenade.Owner).Player.OnEnemyDestroyed(players[i]);

                            World.Instance.PlayerKilled(players[i]);

                        }
                    }
                }
            }
        }
        private bool MissileToStatic(IMissile missile, IGameObject obj, CollisionParameters parameters)
        {
            //RemoveObject((IGameObject)missile);
            if (!(missile is Grenade))
            {
                (missile as IGameObject).PhysicalData.Velocity = new MyVector(0, 0, 0);
                SoundSystem.SoundEngine.PlaySound(SoundSystem.Enums.SoundTypes.arrowOff1, (Vector3)parameters.CollisionPoint);
            }
            else
            {
                GrenadeExplosion(missile as Grenade, parameters);
            }
            return false;
        }
        private bool MissileToDestroyable(IMissile missile, DestroyableObj obj, CollisionParameters parameters)
        {
            if (obj is Character && missile.Owner == obj)
                return false;
            if (missile is Grenade)
            {
                GrenadeExplosion(missile as Grenade, parameters);
            }
            else
            {
                int damage = missile.ImpactDamage;
                if (obj is Character)
                {
                    (obj as Character).Player.OnMissileHit(missile, parameters);
                }
                if (remote)
                    damage = 0;
                if (obj.TakeDamage(damage))
                {
                    if (obj is Character)
                    {
                        ((Character)missile.Owner).Player.OnEnemyDestroyed((obj as Character).Player);
                        RemoveObject((IGameObject)missile);
                        World.Instance.PlayerKilled((obj as Character).Player);
                    }
                    else
                    {
                        RemoveObject((IGameObject)missile);
                    }
                }
            }




            return false;
        }
        private bool DestroyableToDestroyable(DestroyableObj obj1, DestroyableObj obj2, CollisionParameters parameters)
        {
            //int damage = (int)Math.Abs(parameters.RelativeVelocity.Dot(parameters.CollisionNormal));
            //obj1.TakeDamage(damage);
            //obj2.TakeDamage(damage);

            //if (obj1 is Ship)
            //{
            //    (obj1 as Ship).Pilot.OnShipCollision(obj2 as Ship, parameters);
            //}
            //if (obj2 is Ship)
            //{
            //    (obj2 as Ship).Pilot.OnShipCollision(obj1 as Ship, parameters);
            //}
            return false;
        }

        private bool DestroyableToStatic(DestroyableObj obj1, IGameObject obj2, CollisionParameters parameters)
        {
            if (obj1 is Character && obj2 is WeaponPickup)
            {
                if ((obj2 as WeaponPickup).Ready && ((obj1 as Character).CharacterClass.GameTeam ==
                    (obj2 as WeaponPickup).WeaponClass.GameTeam || (obj2 as WeaponPickup).WeaponClass.GameTeam == GameTeam.Both))
                {
                    //zbieranie broni
                    (obj1 as Character).Weapons.AddWeapon((obj2 as WeaponPickup).WeaponClass);
                    (obj1 as Character).Player.OnWeaponPickup((obj2 as WeaponPickup));
                    (obj2 as WeaponPickup).Use();
                }
                return false;
            }
            else
            {
                if (obj1 is Character)
                {
                    (obj1 as Character).Player.OnStaticCollision(obj2, parameters);
                    if (parameters.RelativeVelocity.Length > 100)
                    {
                        int damage = (int)parameters.RelativeVelocity.Length - 100;
                        if (obj1.TakeDamage(damage))
                        {
                            PlayerKilled((obj1 as Character).Player);
                        }

                    }



                }
                return true;
            }
            return false;
        }

        public void PlayerKilled(Player player)
        {
            physicsSimulator.DisableWalking(player.CurrentCharacter);
            RemoveObject(player.CurrentCharacter);
            AddObject(new DeadCharacter(player.CurrentCharacter.CharacterClass.DeadRd,player.CurrentCharacter.Position));
            player.OnDestroyed(player.CurrentCharacter);


            if (OnPlayerKilled != null)
                OnPlayerKilled(player);
        }
        public void Close()
        {
            this.gameObjects.Clear();
            this.players.Clear();
            this.respawnPoints.Clear();
            this.temporaryObjects.Clear();
            this.updateableObjects.Clear();
            _instance = null;
        }
    }
}

