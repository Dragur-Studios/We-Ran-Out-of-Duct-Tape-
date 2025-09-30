using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHUDHandler : MonoBehaviour
{
    Player player;
    [SerializeField] UIDocument playerHUD;

    VisualElement hpBar;

    void Start()
    {
        player = GameManager.Instance.Player;


    }
    Length fillLength = new();
    void SetFill(float percent)
    {

        fillLength.unit = LengthUnit.Percent;
        fillLength.value = percent;

        if (hpBar == null)
        {   
            hpBar = playerHUD.rootVisualElement.Q("bar-fill");
        }

        hpBar.style.width = fillLength;
    }

    // Update is called once per frame
    void Update()
    {
        SetFill(player.HP);
    }
}
