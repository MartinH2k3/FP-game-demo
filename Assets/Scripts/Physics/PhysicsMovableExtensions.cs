using UnityEngine;

namespace Physics
{

public static class PhysicsMovableExtensions {
    public static void AddVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity += velocity;
        }
    }

    public static void AddVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity += new Vector2(x, y);
        }
    }

    public static void SetVelocity(this IPhysicsMovable movable, Vector2 velocity) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity = velocity;
        }
    }

    public static void SetVelocity(this IPhysicsMovable movable, float x, float y) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.linearVelocity = new Vector2(x, y);
        }
    }

    public static void Freeze(this IPhysicsMovable movable) {
        movable.SetVelocity(Vector2.zero);
        movable.Rigidbody.angularVelocity = 0;
        movable.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        movable.Rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public static void Unfreeze(this IPhysicsMovable movable) {
        movable.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        movable.Rigidbody.constraints = RigidbodyConstraints2D.None;
    }

    public static Vector2 GetVelocity(this IPhysicsMovable movable) {
        return movable.Rigidbody.linearVelocity;
    }

    public static bool IsMoving(this IPhysicsMovable movable, bool includeX = true, bool includeY = true) {
        if (movable.Rigidbody is null) return false;

        var velocity = movable.Rigidbody.linearVelocity;
        return (includeX && Mathf.Abs(velocity.x) > Mathf.Epsilon) || (includeY && Mathf.Abs(velocity.y) > Mathf.Epsilon);
    }

    public static Vector2 GetDirection(this IPhysicsMovable movable) {
        return movable.Rigidbody.linearVelocity.normalized;
    }

    public static bool IsKinematic(this IPhysicsMovable movable) {
        return movable.Rigidbody.bodyType == RigidbodyType2D.Kinematic;
    }

    public static void NeverSleep(this IPhysicsMovable movable) {
        if (movable.Rigidbody is not null) {
            movable.Rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        }
    }
}

}