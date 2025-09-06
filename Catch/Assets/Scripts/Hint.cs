using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashing : MonoBehaviour
{
    private float existing_timer = 0f; // This is a counter that records how long did this hint existed.
    private float warningFlashingTime = 0.5f; // This variable controlls how frequently does the hint object flash. 
    private SpriteRenderer spriteRenderer; // This variable contains a reference to the hint game object's render
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get reference to the renderer of hint object.
    }

    void Update() // Depending on the time, choose to make the sprite visible or disappear.
    {
        existing_timer += Time.deltaTime; // Increment time counter.

        if ((int)(existing_timer / warningFlashingTime) % 2 == 0) // This if statement decides to turn the rendering on or off(if you think about it flashing is basically just turning visibility on and off)
        {
            // Set alpha to 0 to make the sprite completely invisible
            Color color = spriteRenderer.color;
            color.a = 0f;  
            spriteRenderer.color = color;
        }
        else
        {
            // Set alpha to 1 to make the sprite completely visible
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }
}
