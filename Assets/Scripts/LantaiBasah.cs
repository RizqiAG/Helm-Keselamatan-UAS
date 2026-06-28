using UnityEngine;

public class LantaiBasah : MonoBehaviour
{
    [Header("Pengaturan Penalti")]
    public float potongWaktu = 10f; 
    
    private bool sudahKena = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sudahKena)
        {
            // PERBAIKAN FINAL: Menggunakan FindAnyObjectByType untuk Unity versi terbaru
            PlayerInteract scriptPlayer = Object.FindAnyObjectByType<PlayerInteract>();

            if (scriptPlayer != null)
            {
                sudahKena = true; 

                // Potong waktu dan panggil fungsi update UI agar layar langsung berubah
                scriptPlayer.waktuSisa -= potongWaktu;
                if (scriptPlayer.waktuSisa < 0) scriptPlayer.waktuSisa = 0;

                if (scriptPlayer.pemutarSuara != null && scriptPlayer.suaraKalah != null)
                {
                    scriptPlayer.pemutarSuara.PlayOneShot(scriptPlayer.suaraKalah);
                }

                Debug.Log("Awas Licin! Waktu terpotong " + potongWaktu + " detik.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sudahKena = false;
        }
    }
}