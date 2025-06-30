using Characters;

namespace Physics
{
public static class CrowdControl
{
    public static void Knockback(Character target, float verticalVelocity, float horizontalVelocity, int direction = 1, float duration = 0.3f) {
        if (target == null) return;

        target.SetVelocity(direction * verticalVelocity, horizontalVelocity);
        target.SetMovementTimeout(duration);
    }
}
}