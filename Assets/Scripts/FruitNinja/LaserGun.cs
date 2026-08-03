using UnityEngine;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using Mediapipe.Tasks.Vision.HandLandmarker;

namespace FruitNinja
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserGun : MonoBehaviour
    {
        private LineRenderer laserLine;

        public bool mirrorX = true;
        public float sensitivity; // 1.0 is 1:1, higher means less hand movement needed
        public float detectionTimeout = 0.15f; // 0.15 default
        public float laserDuration = 0.05f; // Very short for rapid fire look

        private bool isHandDetected = false;
        private bool hasNewDetection = false;
        private float timeSinceLastDetection = 0f;
        private Vector2 targetNormalizedPosition;
        private float laserTimer = 0f;

        private void Awake()
        {
            laserLine = GetComponent<LineRenderer>();
            laserLine.enabled = false;

            // Default laser line visual setup
            laserLine.startWidth = 0.1f;
            laserLine.endWidth = 0.1f;
            laserLine.material = new Material(Shader.Find("Sprites/Default")); // Bright white/unlit material
            laserLine.startColor = Color.red;
            laserLine.endColor = Color.yellow;
        }

        private void OnEnable()
        {
            HandLandmarkerRunner.OnHandLandmarkDetected += OnHandLandmark;
        }

        private void OnDisable()
        {
            HandLandmarkerRunner.OnHandLandmarkDetected -= OnHandLandmark;
        }

        private void OnHandLandmark(HandLandmarkerResult result)
        {
            if (result.handLandmarks != null && result.handLandmarks.Count > 0)
            {
                var hand = result.handLandmarks[0];
                if (hand.landmarks != null && hand.landmarks.Count > 8)
                {
                    // Use index finger tip for aiming
                    var indexFingerTip = hand.landmarks[8];

                    // Get raw coordinates
                    float rawX = mirrorX ? (1.0f - indexFingerTip.x) : indexFingerTip.x;
                    float rawY = 1.0f - indexFingerTip.y;

                    // Apply sensitivity from the center (0.5, 0.5)
                    float x = 0.5f + ((rawX - 0.5f) * sensitivity);
                    float y = 0.5f + ((rawY - 0.5f) * sensitivity);

                    hasNewDetection = true;
                    targetNormalizedPosition = new Vector2(x, y);
                    return;
                }
            }
        }

        private void Update()
        {
            if (hasNewDetection)
            {
                isHandDetected = true;
                timeSinceLastDetection = 0f;
                hasNewDetection = false;
            }
            else
            {
                timeSinceLastDetection += Time.deltaTime;
                if (timeSinceLastDetection > detectionTimeout)
                {
                    isHandDetected = false;
                }
            }

            if (isHandDetected && Camera.main != null)
            {
                ShootLaser();
            }
            else
            {
                laserLine.enabled = false;
            }

            // Lerp laser width back to normal 0.1f smoothly (Visual Juice)
            if (laserLine.enabled)
            {
                float currentWidth = Mathf.Lerp(laserLine.startWidth, 0.1f, Time.unscaledDeltaTime * 20f);
                laserLine.startWidth = currentWidth;
                laserLine.endWidth = currentWidth;

                laserTimer -= Time.deltaTime;
                if (laserTimer <= 0f)
                {
                    laserLine.enabled = false;
                }
            }
        }

        private void ShootLaser()
        {
            // Convert to screen position
            Vector3 screenPos = new Vector3(targetNormalizedPosition.x * Screen.width, targetNormalizedPosition.y * Screen.height, 0f);

            // Create ray from camera
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // Start laser slightly in front of the camera so it doesn't clip
            Vector3 laserStart = ray.origin + ray.direction * 1f;
            laserStart.y -= 2f; // Offset it slightly down so it looks like it comes from the bottom of the screen
            Vector3 laserEnd = ray.origin + ray.direction * 50f;

            // SphereCast gives a thicker beam than Raycast, making it easier to aim and hit
            if (Physics.SphereCast(ray, 0.5f, out RaycastHit hit, 100f, Physics.AllLayers, QueryTriggerInteraction.Collide))
            {
                laserEnd = hit.point;
                Fruit fruit = hit.collider.GetComponentInParent<Fruit>();
                if (fruit != null)
                {
                    fruit.HitByLaser(hit.point);
                    // Visual Juice: Pulse the laser width when hitting a fruit!
                    laserLine.startWidth = 1.2f;
                    laserLine.endWidth = 1.2f;
                }
            }

            // Visualize Laser
            laserLine.SetPosition(0, laserStart);
            laserLine.SetPosition(1, laserEnd);
            laserLine.enabled = true;
            laserTimer = laserDuration;
        }
    }
}
