using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable<T>
{
    //returns true if this damage was lethal
    bool TakeDamage(ref T damageInfo);

    //GameEntity GetDamagedEntity();
    int GetTeamID();

    GameEntity GetGameEntity();
}
