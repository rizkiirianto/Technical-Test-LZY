using System.Collections;
using UnityEngine;

namespace FruitNinja
{
    public class FruitSpawner : MonoBehaviour
    {
        public GameObject[] fruitPrefabs;
        public float spawnDelay = 1f;

        private void Start()
        {
            StartCoroutine(SpawnFruits());
        }

        private IEnumerator SpawnFruits()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);

                if (fruitPrefabs.Length == 0) continue;

                // Calculate half of the spawner's X scale to know the spawn limits
                float halfWidth = transform.localScale.x / 2f;
                float randomX = Random.Range(-halfWidth, halfWidth);

                Vector3 spawnPosition = transform.position + new Vector3(randomX, 0f, 0f);

                GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
                GameObject spawnedFruit = Instantiate(prefab, spawnPosition, Quaternion.identity);

                Rigidbody rb = spawnedFruit.GetComponent<Rigidbody>();
                if (rb == null) rb = spawnedFruit.GetComponentInChildren<Rigidbody>();

                if (rb != null)
                {
                    // No upward force, just let gravity pull it down!
                    // But we can add a little bit of random rotation (torque) so it spins beautifully as it falls
                    rb.AddTorque(Random.insideUnitSphere * Random.Range(2f, 5f), ForceMode.Impulse);
                }
            }
        }
    }
}
