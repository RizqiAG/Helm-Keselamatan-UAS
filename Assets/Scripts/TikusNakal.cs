using UnityEngine;

public class TikusNakal : MonoBehaviour
{
    [Header("Pengaturan Gerak")]
    public float kecepatan = 3f;
    public float waktuGantiArah = 2f; // Tiap 2 detik tikus belok acak
    private float timerArah;

    [Header("Pengaturan Mental (Knockback)")]
    public float jarakMental = 2f; // Seberapa jauh pemain didorong mundur

    void Start()
    {
        timerArah = waktuGantiArah;
    }

    void Update()
    {
        // 1. Tikus selalu berlari maju ke arah depannya sendiri
        transform.Translate(Vector3.forward * kecepatan * Time.deltaTime);

        // 2. Timer untuk ganti arah acak agar keliling ruangan
        timerArah -= Time.deltaTime;
        if (timerArah <= 0)
        {
            GantiArahAcak();
            timerArah = waktuGantiArah;
        }
    }

    // Fungsi memutar arah tikus secara acak
    void GantiArahAcak()
    {
        float sudutAcak = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, sudutAcak, 0);
    }

    // Kalau tikus nabrak tembok/benda padat, langsung putar balik
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            GantiArahAcak();
        }
    }

    // Kalau "Aura Trigger" mengenai pemain -> PENTALKAN!
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Hitung arah dorongan (menjauh dari posisi tikus)
            Vector3 arahMental = (other.transform.position - transform.position).normalized;
            arahMental.y = 0; // Nol-kan sumbu Y agar pemain tidak mental terbang ke plafon

            // Cek apakah pemain menggunakan Character Controller bawaan Unity
            CharacterController cc = other.GetComponent<CharacterController>();
            
            if (cc != null)
            {
                // Pindah paksa menggunakan sistem pergerakan CC
                cc.Move(arahMental * jarakMental);
            }
            else
            {
                // Kalau pakai Rigidbody / sistem lain, dorong posisi aslinya
                other.transform.position += arahMental * jarakMental;
            }

            Debug.Log("Kena Tikus! Pemain Terpental.");
        }
    }
}