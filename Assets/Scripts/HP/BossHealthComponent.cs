using System;
using UnityEngine;

public class BossHealthComponent : HealthComponent
{
    [SerializeField] Animator animator;
    [SerializeField] float[] phases = new float[4] { 1f, 0.75f, 0.5f, 0.25f };
    int phase = 0;
    public Action nextPhase;

    public void Show()
    {
        hpBarPrefab.SetActive(true);
        // play anim
    }

    public void Hide()
    {
        hpBarPrefab.SetActive(false);
        // play anim
    }

    public override void DeductHealth(float value)
    {
        base.DeductHealth(value);

        for (int i = 0; i < phases.Length; i++)
        {
            if (health <= maxHealth * phases[i]) 
            { 
                if (phase != i)
                {
                    phase = i;
                    nextPhase?.Invoke();
                }
                break;
            }
        }
    }

}
