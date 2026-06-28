using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI; // <--- TAMBAHAN: Wajib untuk membaca Slider UI

public class PlayerInteract : MonoBehaviour
{
    public float jarakAmbil = 20f;
    public string helmDiBawa = ""; 
    public Transform posisiTangan; 
    private GameObject wujudHelm;

    [Header("Sistem Skor UI")]
    public int skor = 0; 
    public TextMeshProUGUI teksSkor; 
    public GameObject teksMenang;    

    [Header("Sistem Timer UI")]
    public float waktuSisa = 60f; 
    public TextMeshProUGUI teksTimer; 
    public GameObject teksKalah;
    
    [Header("Menu UI")]
    public GameObject panelMenu; 
    public GameObject panelSetting; // <--- TAMBAHAN: Jendela Pengaturan
    public GameObject tombolNextLevel; 
    private bool gameSelesai = false; 

    [Header("Sistem Bintang (Rating)")]
    public GameObject[] bintangUI; 

    // --- TAMBAHAN: Sistem Pengaturan Audio ---
    [Header("Sistem Pengaturan Audio")]
    public AudioSource pemutarMusikLatar; // Untuk musik background
    public AudioSource pemutarSuara;      // Untuk efek (ambil, benar, kalah)
    public Slider sliderEfek;
    public Slider sliderMusik;
    // -----------------------------------------

    public AudioClip suaraAmbil;     
    public AudioClip suaraBenar;    
    public AudioClip suaraKalah;    

    void Start() 
    {
        Time.timeScale = 1f; 
        UpdateTampilanSkor();
        
        if (teksMenang != null) teksMenang.SetActive(false); 
        if (teksKalah != null) teksKalah.SetActive(false); 
        if (panelMenu != null) panelMenu.SetActive(false); 
        if (panelSetting != null) panelSetting.SetActive(false); // Sembunyikan setting di awal
        if (tombolNextLevel != null) tombolNextLevel.SetActive(false); 

        // Sembunyikan semua bintang saat game baru dimulai
        foreach(GameObject bintang in bintangUI)
        {
            if (bintang != null) bintang.SetActive(false);
        }

        if (pemutarSuara == null) pemutarSuara = GetComponent<AudioSource>();

        // --- MENGHUBUNGKAN SLIDER DENGAN VOLUME ---
        if (pemutarMusikLatar != null && sliderMusik != null)
        {
            sliderMusik.value = pemutarMusikLatar.volume; 
            sliderMusik.onValueChanged.AddListener(UbahVolumeMusik); 
        }

        if (pemutarSuara != null && sliderEfek != null)
        {
            sliderEfek.value = pemutarSuara.volume;
            sliderEfek.onValueChanged.AddListener(UbahVolumeEfek);
        }
        // ------------------------------------------
    }

    void Update()
    {
        // --- LOGIKA TOMBOL ESCAPE UNTUK BUKA/TUTUP MENU & SETTING ---
        if (Input.GetKeyDown(KeyCode.Escape) && !gameSelesai)
        {
            if (panelSetting != null && panelSetting.activeSelf)
            {
                // Jika menu setting terbuka, tombol ESC hanya menutup setting
                KlikTutupSetting();
            }
            else if (panelMenu != null)
            {
                if (panelMenu.activeSelf == false) 
                {
                    panelMenu.SetActive(true);
                    Time.timeScale = 0f; 
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else 
                {
                    panelMenu.SetActive(false);
                    Time.timeScale = 1f; 
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
        // -----------------------------------------------------------

        if (gameSelesai || Time.timeScale == 0f) return;

        HitungMundurTimer();

        if (Input.GetKeyDown(KeyCode.M)) 
        {
            if (posisiTangan == null) return;

            RaycastHit hit;
            int layerMask = ~LayerMask.GetMask("Player");

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, jarakAmbil, layerMask))
            {
                string objekTag = hit.transform.tag;

                if (objekTag.StartsWith("Helm") && helmDiBawa == "")
                {
                    helmDiBawa = objekTag;
                    wujudHelm = hit.transform.gameObject;
                    
                    wujudHelm.transform.SetParent(posisiTangan);
                    wujudHelm.transform.localPosition = Vector3.zero;
                    wujudHelm.transform.localRotation = Quaternion.identity;
                    wujudHelm.transform.localScale = new Vector3(1, 1, 1);
                    
                    Collider[] semuaCol = wujudHelm.GetComponentsInChildren<Collider>();
                    foreach(Collider col in semuaCol) col.enabled = false;
                    
                    Rigidbody[] semuaRB = wujudHelm.GetComponentsInChildren<Rigidbody>();
                    foreach(Rigidbody rb in semuaRB) {
                        rb.isKinematic = true;
                        rb.useGravity = false;
                    }

                    foreach (Transform anak in wujudHelm.transform)
                    {
                        anak.localPosition = Vector3.zero;
                    }

                    MainkanSuara(suaraAmbil);
                }
                
                else if (objekTag == "Jebakan")
                {
                    waktuSisa -= 15f; 
                    if (waktuSisa < 0) waktuSisa = 0; 
                    
                    UpdateTampilanTimer(waktuSisa); 
                    Destroy(hit.transform.gameObject); 
                    MainkanSuara(suaraKalah); 
                }

                else if (objekTag.StartsWith("NPC") && helmDiBawa != "")
                {
                    if ((objekTag == "NPC_Ayah" && helmDiBawa == "Helm_Besar") ||
                        (objekTag == "NPC_Ibu" && helmDiBawa == "Helm_Sedang") ||
                        (objekTag == "NPC_Anak" && helmDiBawa == "Helm_Kecil"))
                    {
                        if (wujudHelm != null) {
                            wujudHelm.SetActive(false); 
                            Destroy(wujudHelm);
                        }
                        
                        helmDiBawa = ""; 
                        wujudHelm = null; 

                        skor += 1; 
                        UpdateTampilanSkor();

                        MainkanSuara(suaraBenar);

                        if (skor >= 3)
                        {
                            MunculkanMenuAkhir(true); 
                        }
                    }
                }
            }
        }
    }

    void HitungMundurTimer()
    {
        if (waktuSisa > 0)
        {
            waktuSisa -= Time.deltaTime; 
            UpdateTampilanTimer(waktuSisa);
        }
        else
        {
            waktuSisa = 0;
            UpdateTampilanTimer(waktuSisa);
            MunculkanMenuAkhir(false); 
        }
    }

    void UpdateTampilanSkor()
    {
        if (teksSkor != null) teksSkor.text = "Helm Terkumpul: " + skor + " / 3";
    }

    void UpdateTampilanTimer(float sisaDetik)
    {
        if (teksTimer != null)
        {
            int menit = Mathf.FloorToInt(sisaDetik / 60f);
            int detik = Mathf.FloorToInt(sisaDetik % 60f);
            teksTimer.text = string.Format("Waktu: {0:00}:{1:00}", menit, detik);
        }
    }

    void MunculkanMenuAkhir(bool isMenang)
    {
        gameSelesai = true; 
        
        if (isMenang && teksMenang != null) 
        {
            teksMenang.SetActive(true);

            if (bintangUI != null && bintangUI.Length == 3)
            {
                if (waktuSisa >= 40f) 
                {
                    bintangUI[0].SetActive(true);
                    bintangUI[1].SetActive(true);
                    bintangUI[2].SetActive(true);
                }
                else if (waktuSisa >= 20f) 
                {
                    bintangUI[0].SetActive(true);
                    bintangUI[1].SetActive(true);
                }
                else 
                {
                    bintangUI[0].SetActive(true);
                }
            }
        }
        else if (!isMenang && teksKalah != null) 
        {
            teksKalah.SetActive(true);
            MainkanSuara(suaraKalah);
        }

        if (panelMenu != null) panelMenu.SetActive(true);

        if (tombolNextLevel != null)
        {
            tombolNextLevel.SetActive(isMenang);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void MainkanSuara(AudioClip clip)
    {
        if (pemutarSuara != null && clip != null)
        {
            pemutarSuara.PlayOneShot(clip);
        }
    }

    // --- FUNGSI MENGATUR VOLUME ---
    public void UbahVolumeMusik(float nilai)
    {
        if (pemutarMusikLatar != null) pemutarMusikLatar.volume = nilai;
    }

    public void UbahVolumeEfek(float nilai)
    {
        if (pemutarSuara != null) pemutarSuara.volume = nilai;
    }

    // --- FUNGSI BUKA TUTUP MENU PENGATURAN ---
    public void KlikBukaSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(true);
        if (panelMenu != null) panelMenu.SetActive(false);
    }

    public void KlikTutupSetting()
    {
        if (panelSetting != null) panelSetting.SetActive(false);
        if (panelMenu != null) panelMenu.SetActive(true);
    }
    // ----------------------------------------

    public void LanjutNextLevel()
    {
        Time.timeScale = 1f; 
        int levelSekarang = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelSekarang + 1);
    }

    public void KlikRestart()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void KlikHalamanUtama()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }

    public void KlikKeluar()
    {
        Application.Quit(); 
    }
}