using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;




// All objects which move or play another role in the game and belong to a faction or take damage and can be taken into aim
// derive from this class. Also helps with game optimisation, cause it runs the update over its components.
// Some Examples: Soldier, Tank, Monster, Turrent, DamageableRock?.
public class GameEntity : MonoBehaviour
{
    [Header("Team")]
    public int teamID;
    [Header("Components to Update")]
    public EntityComponent[] components;

    [Header("Death")]
    public bool destroyOnDie;
    public UnityEvent onDieEvent;

    public bool isDead = false; //to prevent calling onDie more than once

   /* [Header("For Aiming of Enemies")]
    [Tooltip("collection of positions to aim at")]
    public Transform aimPosition;
    [Tooltip("collection of critical positions to aim at - like the head or some weakpoints")]
    public Transform criticalAimPosition;
    public float width;*/

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

    protected void LateUpdate()
    {
        foreach (EntityComponent ability in components)
        {
            ability.LateUpdateComponent();
        }
    }

    #endregion

  /*  public Vector3 GetAimPosition()
    {
        return aimPosition.position;
    }

    public Vector3 GetCriticalAimPosition()
    {
        return criticalAimPosition.position;
    }
  */


    public virtual void OnTakeDamage(ref DamageInfo damageInfo)
    {
        foreach (EntityComponent component in components)
        {
            component.OnTakeDamage(ref damageInfo);
        }
    }

    public virtual void OnDie(ref DamageInfo damageInfo)
    {
        if (!isDead)
        {
            isDead = true;

            onDieEvent.Invoke();
            foreach (EntityComponent component in components)
            {
                component.OnDie(ref damageInfo);
            }
            if (destroyOnDie) Destroy(gameObject);
        }
    }
}
