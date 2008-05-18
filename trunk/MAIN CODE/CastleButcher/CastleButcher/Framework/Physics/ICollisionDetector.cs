using System;
namespace Framework.Physics
{
    public interface ICollisionDetector
    {
        void AddObject(Framework.Physics.IPhysicalObject obj);
        event Framework.Physics.CollisionHandler OnCollision;
        event Framework.Physics.CollisionHandler OnPossibleCollision;
        void ProcessFrame(float frameTime);
        void RemoveObject(Framework.Physics.IPhysicalObject obj);
    }
}
