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
    [SerializeField] private int value;
    [SerializeField] private CandyType type;
    [SerializeField] private float force;
    [SerializeField] private float maxTorque;
    private float _collisionTime = 0.02f;
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

    /*private void OnTriggerEnter2D(Collider2D collision)
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
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_collisionTime > 0)
        {
            _collisionTime -= Time.deltaTime;
            return;
        }

        if (collision.tag == "Basket")
        {
            OnPickup?.Invoke(value);
            switch (type)
            {
                case CandyType.Bomb:
                    SoundManager.instance.PlayAudioClip(boomClip, transform, boomClipVolume);
                    OnHPChange?.Invoke(-1);
                    break;
                case CandyType.Health:
                    OnHPChange?.Invoke(1);
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
