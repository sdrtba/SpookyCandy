using UnityEngine;

public class Spawner : MonoBehaviour
{
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
        if (cooldown > 0.5f) cooldown -= Time.deltaTime / 500;

        _localCooldown -= Time.deltaTime;
        if (_localCooldown < 0 )
        {
            Instantiate(objects[Random.Range(0, objects.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)]);
            _localCooldown = cooldown;
        }
    }
}
