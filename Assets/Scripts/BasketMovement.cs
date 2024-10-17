using System;
using UnityEngine;

public class BasketMovement : MonoBehaviour
{
    public event Action OnActionEvent;

    private float xPos;

    private void Update()
    {
        xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        transform.position = new Vector2(xPos, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        OnActionEvent?.Invoke();
    }
}
