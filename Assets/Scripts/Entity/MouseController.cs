using UnityEngine;

public class MouseController : MonoBehaviour
{
    public static MouseController Instance;

    public GameObject modifierDrop;
    
    ModifierDrop currentDrop;
    StatModifier selectedModifier;

    Camera cam;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        cam = Camera.main;
    }

    void Update() {
        if (selectedModifier != null) {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = Mathf.Abs(cam.transform.position.z); // usually -10

            Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
            worldPos.z = 0; // force to the gameplay plane

            currentDrop.transform.position = worldPos;

            if (Input.GetMouseButtonDown(0)) {
                currentDrop.Dropped();
                selectedModifier = null;
            }
        }
    }

    public void SetModifier(StatModifier modifier) {
        selectedModifier = modifier;

        currentDrop = Instantiate(modifierDrop).GetComponent<ModifierDrop>();
        currentDrop.Init(selectedModifier);
    }
}
