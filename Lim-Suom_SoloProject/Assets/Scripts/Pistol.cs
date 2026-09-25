using Unity.VisualScripting;
using UnityEngine;

public class Pistol : Weapon
{
    // Pistol already inherits everything from weapon class
    public void Awake()
    {
        damage = 5;
    }
}
