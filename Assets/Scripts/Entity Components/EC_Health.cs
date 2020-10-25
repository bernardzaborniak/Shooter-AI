using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EC_Health : EntityComponent
{
    public float currentHealth;
    public float maxHealth;

    [Header("Visuals")]
    public Image healthBarFill; // for later
    public bool changeColorOnDamage;
    bool colorChanged;
    public Material damageMaterial;
    Material[] normalMaterials;
    public MeshRenderer[] renderersToTint;
    public float changeColorTime = 0.2f;
    float nextGoBackToNormalColorTime;

    [Header("Hitbox Logic")]
    public Hitbox[] hitboxes;

   

    public override void SetUpComponent(GameEntity entity)
    {
        base.SetUpComponent(entity);
        //Unit unit = entity.GetComponent<Unit>();
        //if(unit!=null)maxHealth = unit.unitData.healthPoints;
        currentHealth = maxHealth;


        normalMaterials = new Material[renderersToTint.Length];

        for (int i = 0; i < renderersToTint.Length; i++)
        {
            normalMaterials[i] = renderersToTint[i].material;
        }

        for (int i = 0; i < hitboxes.Length; i++)
        {
            hitboxes[i].SetUp(myEntity, this);
        }
    }

    public bool TakeDamage(ref DamageInfo damageInfo)
    {
        //returns true if health falls below 0
        currentHealth -= damageInfo.damage;

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            myEntity.OnDie(ref damageInfo);
            return true;
        }
        else
        {
            myEntity.OnTakeDamage(ref damageInfo);
            return false;
        }

        /*if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }

        if (changeColorOnDamage)
        {
            nextGoBackToNormalColorTime = Time.time + changeColorTime;

            colorChanged = true;
            for (int i = 0; i < renderersToTint.Length; i++)
            {
                renderersToTint[i].material = damageMaterial;
            }
        }*/

        /*if (damageInfo.appliesForce)
        {
            IPusheable<Vector3> pusheable = GetComponent<IPusheable<Vector3>>();

            if (pusheable != null)
            {
                pusheable.Push(damageInfo.damageForce);
            }
           
        }*/
    }

    public override void UpdateComponent()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            DamageInfo di = new DamageInfo(51);
            TakeDamage(ref di);
        }

        base.UpdateComponent();

        if (colorChanged)
        {
            if(Time.time> nextGoBackToNormalColorTime)
            {
                colorChanged = false;

                for (int i = 0; i < renderersToTint.Length; i++)
                {
                    renderersToTint[i].material = normalMaterials[i];

                    // You can re-use this block between calls rather than constructing a new one each time.
                    //var block = new MaterialPropertyBlock();

                    // You can look up the property by ID instead of the string to be more efficient.
                    //block.SetColor("_BaseColor", normalColor[i]);

                    // You can cache a reference to the renderer to avoid searching for it.
                    //renderersToTint[i].SetPropertyBlock(block);
                }
            }
        }
    }

    public void AddHealth(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    
}
