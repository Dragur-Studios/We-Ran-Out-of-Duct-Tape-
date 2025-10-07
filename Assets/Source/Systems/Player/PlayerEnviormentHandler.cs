using UnityEngine;

public class PlayerEnviormentHandler : MonoBehaviour
{
    Player player;
    GameInputReciever inputs;


    [SerializeField] GameObject gasMask;
    [SerializeField] GameObject flashlight;

    bool enableGasMask = false;
    bool enableFlashlight = false;

    void Start()
    {
        player = GetComponent<Player>();
        inputs = player.Input;

        gasMask.SetActive(false); 
        flashlight.SetActive(false);

    }

    float inputChangeDelayTime = 0.5f;
    float delayTimer = 0;


    void Update()
    {
        if (inputs.ItemSlot01 && delayTimer <= 0)
        {
            delayTimer = inputChangeDelayTime;
            
            enableGasMask = !enableGasMask;
        }
        else if(inputs.ItemSlot02 && delayTimer <= 0)
        {
            delayTimer = inputChangeDelayTime;
            enableFlashlight = !enableFlashlight;
        }

        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }

        gasMask.SetActive(enableGasMask);
        flashlight.SetActive(enableFlashlight);
    }
}
