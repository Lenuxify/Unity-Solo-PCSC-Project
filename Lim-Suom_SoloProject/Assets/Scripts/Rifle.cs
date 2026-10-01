using UnityEngine;

public class Rifle : Weapon
{
    [Header("Rifle Exclusive")]
    public float semiAutoROF = 1;
    public float fullAutoROF = 0.1f;

    public void changeFireMode()
    {
        // Check if at least more than 1 fire mode
        if(fireModes >= 2)
        {
            currentFiremode++;

            // Reset firemode when it exceeds number of max firemodes
            if(currentFiremode >= fireModes)
            {
                fireModes = 0;
            }

            if(currentFiremode == 0)
            {
                holdToAttack = false;
                rof = semiAutoROF;
            }
            else
            {
                holdToAttack = true;
                rof = fullAutoROF;
            }
        }
    }
}
