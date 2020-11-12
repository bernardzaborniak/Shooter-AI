using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public GameEntity entityAttachedTo;
    public AIComponent[] aIComponents;

    public EC_HumanoidCharacterController characterController;
    public AIC_HumanSensing sensing;
    public AIC_AimingController aimingController;

    public float maxRangeToEnemy;
    public float minRangeToEnemy;
    public float desiredRangeToEnemy;

    //For MY bad ai so far
    bool changedToPistol;
    bool grenadeThrown;
    float nextCheckGrenadeTime;
    float grenadeThrowInterval = 1f;

    //public Transform targetPosition;
    public float targetMaxOffset;
    Vector3 finalMoveDestination;

    GameEntity nearestEnemyLastFrame;


    //public float throwGrenadeVelocity;
    //only basic AI for now
    enum WeaponState
    {
        FiringSMG,
        FiringPistol,
        ThrowingGrenade
    }
    WeaponState aIState;

    enum PositioningState
    {
        OpenField,
        MovingIntoCover,
        InCoverHiding,
        InCoverShooting
    }
    PositioningState positioningState;

    CoverPost targetCoverPost;
    CoverPost usedCoverPost;

    bool crouching;
    float crouchingPropability = 0.3f;

    public Transform targetPositionVisualised;

    float switchingBetweenCoverHidingAndShootingIntervalMin = 1;
    float switchingBetweenCoverHidingAndShootingIntervalMax = 4;
    float nextChangeCoverStanceTime;

    

    void Start()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].SetUpComponent(entityAttachedTo);
        }

        aIState = WeaponState.FiringSMG;

       


        targetPositionVisualised.SetParent(null);
    }

    public void SetFinalTargetPosition(Vector3 targetPosition)
    {
        finalMoveDestination = targetPosition + new Vector3(UnityEngine.Random.Range(-targetMaxOffset, targetMaxOffset), 0, UnityEngine.Random.Range(-targetMaxOffset, targetMaxOffset));

        //Sammple position on navmesh to prevent ai standing around cause of missing path
        NavMeshHit hit;
        if (NavMesh.SamplePosition(finalMoveDestination, out hit, 10.0f, NavMesh.AllAreas))
        {
            finalMoveDestination = hit.position;
        }
        else
        {
            //finalMoveDestination = targetPosition.position;
            finalMoveDestination = targetPosition;
        }

    }

    void Update()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].UpdateComponent();
        }




        #region AI State update

        #region Set needed Variables

        GameEntity nearestEnemy = sensing.nearestEnemy;

        Vector3 directionToNearestEnemy = Vector3.zero;
        float distanceToNearestEnemy = 0;
        if (nearestEnemy)
        {
            directionToNearestEnemy = (nearestEnemy.transform.position - transform.position);
            distanceToNearestEnemy = directionToNearestEnemy.magnitude;
        }

        //----------Set selectedGun & selectedGrenade-----------------
        Gun selectedGun = null;
        Grenade selectedGrenade = null;
        if ((characterController.GetCurrentlySelectedItem() is Gun))
        {
            selectedGun = (characterController.GetCurrentlySelectedItem() as Gun);
        }
        else if ((characterController.GetCurrentlySelectedItem() is Grenade))
        {
            selectedGrenade = (characterController.GetCurrentlySelectedItem() as Grenade);
        }

        //-----------Set enemyMovementSpeed----------------
        Vector3 enemyMovementSpeed = Vector3.zero;
        if (nearestEnemy)
        {
            IMoveable movement = nearestEnemy.GetComponent<IMoveable>();
            if (movement != null)
            {
                enemyMovementSpeed = movement.GetCurrentVelocity();
            }
        }

        //cover
        HashSet<Tuple<Post,float>> possiblePosts = sensing.postsInSensingRadius;


        #endregion

        if (nearestEnemy)
        {


            #region Positioning 
            if (positioningState == PositioningState.OpenField)
            {
                if (nearestEnemyLastFrame)
                {
                    characterController.StopMoving();
                }

                //Go At The Desired Distance
                if (distanceToNearestEnemy < minRangeToEnemy || distanceToNearestEnemy > maxRangeToEnemy)
                {
                    characterController.MoveTo(nearestEnemy.transform.position + -directionToNearestEnemy.normalized * desiredRangeToEnemy);
                    targetPositionVisualised.position = nearestEnemy.transform.position + -directionToNearestEnemy.normalized * desiredRangeToEnemy;
                }

                if (crouching)
                {
                    characterController.ChangeCharacterStanceToCrouchingStance();
                }
                else
                {
                    characterController.ChangeCharacterStanceToCombatStance();
                }


                //discard if the direction isnt good enough, find the closest one of the remaining
                //HashSet<Tuple<Post, float>> postsDiscardedBecauseOfDirection;

                if (possiblePosts.Count > 0)
                {
                    Post closestPost = null;
                    float closestDistance = Mathf.Infinity;
                    Vector3 myPos = transform.position;

                    foreach (Tuple<Post,float> postTuple in possiblePosts)
                    {
                        //check angle
                        Vector3 directionFromCoverToEnemy = nearestEnemy.transform.position - postTuple.Item1.GetPostPosition();
                        float angle = Vector3.Angle(directionFromCoverToEnemy, postTuple.Item1.transform.forward);

                        if (angle < 80)
                        {
                            if (postTuple.Item2 < closestDistance)
                            {
                                closestDistance = postTuple.Item2;
                                closestPost = postTuple.Item1;
                            }
                        }
                    }

                    if (closestPost)
                    {
                        positioningState = PositioningState.MovingIntoCover;
                        targetCoverPost = closestPost as CoverPost;
                        characterController.MoveTo(targetCoverPost.GetPostPosition());
                        targetPositionVisualised.position = targetCoverPost.GetPostPosition();
                    }
                }
            }
            else if (positioningState == PositioningState.MovingIntoCover)
            {
                if (crouching)
                {
                    characterController.ChangeCharacterStanceToCrouchingStance();
                }
                else
                {
                    characterController.ChangeCharacterStanceToCombatStance();
                }

                if (targetCoverPost.used)
                {
                    targetCoverPost = null;
                    positioningState = PositioningState.OpenField;
                    characterController.StopMoving();
                }
                else
                {
                    if (characterController.GetRemainingDistanceToCurrentMovementTarget() < 0.5f)
                    {
                        positioningState = PositioningState.InCoverHiding;
                        nextChangeCoverStanceTime = Time.time + UnityEngine.Random.Range(switchingBetweenCoverHidingAndShootingIntervalMin, switchingBetweenCoverHidingAndShootingIntervalMax);

                        //code for entering cover
                        EnterCoverPost(targetCoverPost as CoverPost);
                    }
                    if (!characterController.IsMoving())
                    {
                        characterController.MoveTo(targetCoverPost.GetPostPosition());
                        targetPositionVisualised.position = targetCoverPost.GetPostPosition();
                    }
                }

               
            }
            else if (positioningState == PositioningState.InCoverHiding)
            {
                if(Time.time> nextChangeCoverStanceTime)
                {
                    positioningState = PositioningState.InCoverShooting;
                    nextChangeCoverStanceTime = Time.time + UnityEngine.Random.Range(switchingBetweenCoverHidingAndShootingIntervalMin, switchingBetweenCoverHidingAndShootingIntervalMax);
                    characterController.ChangeCharacterStanceToCombatStance();
                    Debug.Log("usedCoverPost: " + usedCoverPost);
                    int randomNumber = UnityEngine.Random.Range(0, usedCoverPost.PeekPositions.Length);
                    Debug.Log("Random number: " + usedCoverPost.PeekPositions[randomNumber]);
                    Debug.Log("usedCoverPost.PeekPositions[UnityEngine.Random.Range(0, usedCoverPost.PeekPositions.Length)]: " + usedCoverPost.PeekPositions[randomNumber]);
                    Debug.Log("usedCoverPost.PeekPositions[UnityEngine.Random.Range(0, usedCoverPost.PeekPositions.Length)].transform.position: " + usedCoverPost.PeekPositions[randomNumber].transform.position);
                    characterController.MoveTo(usedCoverPost.PeekPositions[randomNumber].transform.position);
                }
                else
                {
                    if ((usedCoverPost as CoverPost).stanceType == 0)
                    {
                        characterController.ChangeCharacterStanceToCombatStance();
                    }
                    else if ((usedCoverPost as CoverPost).stanceType == 1)
                    {
                        characterController.ChangeCharacterStanceToCrouchingStance();
                    }

                    //check if the current cover isnt as good as it was anymore
                    if (Vector3.Angle(directionToNearestEnemy, usedCoverPost.transform.forward) > 80)
                    {
                        ExitCoverPost();
                        positioningState = PositioningState.OpenField;
                    }
                }


               

            }
            else if (positioningState == PositioningState.InCoverShooting)
            {
                if (Time.time > nextChangeCoverStanceTime)
                {
                    positioningState = PositioningState.InCoverHiding;
                    nextChangeCoverStanceTime = Time.time + UnityEngine.Random.Range(switchingBetweenCoverHidingAndShootingIntervalMin, switchingBetweenCoverHidingAndShootingIntervalMax);
                    characterController.MoveTo(targetCoverPost.GetPostPosition());
                }
                else
                {
                    if ((usedCoverPost as CoverPost).stanceType == 0)
                    {
                        characterController.ChangeCharacterStanceToCombatStance();
                    }
                    else if ((usedCoverPost as CoverPost).stanceType == 1)
                    {
                        characterController.ChangeCharacterStanceToCrouchingStance();
                    }

                    //check if the current cover isnt as good as it was anymore
                    if (Vector3.Angle(directionToNearestEnemy, usedCoverPost.transform.forward) > 80)
                    {
                        ExitCoverPost();
                        positioningState = PositioningState.OpenField;
                    }
                }


             

            }

            #endregion

            #region Weapon Change

            if (aIState == WeaponState.FiringSMG)
            {
               
                characterController.ChangeSelectedItem(1);
                
                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(1) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    if(positioningState == PositioningState.InCoverHiding)
                    {
                        characterController.StopAimingSpine();
                        characterController.StopAimingWeapon();
                    }
                    else
                    {
                        characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
                        characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                    }

                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " SMG");
                    }

                    if (characterController.GetAmmoRemainingInMagazine() > 0)
                    {

                        if (characterController.GetCurrentWeaponAimingErrorAngle(false) < 10)
                        {
                            if(positioningState != PositioningState.InCoverHiding)
                            {
                                characterController.ShootWeapon();
                            }
                        }
                        
                    }


                    if (Time.time > nextCheckGrenadeTime)
                    {
                        nextCheckGrenadeTime = Time.time + grenadeThrowInterval;
                        if (UnityEngine.Random.Range(0f, 1f) < 0.05f)
                        {
                            if (!grenadeThrown)
                            {
                                aIState = WeaponState.ThrowingGrenade;
                                if (UnityEngine.Random.Range(0f, 1f) < crouchingPropability)
                                {
                                    crouching = true;
                                }
                                else
                                {
                                    crouching = false;
                                }
                            }
                        }
                    }



                    if (!(characterController.GetAmmoRemainingInMagazine() > 0))
                    {
                        if (!changedToPistol)
                        {
                            if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                            {
                                characterController.StartReloadingWeapon();
                            }
                            else
                            {
                                changedToPistol = true;
                                aIState = WeaponState.FiringPistol;
                                if (UnityEngine.Random.Range(0f, 1f) < crouchingPropability)
                                {
                                    crouching = true;
                                }
                                else
                                {
                                    crouching = false;
                                }
                            }
                        }
                        else
                        {
                            characterController.StartReloadingWeapon();
                        }


                    }
                }



            }
            else if (aIState == WeaponState.FiringPistol)
            {

                characterController.ChangeSelectedItem(2);
               


                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(2) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {
                    if (positioningState == PositioningState.InCoverHiding)
                    {
                        characterController.StopAimingSpine();
                        characterController.StopAimingWeapon();
                    }
                    else
                    {
                        characterController.AimSpineAtPosition(nearestEnemy.GetAimPosition());
                        characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                    }



                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.GetAimPosition(), enemyMovementSpeed, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " Psitol");
                    }

                    if (characterController.GetAmmoRemainingInMagazine() > 0)
                    {
                        if (characterController.GetCurrentWeaponAimingErrorAngle(false) < 10)
                        {
                            if (positioningState != PositioningState.InCoverHiding)
                            {
                                characterController.ShootWeapon();
                            }
                        }
                    }



                    if (!(characterController.GetAmmoRemainingInMagazine() > 0))
                    {
                        aIState = WeaponState.FiringSMG;
                        if (UnityEngine.Random.Range(0f, 1f) < crouchingPropability)
                        {
                            crouching = true;
                        }
                        else
                        {
                            crouching = false;
                        }
                    }
                }


            }
            else if (aIState == WeaponState.ThrowingGrenade)
            {
                characterController.ChangeSelectedItem(3);
               

                if (characterController.GetItemInInventory(3) == null)
                {
                    grenadeThrown = false;
                    aIState = WeaponState.FiringSMG;
                    if (UnityEngine.Random.Range(0f, 1f) < crouchingPropability)
                    {
                        crouching = true;
                    }
                    else
                    {
                        crouching = false;
                    }
                    return;
                }

                if (characterController.IsCrouched())
                {
                    characterController.StopMoving();
                }
                

                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(3) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                {

                    Grenade equippedGrenade = characterController.GetCurrentlySelectedItem() as Grenade;

                    // enemyMovementSpeed = Vector3.zero;
                    IMoveable movement = nearestEnemy.GetComponent<IMoveable>();

                    if (movement != null)
                    {
                        enemyMovementSpeed = movement.GetCurrentVelocity();
                    }

                    float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);//we add 2just to fix some errors - needs refactoring later


                    Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false); //dont use enemyMovementVelocityWithgrenade as it will lead to errors and suicidal AI :)

                    if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false).y))
                    {
                        Debug.Log("NAN: " + transform.parent.gameObject.name + " Throwing Grenade");
                        Debug.Log("-> velöpcoty: " + grenadeThrowingVelocity);
                    }
                    //Vector3 grenadeThrowingDirection = aimingController.AddAimErrorAndHandShakeToAimDirection(aimSpineDirection);
                    Vector3 grenadeThrowingDirection = aimSpineDirection;

                    characterController.AimSpineInDirection(aimSpineDirection);

                    if (characterController.GetCurrentSpineAimingErrorAngle() < 3)
                    {
                        characterController.ThrowGrenade(grenadeThrowingVelocity, grenadeThrowingDirection);
                    }
                }


            }

            #endregion 
        }
        else
        {
            if (positioningState == PositioningState.InCoverHiding || positioningState == PositioningState.InCoverShooting)
            {
                ExitCoverPost();
                positioningState = PositioningState.OpenField;
            }


            characterController.ChangeSelectedItem(1);
            characterController.ChangeCharacterStanceToIdle();
            characterController.StopAimingSpine();
            characterController.StopAimingWeapon();
            characterController.MoveTo(finalMoveDestination, true);
            //characterController.MoveTo(targetPosition.position + finalMoveDestination, true);
            //targetPositionVisualised.position = targetPosition.position + currentTargetOffset;
            targetPositionVisualised.position = finalMoveDestination;
        }

        nearestEnemyLastFrame = nearestEnemy;



        //usually shoot smg, if there is no ammo left in smg, theres a 50 % chance that we change to pistol instead of reloading, the same in pistol to smg
        //shot weapon

        #endregion


        /*GameEntity nearestEnemy = sensing.nearestEnemy;
        float distanceToNearestEnemy = 0;
        if (nearestEnemy)
        {
            distanceToNearestEnemy = (nearestEnemy.transform.position - transform.position).magnitude;
        }
       
        characterController.ChangeCharacterStanceToCombatStance();

        if (characterController.GetItemInInventory(3) != null)
        {
            characterController.ChangeSelectedItem(3);

            if (nearestEnemy)
            {
                if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(3))
                {
                    Grenade equippedGrenade = characterController.GetCurrentlySelectedItem() as Grenade;

                    Vector3 enemyMovementSpeed = Vector3.zero;
                    IMoveable movement = nearestEnemy.GetComponent<IMoveable>();

                    if (movement != null)
                    {
                        enemyMovementSpeed = movement.GetCurrentVelocity();
                    }

                    float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);//we add 2just to fix some errors - needs refactoring later


                    Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemy.transform.position, Vector3.zero, true, grenadeThrowingVelocity, false); //dont use enemyMovementVelocityWithgrenade as it will lead to errors and suicidal AI :)
                    //Vector3 grenadeThrowingDirection = aimingController.AddAimErrorAndHandShakeToAimDirection(aimSpineDirection);
                    Vector3 grenadeThrowingDirection = aimSpineDirection;

                    characterController.AimSpineInDirection(aimSpineDirection);

                    if (characterController.GetCurrentSpineAimingErrorAngle() < 5)
                    {
                        characterController.ThrowGrenade(grenadeThrowingVelocity, grenadeThrowingDirection);
                    }

                }
            }
            
        }*/



    }

    void EnterCoverPost(CoverPost post)
    {
        if (!post.used)
        {
            usedCoverPost = post;
            post.used = true;
            post.usingEntity = entityAttachedTo;
        }
    }

    void ExitCoverPost()
    {
       
        usedCoverPost.used = false;
        usedCoverPost.usingEntity = null;
        usedCoverPost = null;
    }

    public void OnDie()
    {
        if (positioningState == PositioningState.InCoverHiding || positioningState == PositioningState.InCoverShooting)
        {
            ExitCoverPost();
        }      
    }
}
