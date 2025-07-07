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
}

}