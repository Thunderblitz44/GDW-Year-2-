using System;
using TMPro;
using UnityEngine;

public class BossHealthComponent : HealthComponent
{
    [SerializeField] Animator animator;
    [SerializeField] float[] phases = new float[4] { 1f, 0.75f, 0.5f, 0.25f};
    int phase = 0;
    public Action nextPhase;

    public void Show()
    {
        hpbar.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = LevelManager.Instance.Boss.name;
        hpBarPrefab.SetActive(true);
    }

    public void Hide()
    {
        hpBarPrefab.SetActive(false);
        animator.SetTrigger("BossDead");
    }

    public override void DeductHealth(int value)
    {
        base.DeductHealth(value);

        for (int i = 0; i < phases.Length; i++)
        {
            if (health > maxHealth * phases[i]) break;

            // below the threshold
            if (phase < i)
            {
                phase = i;
                nextPhase?.Invoke();
                break;
            }
        }
    }

}
