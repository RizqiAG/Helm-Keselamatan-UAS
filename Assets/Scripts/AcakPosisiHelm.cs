using System.Collections.Generic;
using UnityEngine;

public class AcakPosisiHelm : MonoBehaviour
{
    [Header("Daftar Helm yang Akan Diacak")]
    public GameObject[] daftarHelm; 

    [Header("Titik Kemunculan (Spawn Points)")]
    public Transform[] titikAcak; 

    void Start()
    {
        // Jalankan fungsi acak secara otomatis saat Level 2 dimulai
        AcakPosisi();
    }

    public void AcakPosisi()
    {
        // Pengecekan keamanan: jumlah titik harus lebih banyak atau sama dengan jumlah helm
        if (daftarHelm.Length > titikAcak.Length)
        {
            Debug.LogWarning("Titik acak kurang! Tambahkan lebih banyak titik di peta.");
            return;
        }

        // Membuat list sementara untuk menampung titik acak
        // (Agar tidak ada 2 helm yang nyasar ke 1 titik yang sama)
        List<Transform> sisaTitik = new List<Transform>(titikAcak);

        foreach (GameObject helm in daftarHelm)
        {
            // Pilih satu titik secara acak dari sisa titik yang ada
            int indeksAcak = Random.Range(0, sisaTitik.Count);
            Transform titikTerpilih = sisaTitik[indeksAcak];

            // Pindahkan posisi objek Induk Helm ke titik terpilih
            helm.transform.position = titikTerpilih.position;

            // Hapus titik yang sudah terpakai dari list sementara
            // agar tidak terpilih lagi oleh helm berikutnya
            sisaTitik.RemoveAt(indeksAcak);
        }
    }
}