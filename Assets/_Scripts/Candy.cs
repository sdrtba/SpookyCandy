using System;
using UnityEngine;

public enum CandyType
{
    Base,
    Bomb,
    Health
}

public class Candy : MonoBehaviour
{
    public static event Action<int> OnPickup;
    public static event Action<int> OnHPChange;

    [SerializeField] private AudioClip boomClip;
    [Range(0f,1f)][SerializeField] private float boomClipVolume;
    [SerializeField] private CandyType type;
    [SerializeField] private float force;
    [SerializeField] private float maxTorque;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2)) * force, ForceMode2D.Impulse);
        _rb.AddTorque(UnityEngine.Random.Range(0, maxTorque), ForceMode2D.Impulse);
    }

    private void Update()
    {
        force += Time.deltaTime / 500;
        if (force > 5) force = 5;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Basket")
        {
            switch (type)
            {
                case CandyType.Base:
                    OnPickup?.Invoke(1);
                    break;
                case CandyType.Bomb:
                    SoundManager.instance.PlayAudioClip(boomClip, transform, boomClipVolume);
                    OnHPChange?.Invoke(-1);
                    OnPickup?.Invoke(-1);
                    break;
                case CandyType.Health:
                    OnHPChange?.Invoke(1);
                    OnPickup?.Invoke(1);
                    break;
            }
            Destroy(gameObject);
        }
        else if (collision.tag == "Floor")
        {
            if (type != CandyType.Bomb) OnHPChange?.Invoke(-1);
            Destroy(gameObject);
        }
    }
}
