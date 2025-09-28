using UnityEngine;
using UnityEngine.UIElements;

public class PhoneBatteryManager : MonoBehaviour
{
    [SerializeField] UIDocument doc;

    VisualElement root;
    VisualElement phoneBatteryBar;
    VisualElement chargeIcon;
    
    [SerializeField] Sprite chargingSprite;
    [SerializeField] Sprite batteryLowSprite;

    float percent; // 0-1 0->100.
    
    [SerializeField] Color nuffColor;
    [SerializeField] Color littleColor;
    [SerializeField] Color ohShitColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = doc.rootVisualElement;
        phoneBatteryBar = root.Q("PhoneBatteryBar");
        chargeIcon = root.Q("ChargeIcon");
       

    }
    float resetTimer = 2.0f;
    public bool isCharging = false;

    // Update is called once per frame
    void Update()
    {

        Color color = Color.white;
        Sprite batterySprite = null;

        if (isCharging)
        {
            batterySprite = chargingSprite;
        }
        else
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer < 0)
            {
                resetTimer = 2.0f;

                percent = Random.Range(0, 101);
            }
        }

        if(percent > 50)
        {
            color = nuffColor;
            batterySprite = null;
        }
        else if(percent < 30)
        {
            color = ohShitColor;
            batterySprite = batteryLowSprite;
        }
        else
        {
            color = littleColor;
        }


        phoneBatteryBar.style.height = new Length { unit = LengthUnit.Percent, value = percent };
        phoneBatteryBar.style.backgroundColor = color;
        var img = new StyleBackground();
        img.value = new Background { sprite = batterySprite };

        chargeIcon.style.backgroundImage = img;
        
    }
}
