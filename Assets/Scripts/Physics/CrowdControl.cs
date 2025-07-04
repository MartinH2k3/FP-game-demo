using Characters;

namespace Physics
{
public static class CrowdControl
{
    public static void Knockback(Character target, float horizontalVelocity, float verticalVelocity, int direction = 1, float duration = 0.3f) {
        if (target == null) return;

        target.SetVelocity(direction * horizontalVelocity, verticalVelocity);
        target.SetMovementTimeout(duration);
    }
}
}