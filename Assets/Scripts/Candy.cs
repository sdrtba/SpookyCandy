using UnityEngine;

public class Candy : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float maxTorque;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)) * force, ForceMode2D.Impulse);
        _rb.AddTorque(Random.Range(0, maxTorque), ForceMode2D.Impulse);
    }
}
