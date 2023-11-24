using Unity.Netcode;
using UnityEngine;

abstract public class Ability : NetworkBehaviour
{
    public abstract void Part1();
    public abstract void Part2();
}

public class PlayerAbilities : NetworkBehaviour, IInputExpander
{
    [SerializeField] Ability[] abilities = {null, null, null, null};
    
    Player playerScript;
    ActionMap actions;

    public void SetupInputEvents(object sender, ActionMap actions)
    {
        playerScript = (Player)sender;
        this.actions = actions;

        


        // Abilities
        actions.Abilities.First.started += ctx => abilities[0]?.Part1();
        actions.Abilities.First.canceled += ctx => abilities[0]?.Part2();

        actions.Abilities.Second.started += ctx => abilities[1]?.Part1();
        actions.Abilities.Second.canceled += ctx => abilities[1]?.Part2();

        actions.Abilities.Third.started += ctx => abilities[2]?.Part1();
        actions.Abilities.Third.canceled += ctx => abilities[2]?.Part2();

        actions.Abilities.Fourth.started += ctx => abilities[3]?.Part1();
        actions.Abilities.Fourth.canceled += ctx => abilities[3]?.Part2();

        // for testing
        actions.General.DamageSelf.performed += ctx => 
        {
            if (IsOwner) GetComponent<IDamageable>().ApplyDamage(1f, DamageTypes.physical); 
        };
        actions.General.HealSelf.performed += ctx =>
        {
            if (IsOwner) GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.physical);
        };

        // For testing
        actions.General.Attack.performed += ctx =>
        {
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(1f , DamageTypes.physical);
        };
        actions.CameraControl.Aim.performed += ctx =>
        { 
            GameObject.Find("TestDummy").GetComponent<IDamageable>().ApplyDamage(-1f, DamageTypes.magic);
        };

        actions.General.Attack.Enable();
        actions.General.DamageSelf.Enable();
        actions.General.HealSelf.Enable();

        EnableAllAbilities();
    }


    public void EnableAllAbilities() => actions.Abilities.Enable();
    public void DisableAllAbilities() => actions.Abilities.Disable();
}