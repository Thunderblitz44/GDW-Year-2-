using UnityEngine;

public class SpiritOfViolence : MonoBehaviour, IBossCommands
{
    [SerializeField] bool bearIsFirst = true;
    [SerializeField] float nextPartDelay = 2f;
    BossHealthComponent hp;
    ElanaDoppleganger ed;
    Bear b;
    int deaths;

    private void Awake()
    {
        ed = GetComponentInChildren<ElanaDoppleganger>();
        b = GetComponentInChildren<Bear>();

        if (bearIsFirst)
        {
            ed.gameObject.SetActive(false);
        }
        else
        {
            b.gameObject.SetActive(false);
        }
    }

    public BossHealthComponent GetHPComponent()
    {
        if (b.gameObject.activeSelf) return b.GetHPComponent();
        else return ed.GetHPComponent();
    }

    public void Introduce()
    {
        GetHPComponent().Show();
        if (bearIsFirst)
        {
            b.Introduce();
            b.GetHPComponent().onHealthZeroed += OnDead;
        }
        else
        {
            ed.Introduce();
            ed.GetHPComponent().onHealthZeroed += OnDead;
        }
    }

    void OnDead()
    {
        deaths++;
        if (deaths < 2) Invoke(nameof(NextPart), nextPartDelay);
        else hp.Hide(); 
    }

    void NextPart()
    {
        if (bearIsFirst)
        {
            b.GetHPComponent().onHealthZeroed -= OnDead;
            ed.gameObject.SetActive(true);
            ed.GetHPComponent().onHealthZeroed += OnDead;
            ed.Introduce();
            hp = ed.GetHPComponent();
        }
        else
        {
            ed.GetHPComponent().onHealthZeroed -= OnDead;
            b.gameObject.SetActive(true);
            b.GetHPComponent().onHealthZeroed += OnDead;
            b.Introduce();
            hp = b.GetHPComponent();
        }
    }
    
}
