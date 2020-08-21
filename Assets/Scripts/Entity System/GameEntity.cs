using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




// All objects which move or play another role in the game and belong to a faction or take damage and can be taken into aim
// derive from this class. Also helps with game optimisation, cause it runs the update over its components.
// Some Examples: Soldier, Tank, Monster, Turrent, DamageableRock?.
public class GameEntity : MonoBehaviour
{
    public int teamID;
    public EntityComponent[] components;
    //[Tooltip("Every Unit should have its origin in the ground, the aiming corrector should be a vector in the y direction with halfe the units height as length, so other units can aim into the middle of this unit")]
    //public Vector3 aimingCorrector;  //replace this with a transform tracked to a bone or something?
    public bool destroyOnDie;
    public UnityEvent onDieEvent;
    public float width;

    //public EC_Health health;

    #region Handling Entity Components

    private void Start()
    {
        foreach (EntityComponent component in components)
        {
            component.SetUpComponent(this);
        }
    }

    protected void Update()
    {
        foreach (EntityComponent ability in components)
        {
            ability.UpdateComponent();
        }
    }

    protected void FixedUpdate()
    {
        foreach (EntityComponent ability in components)
        {
            ability.FixedUpdateComponent();
        }
    }

    #endregion

    public Vector3 GetPositionForAiming()
    {
       // return (transform.position + aimingCorrector);
        return (transform.position);
    }

    /*public virtual void OnTakeDamage(DamageInfo damageInfo)
    {
        foreach (EntityComponent component in components)
        {
            component.OnTakeDamage(damageInfo);
        }
    }*/

    public virtual void OnDie()//GameEntity killer)
    {
        onDieEvent.Invoke();
        foreach (EntityComponent component in components)
        {
            component.OnDie();//killer);
        }
        if(destroyOnDie)Destroy(gameObject);
    }
}
