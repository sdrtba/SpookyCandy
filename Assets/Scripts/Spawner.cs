using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public event Action<int> OnMiss;

    [SerializeField] private GameObject[] objects;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float cooldown;
    private float _localCooldown;

    private void Start()
    {
        _localCooldown = cooldown;
    }

    private void Update()
    {
        _localCooldown -= Time.deltaTime;
        if (_localCooldown < 0 )
        {
            Instantiate(objects[UnityEngine.Random.Range(0, objects.Length)], spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)]);
            _localCooldown = cooldown;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        OnMiss?.Invoke(-1);
    }
}
