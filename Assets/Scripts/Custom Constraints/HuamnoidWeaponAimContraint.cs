using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuamnoidWeaponAimContraint : MonoBehaviour
{
    [Header("Weapon Aiming")]
    public Transform weaponAimTarget;
    public Transform weaponAimTransform;
    public float weaponAimWeight;
    Quaternion weaponAimLocalStartRotation;

    void Start()
    {
        weaponAimLocalStartRotation = weaponAimTransform.localRotation;
    }

    void LateUpdate()
    {
        #region Orient the Weapon Aim Constraint

        Vector3 directionToTarget = weaponAimTarget.position - weaponAimTransform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);


        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(weaponAimTransform.parent.rotation * weaponAimLocalStartRotation);
        weaponAimTransform.rotation = Quaternion.Slerp(Quaternion.identity, rotationDifference, weaponAimWeight) * (weaponAimTransform.parent.rotation * weaponAimLocalStartRotation);


        #endregion
    }
}
