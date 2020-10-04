using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class RecoilManager : MonoBehaviour
{
    public bool usedByPlayer;
    [Tooltip("The recoil will be applied to this transform to allow the use of transform.transformPoint()")]
    public Transform transformToApplyRecoilTo;

    #region Up Recoil Values

    // Set by the gun
    float maxRecoilUpSpeed;
    float maxReduceRecoilUpAcceleration;
    float maxReduceRecoilUpSpeed;
    float maxRotationUp;

    // Keep track of recoil
    float currentRotationUp;
    float currentUpVelocity;
    float currentReduceRecoilUpVelocity;
    bool reduceRecoilUp;

    bool brakeUp;
    float remainingUpDistance;
    float currentUpBreakDistance;     // Represents the distance the hand needs to deccelerate to 0 degrees/s at its current speed. Calculated without considering friction.

    #endregion

    #region Side Recoil Values

    // Set by the gun
    float maxRecoilSideSpeed;
    float maxReduceRecoilSideAcceleration;
    float maxReduceRecoilSideSpeed;
    float maxRotationSide;

    // Keep track of recoil
    float currentRotationSide;
    float currentSideVelocity;
    float currentReduceRecoilSideVelocity;
    bool reduceRecoilSide;

    bool brakeSide;
    float remainingSideDistance;
    float currentSideBreakDistance;

    #endregion

    #region Back Recoil Values

    // Set by the gun
    float maxRecoilBackSpeed;
    float maxReduceRecoilBackAcceleration;
    float maxReduceRecoilBackSpeed;
    float maxPositionBack;

    // Keep track of recoil
    float currentPositionBack;
    float currentBackVelocity;
    float currentReduceRecoilBackVelocity;
    bool reduceRecoilBack;

    bool brakeBack;
    float remainingBackDistance;
    float currentBackBreakDistance;

    #endregion

    #region Debug Time calculation Fields
    //count the forces per minute for UP only for now: maybe usefull for setting up the values later?
    float recoilForceCounter;
    float counterRecoilForceCounter;
    float nextCountTime;

    float lastRecoilTime;
    #endregion

    //[Header("Debug")]
    //public bool debugVisualisation;
    //public GameObject visualRepresentation;

   /* private void Start()
    {
        if (debugVisualisation)
        {
            visualRepresentation.SetActive(true);
        }
        else
        {
            visualRepresentation.SetActive(false);
        }
    }*/

    public void AddRecoil(ref RecoilInfo recoilInfo)
    {
        currentUpVelocity += recoilInfo.recoilUpShootForce;
        currentBackVelocity += recoilInfo.recoilBackShootForce;

        // -----Add side velocity-----
        float sideVelocityToApply = 0;

        if (currentRotationUp > 0)
        {
            sideVelocityToApply = recoilInfo.recoilSideShootForce * (currentRotationUp / maxRotationUp);
        }

        if (Random.value > 0.5f)
        {
            currentSideVelocity += sideVelocityToApply;
        }
        else
        {
            currentSideVelocity -= sideVelocityToApply;
        }


        // -------Set the other values----------
        // Up
        maxRecoilUpSpeed = recoilInfo.maxRecoilUpSpeed;
        maxReduceRecoilUpAcceleration = recoilInfo.maxReduceRecoilUpAcceleration;
        maxReduceRecoilUpSpeed = recoilInfo.maxReduceRecoilUpSpeed;
        maxRotationUp = recoilInfo.maxRotationUp;

        // Side
        maxRecoilSideSpeed = recoilInfo.maxRecoilSideSpeed;
        maxReduceRecoilSideAcceleration = recoilInfo.maxReduceRecoilSideAcceleration;
        maxReduceRecoilSideSpeed = recoilInfo.maxReduceRecoilSideSpeed;
        maxRotationSide = recoilInfo.maxRotationSide;

        //Back
        maxRecoilBackSpeed = recoilInfo.maxRecoilBackSpeed;
        maxReduceRecoilBackAcceleration = recoilInfo.maxReduceRecoilBackAcceleration;
        maxReduceRecoilBackSpeed = recoilInfo.maxReduceRecoilBackSpeed;
        maxPositionBack = recoilInfo.maxPositionBack;


        #region Debug Time calculation
        recoilForceCounter += recoilInfo.recoilUpShootForce;
        /*Debug.Log("[-------------------------------------Add recoil--------------------------------------]");
        Debug.Log("Added shhot force: " + recoilInfo.recoilBackShootForce);
        Debug.Log("vel: " + currentBackVelocity);
        Debug.Log("time since last Recoil: " + (Time.time - lastRecoilTime));*/
        lastRecoilTime = Time.time;
        // Debug time end
        #endregion
    }

    private void Update()
    {
        //apply the angle settings
        if (usedByPlayer)
        {
            //transform.localRotation = Quaternion.Euler(GameSettings.Instance.GetAimCorrectionAngle(), 0, 0);
        }
        else
        {
            //transform.localRotation = Quaternion.Euler(GameSettings.Instance.GetAimCorrectionAngle(), 0, 0);
        }
       
        
        #region Debug Time calculation
        if (Time.time > nextCountTime)
        {
           // Debug.Log("{-------------------------------------values per minute ----------------------}");
            //Debug.Log("recoil per minute: " + recoilForceCounter);
            //Debug.Log("counter recoil per minute: " + counterRecoilForceCounter);

            recoilForceCounter = 0;
            counterRecoilForceCounter = 0;

            nextCountTime = Time.time + 1;
        }
        // Debug time end
        #endregion

        #region Calculate recoil reduction Up

        // Check if we should reduce recoil:
        if (currentRotationUp > 0.01f)
        {
            reduceRecoilUp = true;
        }
        else
        {
            reduceRecoilUp = false;
        }

        // Calculate counter velocity to apply:
        if (reduceRecoilUp)
        {
            brakeUp = false;

            if (currentUpVelocity < 0)
            {
                remainingUpDistance = currentRotationUp;
                currentUpBreakDistance = currentUpVelocity * currentUpVelocity / (2 * maxReduceRecoilUpAcceleration);

                if (remainingUpDistance < currentUpBreakDistance)
                {
                    brakeUp = true;
                }
            }

            if (brakeUp)
            {
                currentReduceRecoilUpVelocity = maxReduceRecoilUpAcceleration * Time.deltaTime;
            }
            else
            {
                currentReduceRecoilUpVelocity = -maxReduceRecoilUpAcceleration * Time.deltaTime;
            }

            counterRecoilForceCounter += currentReduceRecoilUpVelocity;
            currentUpVelocity += currentReduceRecoilUpVelocity;  
        }


        // Cap Velocity Up:
        if (currentUpVelocity < 0)
        {
            if (currentUpVelocity < -maxReduceRecoilUpSpeed)
            {
                currentUpVelocity = -maxReduceRecoilUpSpeed;
            }
        }
        else
        {
            if(currentUpVelocity> maxRecoilUpSpeed)
            {
                currentUpVelocity = maxRecoilUpSpeed;
            }
        }

        currentRotationUp += currentUpVelocity * Time.deltaTime;

        // Cap Rotation Up:
        if (reduceRecoilUp)
        {
            if (currentRotationUp < 0.01f)
            {
                currentRotationUp = 0;
                currentUpVelocity = 0;
            }
            else if (currentRotationUp > maxRotationUp)
            {
                currentRotationUp = maxRotationUp;
                currentUpVelocity = 0;
            }
        }

        #endregion

        #region Calculate recoil reduction Side

        // Check if we should reduce recoil:
        if (currentRotationSide > 0.01f || currentRotationSide < -0.01f)
        {
            reduceRecoilSide = true;
        }
        else
        {
            reduceRecoilSide = false;
        }

        // Check if the current rotation is on the right side.
        bool isRight = false;

        if (currentRotationSide > 0)
        {
            isRight = true;
        }


        // Calculate counter velocity to apply.
        if (reduceRecoilSide)
        {
            // Check if we should start to brake.
            brakeSide = false;

            if (isRight && currentSideVelocity < 0 || !isRight && currentSideVelocity > 0)
            {
                remainingSideDistance = Mathf.Abs(currentRotationSide);
                currentSideBreakDistance = currentSideVelocity * currentSideVelocity / (2 * maxReduceRecoilSideAcceleration);

                if (remainingSideDistance < currentSideBreakDistance)
                {
                    brakeSide = true;
                }
            }

            // Calculate reduce recoil Velocity.
            if (isRight)
            {
                if (brakeSide)
                {
                    currentReduceRecoilSideVelocity = maxReduceRecoilSideAcceleration * Time.deltaTime;
                }
                else
                {
                    currentReduceRecoilSideVelocity = -maxReduceRecoilSideAcceleration * Time.deltaTime;
                }
            }
            else
            {
                if (brakeSide)
                {
                    currentReduceRecoilSideVelocity = -maxReduceRecoilSideAcceleration * Time.deltaTime;
                }
                else
                {
                    currentReduceRecoilSideVelocity = maxReduceRecoilSideAcceleration * Time.deltaTime;
                }
            }
        
            currentSideVelocity += currentReduceRecoilSideVelocity;
        }

        // Cap Side Velocity:
        if (isRight)
        {
            if(currentSideVelocity > maxRecoilSideSpeed)
            {
                currentSideVelocity = maxRecoilSideSpeed;
            }
            else if (currentSideVelocity < -maxReduceRecoilSideSpeed)
            {
                currentSideVelocity = -maxReduceRecoilSideSpeed;
            }
        }
        else
        {
            if (currentSideVelocity < -maxRecoilSideSpeed)
            {
                currentSideVelocity = -maxRecoilSideSpeed;
            }
            else if (currentSideVelocity > maxReduceRecoilSideSpeed)
            {
                currentSideVelocity = maxReduceRecoilSideSpeed;
            }
        }

        currentRotationSide += currentSideVelocity * Time.deltaTime;

        // Cap Side Rotation:
        if (reduceRecoilSide)
        {
            if (isRight)
            {
                if (currentRotationSide < 0.01f)
                {
                    currentRotationSide = 0;
                    currentSideVelocity = 0;
                }
                else if (currentRotationSide > maxRotationSide)
                {
                    currentRotationSide = maxRotationSide;
                    currentSideVelocity = 0;
                }
            }
            else
            {
                if (currentRotationSide > -0.01f)
                {
                    currentRotationSide = 0;
                    currentSideVelocity = 0;
                }
                else if (currentRotationSide < -maxRotationSide)
                {
                    currentRotationSide = -maxRotationSide;
                    currentSideVelocity = 0;
                }
            }
        }

        #endregion

        #region Calculate recoil reduction Back

        // Check if we should reduce recoil:
        if (currentPositionBack > 0.001f)
        {
            reduceRecoilBack = true;
        }
        else
        {
            reduceRecoilBack = false;
        }

        // Calculate counter velocity to apply:
        if (reduceRecoilBack)
        {
            brakeBack = false;

            if (currentBackVelocity < 0)
            {
                remainingBackDistance = currentPositionBack;
                currentBackBreakDistance = currentBackVelocity * currentBackVelocity / (2 * maxReduceRecoilBackAcceleration);

                if (remainingBackDistance < currentBackBreakDistance)
                {
                    brakeBack = true;
                }
            }

            if (brakeBack)
            {
                currentReduceRecoilBackVelocity = maxReduceRecoilBackAcceleration * Time.deltaTime;
            }
            else
            {
                currentReduceRecoilBackVelocity = -maxReduceRecoilBackAcceleration * Time.deltaTime;
            }

            currentBackVelocity += currentReduceRecoilBackVelocity;
        }

        // Cap Velocity Back
        if (currentBackVelocity < 0)
        {
            if (currentBackVelocity < -maxReduceRecoilBackSpeed)
            {
                currentBackVelocity = -maxReduceRecoilBackSpeed;
            }
        }
        else
        {
            if (currentBackVelocity > maxRecoilBackSpeed)
            {
                currentBackVelocity = maxRecoilBackSpeed;
            }
        }

        currentPositionBack += currentBackVelocity * Time.deltaTime;

        // Cap Back Position
        if (reduceRecoilBack)
        {
            if (currentPositionBack < 0.001f)
            {
                currentPositionBack = 0;
                currentBackVelocity = 0;
            }
            else if (currentPositionBack > maxPositionBack)
            {
                currentPositionBack = maxPositionBack;
                currentBackVelocity = 0;
            }
        }

        #endregion

        transformToApplyRecoilTo.localPosition = new Vector3(0, 0, -currentPositionBack);
        transformToApplyRecoilTo.localRotation = Quaternion.Euler(-currentRotationUp, currentRotationSide, 0);
    }

    public Vector3 GetPos()
    {
        return transformToApplyRecoilTo.position;
    }

    public Vector3 GetPosWithoutBackRecoil()
    {
        return transform.position;
    }

    public Vector3 GetUp()
    {
        return transformToApplyRecoilTo.up;
    }

    public Vector3 GetRight()
    {
        return transformToApplyRecoilTo.right;
    }

    public Vector3 GetForward()
    {
        return transformToApplyRecoilTo.forward;
    }

    public Quaternion GetRot()
    {
        return transformToApplyRecoilTo.rotation;
    }

    public Quaternion GetLocalRot()
    {
        return transformToApplyRecoilTo.localRotation;
    }

    public Vector3 TransformPoint(Vector3 point)
    {
        return transformToApplyRecoilTo.TransformPoint(point);
    }

    public Vector3 InverseTransformPoint(Vector3 point)
    {
        return transformToApplyRecoilTo.InverseTransformPoint(point);
    }

    public float GetCurrentUpRecoil()
    {
        return -currentRotationUp;
    }

    public float GetCurrentSideRecoil()
    {
        return currentRotationSide; 
    }

    public float GetCurrentBackRecoil()
    {
        return currentPositionBack;
    }
}
