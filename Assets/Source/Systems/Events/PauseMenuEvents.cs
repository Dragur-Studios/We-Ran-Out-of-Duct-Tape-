using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



public class PauseMenuEvents : MonoBehaviour
{
    [SerializeField] UIDocument doc;

    VisualElement[] content;

    enum Tab
    {
        System,
        Equipment,
        Inventory,
        Map,
        Player,
        Quests,
        __COUNT__
    }

    string TabToString(Tab tab)
    {
        switch (tab)
        {
            case Tab.System:
                return "System";
            case Tab.Equipment:
                return "Equipment";
            case Tab.Inventory:
                return "Inventory";
            case Tab.Map:
                return "Map";
            case Tab.Player:
                return "Player";
            case Tab.Quests:
                return "Quests";
        }
        return null;
    }

    void Start()
    {
        doc.rootVisualElement.visible = false;

        GameManager.Instance.OnPause += (isPaused) =>
        {
            doc.rootVisualElement.visible = isPaused;
        };
    
        var root = doc.rootVisualElement;


        content = new VisualElement[]
        {
            root.Q("SystemTabContent"),
            root.Q("EquipmentTabContent"),
            root.Q("InventoryTabContent"),
            root.Q("MapTabContent"),
            root.Q("PlayerTabContent"),
            root.Q("QuestsTabContent")
        };

        int count = (int)Tab.__COUNT__;
        for (int i = count - 1; i >= 0; i--)
        {
            int idx = i;
            var tab = TabToString((Tab)i);
            var btn = root.Q<Button>($"{tab}Tab_BTN");
            btn.clicked += () => { SetActive(idx); };
        }


        SetupSystemButtons();
        
    }

    void SetupSystemButtons()
    {
        var root = doc.rootVisualElement;
        var btn = root.Q<Button>("Resume_BTN");
        btn.clicked += TryResumeGame;

        btn = root.Q<Button>("QuitToMenu_BTN");
        btn.clicked += ExitToMenu;

    }

    void TryResumeGame()
    {
        GameManager.Unpause();
    }

    void ExitToMenu()
    {
        GameManager.Unpause();
        GameSceneLoader.LoadMainMenu();
    }

    void SetActive(int activeIndex)
    {
        foreach (var item in content)
        {
            item.style.display = DisplayStyle.None;
        }

        content[activeIndex].style.display = DisplayStyle.Flex;
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}
