using UnityEngine;

namespace Environment.Weapons
{

public class Ball : ThrowableWeapon {
    public override void Launch() {
        // Implement the logic for launching the ball
        // This could involve setting its velocity, direction, etc.
        Debug.Log("Ball launched!");
    }

    public override void Launch(float x, float y) {
        throw new System.NotImplementedException();
    }
}

}