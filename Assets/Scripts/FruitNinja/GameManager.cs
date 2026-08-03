using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace FruitNinja
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int score;
        public TextMeshProUGUI scoreText;
        public int hp = 100;
        public TextMeshProUGUI hpText;
        public Image damageOverlay;
        public GameObject gameOverPanel;
        
        [Header("Audio SFX")]
        public AudioSource sfxSource;
        public AudioClip fruitHitSound;
        public AudioClip bombHitSound;

        private bool isGameOver = false;

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

                if (hpText != null) hpText.text = "HP: " + hp;
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
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

        public void TriggerBombImpact()
        {
            if (isGameOver) return;

            // Kurangi HP
            hp -= 25;
            if (hp <= 0) hp = 0;

            if (hpText != null) hpText.text = "HP: " + hp;

            // Camera Shake lebih hebat untuk bom
            if (Camera.main != null)
            {
                if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeRoutine(0.3f, 1.5f));
            }

            // Layar Merah
            StartCoroutine(BombImpactRoutine());
            
            // SFX Bomb
            if (sfxSource != null && bombHitSound != null)
            {
                sfxSource.PlayOneShot(bombHitSound);
            }

            if (hp <= 0)
            {
                GameOver();
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
            
            // SFX Fruit
            if (sfxSource != null && fruitHitSound != null)
            {
                sfxSource.PlayOneShot(fruitHitSound);
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

        private IEnumerator BombImpactRoutine()
        {
            if (damageOverlay != null)
            {
                Color c = damageOverlay.color;

                // Flash (50% opacity)
                damageOverlay.color = new Color(c.r, c.g, c.b, 0.5f);

                // Fade out smoothly
                float elapsed = 0f;
                float duration = 0.5f; // 0.5 detik
                while (elapsed < duration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float alpha = Mathf.Lerp(0.5f, 0f, elapsed / duration);
                    damageOverlay.color = new Color(c.r, c.g, c.b, alpha);
                    yield return null;
                }

                damageOverlay.color = new Color(c.r, c.g, c.b, 0f);
            }
        }

        public void GameOver()
        {
            if (isGameOver) return;
            isGameOver = true;
            Debug.Log("Game Over! HP habis.");

            // Matikan gameplay dengan membekukan waktu
            Time.timeScale = 0f;

            // Tampilkan panel Game Over
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            // Mulai penghitung waktu mundur 5 detik
            StartCoroutine(GameOverRoutine());
        }

        private IEnumerator GameOverRoutine()
        {
            // Menggunakan Realtime karena Time.timeScale sekarang 0
            yield return new WaitForSecondsRealtime(5f);

            // Kembalikan waktu ke normal sebelum memuat scene baru
            Time.timeScale = 1f;

            // Kembali ke Main Menu (Pastikan nama Scene Main Menu Anda sesuai)
            SceneManager.LoadScene("Scenes/MainMenu");
        }
    }
}
