using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnimation : MonoBehaviour
{
    public Sprite[] spriteSheet; // Array to hold the individual sprites from the sprite sheet
    public float framesPerSecond = 10f; // Number of frames per second for the animation

    private Image imageComponent;
    private int currentFrame;
    private float timer;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (spriteSheet.Length > 0)
        {
            currentFrame = 0;
            imageComponent.sprite = spriteSheet[currentFrame];
        }
    }

    void Update()
    {
        if (spriteSheet.Length > 0)
        {
            timer += Time.deltaTime;
            if (timer >= 1f / framesPerSecond)
            {
                timer -= 1f / framesPerSecond;
                currentFrame = (currentFrame + 1) % spriteSheet.Length;
                imageComponent.sprite = spriteSheet[currentFrame];
            }
        }
    }
}
