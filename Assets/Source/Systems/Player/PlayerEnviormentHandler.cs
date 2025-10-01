using UnityEngine;

public class PlayerEnviormentHandler : MonoBehaviour
{
    Player player;
    PlayerInputReciever inputs;


    [SerializeField] GameObject gasMask;

    bool enableGasMask = false;

    void Start()
    {
        player = GetComponent<Player>();
        inputs = player.Input;

        gasMask.SetActive(false);    
    }

    float gasMaskCheckDelayTime = 0.5f;
    float delayTimer = 0;


    void Update()
    {
        if (inputs.ItemSlot01 && delayTimer <= 0)
        {
            delayTimer = gasMaskCheckDelayTime;
            
            enableGasMask = !enableGasMask;
        }

        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }

        gasMask.SetActive(enableGasMask);
    }
}
