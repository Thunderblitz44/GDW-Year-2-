using UnityEngine;

public class Level2Puzzle : MonoBehaviour
{
    private Animator Animator;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public void GreenGemActivation()
    {
        //Player Completes Green Gem Encounter
        Animator.SetTrigger("Green Gem");
    }
    
    public void YellowGemActivation()
    {
        //Same thing with other colors
        Animator.SetTrigger("Yellow Gem");
    }
    public void BlueGemActivation()
    {
        Animator.SetTrigger("Blue Gem");
    }

    public void OpenSesame()
    {
        Animator.SetTrigger("Door");
    }
}