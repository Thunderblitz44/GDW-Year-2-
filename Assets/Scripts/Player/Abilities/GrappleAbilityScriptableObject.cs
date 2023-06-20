using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DashAbility", menuName ="Abilities/Dash")]
public class GrappleAbilityScriptableObject : ScriptableObject, IAbility
{
    public void UseAbility(object caller)
    {
        // ray cast
        // hit?
        // check if is either:
        // enemy, movableObject or immovableObject
    }
}
