using System.Collections;
using UnityEngine;

namespace FruitNinja
{
    public class FruitSpawner : MonoBehaviour
    {
        public GameObject[] fruitPrefabs;
        public GameObject[] bombPrefabs;
        public float bombChance = 0.3f;
        
        [Header("Progression System")]
        public float spawnDelay = 1.5f; // Initial delay
        public float minSpawnDelay = 0.15f; // Bisa sangat cepat (0.15 detik)
        public float delayDecreaseRate = 0.02f; // Jeda berkurang 0.02 detik setiap 1 detik waktu bermain

        private float timeElapsed = 0f;

        private void Start()
        {
            timeElapsed = 0f;
            StartCoroutine(SpawnFruits());
        }

        private void Update()
        {
            // Catat waktu bermain secara terus-menerus
            timeElapsed += Time.deltaTime;
        }

        private IEnumerator SpawnFruits()
        {
            while (true)
            {
                // Kalkulasi tingkat kesulitan
                float currentDelay = Mathf.Max(minSpawnDelay, spawnDelay - (timeElapsed * delayDecreaseRate));

                yield return new WaitForSeconds(currentDelay);

                if (fruitPrefabs.Length == 0) continue;

                // Calculate half of the spawner's X scale to know the spawn limits
                float halfWidth = transform.localScale.x / 2f;
                float randomX = Random.Range(-halfWidth, halfWidth);

                Vector3 spawnPosition = transform.position + new Vector3(randomX, 0f, 0f);

                GameObject prefab = null;
                // Check if we should spawn a bomb
                if (bombPrefabs != null && bombPrefabs.Length > 0 && Random.value < bombChance)
                {
                    prefab = bombPrefabs[Random.Range(0, bombPrefabs.Length)];
                }
                else
                {
                    prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
                }

                GameObject spawnedFruit = Instantiate(prefab, spawnPosition, Quaternion.identity);

                Rigidbody rb = spawnedFruit.GetComponent<Rigidbody>();
                if (rb == null) rb = spawnedFruit.GetComponentInChildren<Rigidbody>();

                if (rb != null)
                {
                    // Add slight random velocity so they don't fall perfectly straight
                    Vector3 randomVelocity = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-1f, 1f));
                    rb.linearVelocity = randomVelocity;
                    
                    // Add a little bit of random rotation (torque) so it spins beautifully as it falls
                    rb.AddTorque(Random.insideUnitSphere * Random.Range(2f, 5f), ForceMode.Impulse);
                }
            }
        }
    }
}
