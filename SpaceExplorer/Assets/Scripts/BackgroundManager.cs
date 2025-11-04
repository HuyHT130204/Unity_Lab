using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField]
    private Sprite[] backgroundImages;

    [SerializeField]
    private Renderer Background;

    // Start is called before the first frame update
    void Start()
    {
        if (backgroundImages.Length == 0)
        {
            Debug.LogError("No background images assigned in the inspector.");
            return;
        }

        // Set the selected background image to the Background GameObject
        Background.material.mainTexture = backgroundImages[
            Random.Range(0, backgroundImages.Length)
        ].texture;
    }

    // Update is called once per frame
    void Update()
    {
        Background.material.mainTextureOffset += new Vector2(0, 0.2f * Time.deltaTime);
    }
}
