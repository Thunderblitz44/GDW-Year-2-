using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputExpander
{
    public void SetupInputEvents(object sender, ActionMap actions);
}