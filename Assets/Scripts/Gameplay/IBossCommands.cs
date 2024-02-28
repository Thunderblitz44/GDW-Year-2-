using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossCommands
{
    public void Introduce();
    public BossHealthComponent GetHPComponent();
}
