using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuEvents : MonoBehaviour
{
    [SerializeField] UIDocument doc;

    void Start()
    {
        doc.rootVisualElement.visible = false;

        GameManager.Instance.OnPause += (isPaused) =>
        {
            doc.rootVisualElement.visible = isPaused;
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
