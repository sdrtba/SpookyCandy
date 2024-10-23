using UnityEngine;

public class Basket : MonoBehaviour
{
    private float xPos;

    private void Update()
    {
        xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        transform.position = new Vector2(xPos, transform.position.y);
    }
}
