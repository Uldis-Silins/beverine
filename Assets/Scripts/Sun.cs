using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Microsystem microsystemPrefab;
    public float spawnMoveDistance = 10f;
    public float spawnMoveSpeed = 10f;
    public float microsystemSpawnDistance = 10f;

    private Vector3 m_startAnimationPosition;

    private List<Microsystem> m_spawnedMicrosystems;

    private const int MICROSYSTEM_COUNT = 8;

    public IReadOnlyList<Microsystem> SpawnedSystems { get { return m_spawnedMicrosystems; } }

    private void Awake()
    {
        m_spawnedMicrosystems = new List<Microsystem>();
    }

    public void SpawnMicrosystems()
    {
        for(int i = 0; i < MICROSYSTEM_COUNT; i++)
        {
            var radians = 2 * Mathf.PI / MICROSYSTEM_COUNT * i;
            var x = Mathf.Cos(radians);
            var y = Mathf.Sin(radians);
               
            var spawnDir = new Vector3(x, y, 0);
            var instance = Instantiate(microsystemPrefab, transform.position, Quaternion.identity);
            instance.Spawn(transform.position, transform.position + spawnDir * microsystemSpawnDistance, 1f);
            m_spawnedMicrosystems.Add(instance);
        }
        
    }

    public void StartSpawnAnimation(Vector3 cameraForward)
    {
        m_startAnimationPosition = transform.position;
        StartCoroutine(SpawnAnimation(cameraForward));
    }

    private IEnumerator SpawnAnimation(Vector3 cameraForward)
    {
        while(Vector3.Distance(transform.position, m_startAnimationPosition) < spawnMoveDistance)
        {
            transform.position = transform.position + cameraForward * spawnMoveSpeed * Time.deltaTime;
            yield return null;
        }

        SpawnMicrosystems();
    }
}
