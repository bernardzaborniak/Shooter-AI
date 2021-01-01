using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BenitosAI
{
    public class AIControllerOld : EntityComponent
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

        SensedEntityInfo nearestEnemyInfoLastFrame;


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

        TacticalPoint targetTacticalPoint;
        TacticalPoint usedTacticalPoint;

        bool crouching;
        float crouchingPropability = 0.3f;

        public Transform targetPositionVisualised;

        float switchingBetweenCoverHidingAndShootingIntervalMin = 1;
        float switchingBetweenCoverHidingAndShootingIntervalMax = 4;
        float nextChangeCoverStanceTime;



        //void Start()
        public override void SetUpComponent(GameEntity entity)
        {
            for (int i = 0; i < aIComponents.Length; i++)
            {
                aIComponents[i].SetUpComponent(entityAttachedTo);
            }

            aIState = WeaponState.FiringSMG;




            targetPositionVisualised.SetParent(null);
        }


        //void Update()
        public override void UpdateComponent()
        {
            for (int i = 0; i < aIComponents.Length; i++)
            {
                aIComponents[i].UpdateComponent();
            }

            UnityEngine.Profiling.Profiler.BeginSample("AI Controller Profiling");



            #region AI State update

            #region Set needed Variables

            SensedEntityInfo nearestEnemyInfo = null;
            try
            {
                nearestEnemyInfo = sensing.sensingInfo.enemyInfos[0];
            }catch(Exception e) { }

            Vector3 directionToNearestEnemy = Vector3.zero;
            float distanceToNearestEnemy = 0;
            if (nearestEnemyInfo != null)
            {
                directionToNearestEnemy = (nearestEnemyInfo.GetEntityPosition() - transform.position);
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
            Vector3 enemyVelocity = Vector3.zero;
            if (nearestEnemyInfo != null)
            {
                enemyVelocity = nearestEnemyInfo.GetCurrentVelocity();
            }
            /*if (nearestEnemyInfo != null)
            {
                //VisibilityInfo //TODO
                IMoveable movement = nearestEnemyInfo.GetComponent<IMoveable>();
                if (movement != null)
                {
                    enemyMovementSpeed = movement.GetCurrentVelocity();
                }
            }*/

            //cover
            //HashSet<Tuple<TacticalPoint,float>> possiblePosts = sensing.postsInSensingRadius;

            //HashSet<SensedTacticalPointInfo> possiblePosts = sensing.sensingInfo.tPointsCoverInfos.;
            //Dictionary<int,SensedTacticalPointInfo>.ValueCollection possiblePosts = sensing.sensingInfo.tPointsCoverInfos;
            SensedTacticalPointInfo[] possiblePosts = sensing.sensingInfo.tPointCoverInfos;


            #endregion

            if (nearestEnemyInfo != null)
            {
                #region Positioning 
                if (positioningState == PositioningState.OpenField)
                {
                    if (nearestEnemyInfoLastFrame != null)
                    {
                        if(characterController.IsMoving()) characterController.StopMoving();
                    }

                    //Go At The Desired Distance
                    if (distanceToNearestEnemy < minRangeToEnemy || distanceToNearestEnemy > maxRangeToEnemy)
                    {
                        characterController.MoveTo(nearestEnemyInfo.GetEntityPosition() + -directionToNearestEnemy.normalized * desiredRangeToEnemy);
                        targetPositionVisualised.position = nearestEnemyInfo.GetEntityPosition() + -directionToNearestEnemy.normalized * desiredRangeToEnemy;
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

                    if (possiblePosts.Length > 0)
                    {
                        TacticalPoint closestTacticalPoint = null;
                        float closestDistance = Mathf.Infinity;
                        Vector3 myPos = transform.position;

                        //foreach (Tuple<TacticalPoint,float> postTuple in possiblePosts)
                        foreach (SensedTacticalPointInfo postInfo in possiblePosts)
                        {
                            //check angle
                            Vector3 directionFromCoverToEnemy = nearestEnemyInfo.GetEntityPosition() - postInfo.tacticalPoint.GetPostPosition();
                            float angle = Vector3.Angle(directionFromCoverToEnemy, postInfo.tacticalPoint.transform.forward); //using forward for now - this should be reworked to work based on current rating

                            if (angle < 80)
                            {
                                if (postInfo.lastDistanceMeasured < closestDistance)
                                {
                                    closestDistance = postInfo.lastDistanceMeasured;
                                    closestTacticalPoint = postInfo.tacticalPoint;
                                }
                            }
                        }

                        if (closestTacticalPoint)
                        {
                            positioningState = PositioningState.MovingIntoCover;
                            targetTacticalPoint = closestTacticalPoint;
                            characterController.MoveTo(targetTacticalPoint.GetPostPosition());
                            targetPositionVisualised.position = targetTacticalPoint.GetPostPosition();
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

                    if (targetTacticalPoint.IsPointFull())
                    {
                        targetTacticalPoint = null;
                        positioningState = PositioningState.OpenField;
                        if(characterController.IsMoving())  characterController.StopMoving();
                    }
                    else
                    {
                        if (characterController.GetRemainingDistanceToCurrentMovementTarget() < 0.5f)
                        {
                            positioningState = PositioningState.InCoverHiding;
                            nextChangeCoverStanceTime = Time.time + UnityEngine.Random.Range(switchingBetweenCoverHidingAndShootingIntervalMin, switchingBetweenCoverHidingAndShootingIntervalMax);

                            //code for entering cover
                            EnterTacticalPoint(targetTacticalPoint);
                        }
                        if (!characterController.IsMoving())
                        {
                            characterController.MoveTo(targetTacticalPoint.GetPostPosition());
                            targetPositionVisualised.position = targetTacticalPoint.GetPostPosition();
                        }
                    }


                }
                else if (positioningState == PositioningState.InCoverHiding)
                {
                    if (Time.time > nextChangeCoverStanceTime)
                    {
                        positioningState = PositioningState.InCoverShooting;
                        nextChangeCoverStanceTime = Time.time + UnityEngine.Random.Range(switchingBetweenCoverHidingAndShootingIntervalMin, switchingBetweenCoverHidingAndShootingIntervalMax);
                        characterController.ChangeCharacterStanceToCombatStance();
                        Debug.Log("usedCoverPost: " + usedTacticalPoint);
                        int randomNumber = UnityEngine.Random.Range(0, usedTacticalPoint.coverShootPoints.Length);
                        Debug.Log("Random number: " + usedTacticalPoint.coverShootPoints[randomNumber]);
                        Debug.Log("usedCoverPost.PeekPositions[UnityEngine.Random.Range(0, usedCoverPost.PeekPositions.Length)]: " + usedTacticalPoint.coverShootPoints[randomNumber]);
                        Debug.Log("usedCoverPost.PeekPositions[UnityEngine.Random.Range(0, usedCoverPost.PeekPositions.Length)].transform.position: " + usedTacticalPoint.coverShootPoints[randomNumber].transform.position);
                        characterController.MoveTo(usedTacticalPoint.coverShootPoints[randomNumber].transform.position);
                    }
                    else
                    {
                        if ((usedTacticalPoint).stanceType == 0)
                        {
                            characterController.ChangeCharacterStanceToCombatStance();
                        }
                        else if ((usedTacticalPoint).stanceType == 1)
                        {
                            characterController.ChangeCharacterStanceToCrouchingStance();
                        }

                        //check if the current cover isnt as good as it was anymore
                        if (Vector3.Angle(directionToNearestEnemy, usedTacticalPoint.transform.forward) > 80)
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
                        characterController.MoveTo(targetTacticalPoint.GetPostPosition());
                    }
                    else
                    {
                        if ((usedTacticalPoint).stanceType == 0)
                        {
                            characterController.ChangeCharacterStanceToCombatStance();
                        }
                        else if ((usedTacticalPoint).stanceType == 1)
                        {
                            characterController.ChangeCharacterStanceToCrouchingStance();
                        }

                        //check if the current cover isnt as good as it was anymore
                        if (Vector3.Angle(directionToNearestEnemy, usedTacticalPoint.transform.forward) > 80)
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
                        if (positioningState == PositioningState.InCoverHiding)
                        {
                            characterController.StopAimingSpine();
                            characterController.StopAimingWeapon();
                        }
                        else
                        {
                            characterController.AimSpineAtPosition(nearestEnemyInfo.GetAimPosition());
                            characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetAimPosition(), enemyVelocity, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                        }

                        if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetAimPosition(), enemyVelocity, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
                        {
                            Debug.Log("NAN: " + transform.parent.gameObject.name + " SMG");
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
                            characterController.AimSpineAtPosition(nearestEnemyInfo.GetAimPosition());
                            characterController.AimWeaponInDirection(aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetAimPosition(), enemyVelocity, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true));

                        }



                        if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetAimPosition(), enemyVelocity, selectedGun.aimWithAngledShotCalculation, selectedGun.projectileLaunchVelocity, true).y))
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
                        if (characterController.IsMoving()) characterController.StopMoving();
                    }


                    if (characterController.GetCurrentlySelectedItem() == characterController.GetItemInInventory(3) && characterController.DoesCurrentItemInteractionStanceAllowAimingWeapon())
                    {

                        Grenade equippedGrenade = characterController.GetCurrentlySelectedItem() as Grenade;

                        // enemyMovementSpeed = Vector3.zero;
                        /*IMoveable movement = nearestEnemyInfo.GetComponent<IMoveable>();

                        if (movement != null)
                        {
                            enemyVelocity = movement.GetCurrentVelocity();
                        }*/

                        float grenadeThrowingVelocity = aimingController.DetermineThrowingObjectVelocity(equippedGrenade, distanceToNearestEnemy);//we add 2just to fix some errors - needs refactoring later


                        Vector3 aimSpineDirection = aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetEntityPosition(), Vector3.zero, true, grenadeThrowingVelocity, false); //dont use enemyMovementVelocityWithgrenade as it will lead to errors and suicidal AI :)

                        if (float.IsNaN(aimingController.GetDirectionToAimAtTarget(nearestEnemyInfo.GetEntityPosition(), Vector3.zero, true, grenadeThrowingVelocity, false).y))
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
                if (selectedGun != null)
                {
                    characterController.MoveTo(finalMoveDestination, true);
                }
                else
                {
                    characterController.MoveTo(finalMoveDestination, false);
                }
                //characterController.MoveTo(targetPosition.position + finalMoveDestination, true);
                //targetPositionVisualised.position = targetPosition.position + currentTargetOffset;
                targetPositionVisualised.position = finalMoveDestination;
            }

            nearestEnemyInfoLastFrame = nearestEnemyInfo;



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


            UnityEngine.Profiling.Profiler.EndSample();
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

        void EnterTacticalPoint(TacticalPoint point)
        {
            if (!point.IsPointFull())
            {
                point.OnEntityEntersPoint(entityAttachedTo);
                usedTacticalPoint = point;
            }
        }

        void ExitCoverPost()
        {
            if (usedTacticalPoint)
            {
                usedTacticalPoint.OnEntityExitsPoint(entityAttachedTo);
                usedTacticalPoint = null;
            }
        }

        public void OnDie()
        {
            if (positioningState == PositioningState.InCoverHiding || positioningState == PositioningState.InCoverShooting)
            {
                ExitCoverPost();
            }
        }
    }
}
