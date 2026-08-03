using UnityEngine;

namespace FruitNinja
{
    public class Fruit : MonoBehaviour
    {
        public GameObject wholeFruit;
        public GameObject slicedFruitObject; // Drag the sliced child object here
        public int points = 1;
        private Rigidbody rb;
        private bool isSliced = false;

        private void Awake()
        {
            // Initial state: hide sliced, show whole
            if (wholeFruit != null) wholeFruit.SetActive(true);
            if (slicedFruitObject != null) slicedFruitObject.SetActive(false);

            // Ensure colliders are triggers so they don't bounce off each other
            Collider[] colls = GetComponentsInChildren<Collider>();
            foreach (var c in colls) c.isTrigger = true;
        }



        public void HitByLaser(Vector3 hitPosition)
        {
            if (!isSliced)
            {
                Slice(hitPosition, Vector3.up);
            }
        }

        private void Slice(Vector3 hitPosition, Vector3 hitDirection)
        {
            isSliced = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(points);
                GameManager.Instance.TriggerHitImpact();
            }
            else
            {
                Debug.LogWarning("GameManager.Instance is null! Score not added.");
            }

            Rigidbody wholeRb = null;
            if (wholeFruit != null)
            {
                wholeRb = wholeFruit.GetComponent<Rigidbody>();
                wholeFruit.SetActive(false);
            }

            if (slicedFruitObject != null)
            {
                // Teleport the sliced object to where the whole fruit fell
                if (wholeFruit != null)
                {
                    slicedFruitObject.transform.position = wholeFruit.transform.position;
                    slicedFruitObject.transform.rotation = wholeFruit.transform.rotation;
                }

                slicedFruitObject.SetActive(true);
                // buat slicedFruitObject diignore raycast
                Collider[] slicedColls = slicedFruitObject.GetComponentsInChildren<Collider>();
                foreach (var c in slicedColls) c.gameObject.layer = 2;

                Rigidbody[] rbs = slicedFruitObject.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody slicedRb in rbs)
                {
                    if (wholeRb != null) slicedRb.linearVelocity = wholeRb.linearVelocity;
                    slicedRb.AddExplosionForce(15f, hitPosition, 5f, 0f, ForceMode.Force);
                }
            }

            // Destroy the root object after 3 seconds so the sliced parts have time to fall
            Destroy(gameObject, 3f);
        }

        private void Update()
        {
            Transform trackingTransform = wholeFruit != null ? wholeFruit.transform : transform;
            if (trackingTransform.position.y < -10f)
            {
                Destroy(gameObject);
            }
        }
    }
}
