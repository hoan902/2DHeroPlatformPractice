using UnityEngine;

public class SpriteReflection : MonoBehaviour {
    
    public enum Axis { X, Y, Z }
    [Tooltip("Axis reflected around. (e.g. use Z for typical 2D, Y for top-down)")]
    public Axis reflectionAxis = Axis.Z;

    [Tooltip("Position of water plane along the chosen axis")]
    public float reflectionOffset = 0;

    [Tooltip("Layer to place reflections on")]
    public int reflectionLayer = 10;

    [Tooltip("Override the sprite used for reflections (Optional)")]
    public Sprite reflectionSprite;

    [Tooltip("Updates reflection position and SpriteRenderer params each frame. (Enable for non-static sprites, e.g. player)")]
    public bool updatePerFrame;

    private GameObject reflectionObj;
    private SpriteRenderer spriteRenderer, reflectionRenderer;
    private Vector3 axis;
    private Transform parent;
    private bool useParentAsPivot;

    void Start(){
        if (reflectionAxis == Axis.X) axis = new(0,1,1);
        else if (reflectionAxis == Axis.Y) axis = new(1,0,1);
        else if (reflectionAxis == Axis.Z) axis = new(1,1,0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        reflectionObj = new GameObject("Reflection") {
            layer = reflectionLayer
        };
        reflectionRenderer = reflectionObj.AddComponent<SpriteRenderer>();
        parent = transform.parent;
        useParentAsPivot = parent != null && parent.TryGetComponent(out SpriteReflection _);
        DoReflection();
    }

    void OnDestroy(){
        Destroy(reflectionObj);
    }

    void LateUpdate(){
        if (updatePerFrame) DoReflection();
    }

    void DoReflection(){
        Vector3 pivot = transform.position;
        if (useParentAsPivot){
            pivot = parent.transform.position;
        }
        pivot = new Vector3(
            axis.x * pivot.x,
            axis.y * pivot.y,
            axis.z * pivot.z
        );

        // Transform
        Vector3 pos = Quaternion.Euler(180, 0, 0) * (transform.position - pivot) + pivot;
        reflectionObj.transform.SetPositionAndRotation(
            pos, transform.rotation * Quaternion.Euler(180,0,0)
        );
        reflectionObj.transform.localScale = transform.localScale;

        // Sprite Data
        reflectionRenderer.sprite = reflectionSprite != null ? reflectionSprite : spriteRenderer.sprite;
        reflectionRenderer.flipX = spriteRenderer.flipX;
        reflectionRenderer.flipY = spriteRenderer.flipY;
        reflectionRenderer.color = spriteRenderer.color;
    }
}