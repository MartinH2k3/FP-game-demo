using UnityEngine;

namespace Physics
{

public static class PhysicsMovableExtensions {
    public static void AddVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody != null) {
            movable.Rigidbody.linearVelocity += velocity;
        }
    }

    public static void AddVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody != null) {
            movable.Rigidbody.linearVelocity += new Vector2(x, y);
        }
    }

    public static void SetVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody != null) {
            movable.Rigidbody.linearVelocity = velocity;
        }
    }

    public static void SetVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody != null) {
            movable.Rigidbody.linearVelocity = new Vector2(x, y);
        }
    }

    public static void Freeze(this IPhysicsMovable movable) {
        movable.SetVelocity(Vector2.zero);
        movable.Rigidbody.simulated = false;
    }

    public static void Unfreeze(this IPhysicsMovable movable) {
        movable.Rigidbody.simulated = true;
    }
}

}