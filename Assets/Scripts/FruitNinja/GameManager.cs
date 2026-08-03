using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace FruitNinja
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int score;
        public TextMeshProUGUI scoreText;

        // Game Feel variables
        private Vector3 originalCameraPos;
        private Coroutine shakeCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (Camera.main != null)
                {
                    originalCameraPos = Camera.main.transform.localPosition;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddScore(int amount)
        {
            score += amount;
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score;
            }
        }

        public void TriggerHitImpact()
        {
            // 1. Hit Stop (Freeze time to 5% speed for 0.05 seconds - diperlama sedikit)
            StartCoroutine(HitStopRoutine(0.06f, 0.02f));

            // 2. Camera Shake (Diperbesar magnitude-nya agar lebih terasa)
            if (Camera.main != null)
            {
                if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeRoutine(0.2f, 1.0f));
            }
        }

        private IEnumerator HitStopRoutine(float duration, float timeScale)
        {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(duration); // Gunakan Realtime agar tidak ikut membeku
            Time.timeScale = 1f;
        }

        private IEnumerator ShakeRoutine(float duration, float magnitude)
        {
            float elapsed = 0.0f;
            Transform camTransform = Camera.main.transform;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                camTransform.localPosition = originalCameraPos + new Vector3(x, y, 0f);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            camTransform.localPosition = originalCameraPos;
        }

        public void GameOver()
        {
            Debug.Log("Game Over!");
        }
    }
}
