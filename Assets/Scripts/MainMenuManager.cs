using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel UI")]
    public GameObject panelPilihLevel;
    public GameObject panelCredit; // Tambahan untuk panel About/Credit

    void Start()
    {
        // Memastikan panel disembunyikan saat menu utama pertama kali dibuka
        if (panelPilihLevel != null)
        {
            panelPilihLevel.SetActive(false);
        }

        // Menyembunyikan panel credit di awal
        if (panelCredit != null)
        {
            panelCredit.SetActive(false);
        }
    }

    // Fungsi ini sudah tersambung dengan tombol "Mulai Game" milikmu
    public void MainkanGame()
    {
        // Alih-alih langsung memuat level, sekarang fungsi ini membuka panel
        if (panelPilihLevel != null) 
        {
            panelPilihLevel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel Pilih Level belum dimasukkan ke Inspector!");
        }
    }

    // --- FUNGSI UNTUK TOMBOL DI DALAM PANEL LEVEL ---

    // Dipanggil oleh tombol "Kembali / X"
    public void TutupPanelLevel()
    {
        if (panelPilihLevel != null) 
        {
            panelPilihLevel.SetActive(false);
        }
    }

    // Dipanggil oleh tombol "Level 1"
    public void MasukLevel1()
    {
        // Pastikan Level_1 ada di urutan ke-1 di Build Settings
        SceneManager.LoadScene(1); 
    }

    // Dipanggil oleh tombol "Level 2"
    public void MasukLevel2()
    {
        // Pastikan Level_2 ada di urutan ke-2 di Build Settings
        SceneManager.LoadScene(2); 
    }

    // Dipanggil oleh tombol "Level 3"
    public void MasukLevel3()
    {
        // Pastikan Level_3 ada di urutan ke-3 di Build Settings
        SceneManager.LoadScene(3); 
    }

    // --- FUNGSI UNTUK TOMBOL TENTANG/CREDIT ---

    // Dipanggil oleh tombol "Credit" di Menu Utama
    public void BukaPanelCredit()
    {
        if (panelCredit != null)
        {
            panelCredit.SetActive(true);
        }
    }

    // Dipanggil oleh tombol "Kembali" di dalam Panel Credit
    public void TutupPanelCredit()
    {
        if (panelCredit != null)
        {
            panelCredit.SetActive(false);
        }
    }

    // ------------------------------------------

    public void KeluarGame()
    {
        Debug.Log("GAME DITUTUP!");
        Application.Quit();
    }
}