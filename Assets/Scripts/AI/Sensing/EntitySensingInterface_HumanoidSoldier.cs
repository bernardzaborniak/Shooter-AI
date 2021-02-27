using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class EntitySensingInterface_HumanoidSoldier : EntitySensingInterface
{
    [Header("States")]
    //public EntityActionBeingExecuted soldierPositioningState;
    public EntityActionBeingExecuted soldierCombatAndInteractionState;
    [Header("References to read to determine states")]
    public EC_HumanoidCharacterController characterController;
    public AI_Controller_HumanoidSoldier
        //-> would it be better if the ai controller would write this info compiled here?

    protected override void SetUp()
    {
        base.SetUp();

        soldierCombatAndInteractionState = new EntityActionBeingExecuted();
    }

    public override EntityActionBeingExecuted[] GetActionsBeingExecuted()
    {
        //Soldier Positioing
        Idle,
        Sprinting,
        Walking,
        InCoverHiding,
        InCoverPeeking,
        //Soldier Combat & Interaction
        InteractionIdle,
        ChangingWeapon,
        ReloadingWeapon,
        ShootingAt,
        ThrowingGrenade

            //only fill the debug states for now;

        if (characterController.IsThrowingGrenade())
        {
            soldierCombatAndInteractionState.actionBeingExecuted = EntityActionBeingExecuted.EntityActionBeingExecutedType.ThrowingGrenade;
        }
        else if (characterController.IsReloadingWeapon())
        {
            soldierCombatAndInteractionState.actionBeingExecuted = EntityActionBeingExecuted.EntityActionBeingExecutedType.ReloadingWeapon;
        }
        else if () //check if the current selected desision is shooting at - this isnt dynamic enough?
        {

        }
        else
        {
            soldierCombatAndInteractionState.actionBeingExecuted = EntityActionBeingExecuted.EntityActionBeingExecutedType.InteractionIdle;
        }


        return null;
    }
}*/
