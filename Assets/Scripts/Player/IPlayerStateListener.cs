using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerStateListener
{
    public void SetCombatState();
    public void SetFreeLookState();
}
