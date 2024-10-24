using UnityEngine;

public class Basket : MonoBehaviour
{
    private float _xPos;
    private Rigidbody2D _rb;
    private float _border = 8f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    private void Update()
    {
        _xPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        if (_xPos < -_border ) _xPos = -_border;
        else if (_xPos > _border ) _xPos = _border;

        transform.position = new Vector2(_xPos, transform.position.y);
    }
}
