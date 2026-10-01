using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{
    PlayerControllerScript playerCTRL;

    // Hit Sphere Object
    public GameObject hitSphere;

    // Boolean
    public bool sphereToggle;

    private void Awake()
    {
        playerCTRL = GetComponentInParent<PlayerControllerScript>();
        sphereToggle = false;
    }

    private void Update()
    {
        if (sphereToggle) hitSphere.SetActive(true);
        else hitSphere.SetActive(false);
    }

    public void ResetAction()
    {
        playerCTRL.inAction = false;
    }

    public void ToggleSphere()
    {
        sphereToggle = !sphereToggle;
    }    
}
