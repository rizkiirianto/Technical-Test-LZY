using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using Mediapipe.Tasks.Vision.HandLandmarker;

namespace FruitNinja
{
    [RequireComponent(typeof(LineRenderer))]
    public class MenuLaserPointer : MonoBehaviour
    {
        private LineRenderer laserLine;

        public bool mirrorX = true;
        public float sensitivity = 1.5f; // 1.0 is 1:1, higher means less hand movement needed
        public float detectionTimeout = 0.15f; // 0.15 default
        public float fistThreshold = 0.25f; // Jarak maksimum ujung jari ke pergelangan untuk dianggap mengepal

        private bool isHandDetected = false;
        private bool isFist = false;
        private bool wasFist = false; // Untuk mendeteksi 'klik' sekali saja (on mouse down)
        private bool hasNewDetection = false;
        private float timeSinceLastDetection = 0f;
        private Vector2 targetNormalizedPosition;

        private void Awake()
        {
            laserLine = GetComponent<LineRenderer>();
            laserLine.enabled = false;

            // Default laser line visual setup
            laserLine.startWidth = 0.05f;
            laserLine.endWidth = 0.05f;

            Material laserMat = new Material(Shader.Find("Unlit/Color"));
            laserMat.color = Color.cyan; // Laser UI berwarna cyan/biru terang
            laserLine.material = laserMat;

            // Berusaha memaksa render di atas objek lain (jika Canvas mode Camera)
            laserLine.sortingOrder = 32767;
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
                if (hand.landmarks != null && hand.landmarks.Count > 20)
                {
                    // Gunakan pangkal jari tengah (Middle Finger MCP - titik 9) untuk membidik di Main Menu.
                    var aimPoint = hand.landmarks[9];

                    // Deteksi Kepalan Tangan (Fist)
                    // Cek jarak ujung jari tengah (12), manis (16), dan kelingking (20) terhadap pergelangan tangan (0)
                    var wrist = hand.landmarks[0];
                    var middleTip = hand.landmarks[12];
                    var ringTip = hand.landmarks[16];
                    var pinkyTip = hand.landmarks[20];

                    Vector2 wristPos = new Vector2(wrist.x, wrist.y);
                    float midDist = Vector2.Distance(new Vector2(middleTip.x, middleTip.y), wristPos);
                    float ringDist = Vector2.Distance(new Vector2(ringTip.x, ringTip.y), wristPos);
                    float pinkyDist = Vector2.Distance(new Vector2(pinkyTip.x, pinkyTip.y), wristPos);

                    // Jika ketiga jari tersebut melipat ke dekat pergelangan tangan, itu adalah gestur kepal (fist)
                    isFist = (midDist < fistThreshold && ringDist < fistThreshold && pinkyDist < fistThreshold);

                    // Get raw coordinates (untuk aiming) menggunakan telapak tangan
                    float rawX = mirrorX ? (1.0f - aimPoint.x) : aimPoint.x;
                    float rawY = 1.0f - aimPoint.y;

                    // Apply sensitivity from the center (0.5, 0.5)
                    float x = 0.5f + ((rawX - 0.5f) * sensitivity);
                    float y = 0.5f + ((rawY - 0.5f) * sensitivity);

                    hasNewDetection = true;

                    // PENTING: Kunci (freeze) posisi bidikan jika sedang mengepal!
                    // Ini mencegah laser meleset/memendek akibat ujung jari ikut tertekuk saat mengepal.
                    if (!isFist)
                    {
                        targetNormalizedPosition = new Vector2(x, y);
                    }
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
                ProcessUIInteraction();
            }
            else
            {
                laserLine.enabled = false;
                wasFist = false; // Reset state
            }
        }

        private void ProcessUIInteraction()
        {
            // Convert to screen position
            Vector3 screenPos = new Vector3(targetNormalizedPosition.x * Screen.width, targetNormalizedPosition.y * Screen.height, 0f);

            // Buat sinarnya terlihat
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Vector3 laserStart = ray.origin + ray.direction * 0.5f;
            laserStart.y -= 1f; // Offset agar terlihat keluar dari bawah layar
            Vector3 laserEnd = ray.origin + ray.direction * 15f; // Di menu tidak perlu sepanjang di game

            laserLine.SetPosition(0, laserStart);
            laserLine.SetPosition(1, laserEnd);

            // Visual feedback: Ubah warna jadi Merah saat mengepal, Cyan saat biasa
            laserLine.material.color = isFist ? Color.red : Color.cyan;
            laserLine.startWidth = isFist ? 0.15f : 0.05f; // Sedikit menebal saat mengepal
            laserLine.endWidth = isFist ? 0.15f : 0.05f;

            laserLine.enabled = true;

            // Logika Klik UI dengan GraphicRaycaster
            if (EventSystem.current != null)
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = screenPos;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                // Jika mengenai sesuatu di UI, cari Button teratas
                Button targetBtn = null;
                foreach (var res in results)
                {
                    targetBtn = res.gameObject.GetComponentInParent<Button>();
                    if (targetBtn != null) break;
                }

                if (targetBtn != null)
                {
                    // Deteksi Klik: Jika SEKARANG mengepal, dan SEBELUMNYA tidak mengepal (OnKeyDown)
                    if (isFist && !wasFist)
                    {
                        Debug.Log("Mengklik Tombol UI: " + targetBtn.gameObject.name);
                        targetBtn.onClick.Invoke();
                    }
                }
            }

            // Simpan status kepalan untuk frame berikutnya
            wasFist = isFist;
        }
    }
}
