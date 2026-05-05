using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // --- Các thông tin cần lưu trữ ---
    public int totalGold;
    public int highscore;
    public int currentLevel;
    public string lastUsedWeapon;
    // Bạn có thể thêm danh sách vũ khí đã mở khóa, stats, v.v.
    
    // Khởi tạo giá trị mặc định cho lần đầu chơi
    public GameData()
    {
        totalGold = 0;
        highscore = 0;
        currentLevel = 1;
        lastUsedWeapon = "BasicGun";
    }
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public GameData gameData;

    private string saveFilePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Đường dẫn lưu file: C:/Users/TenUser/AppData/LocalLow/CompanyName/ProjectName/save.json
        saveFilePath = Path.Combine(Application.persistentDataPath, "player_save.json");
        
        LoadGame();
    }

    // LƯU GAME
    public void SaveGame()
    {
        try
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"<color=green>Game Saved to: {saveFilePath}</color>");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Loi khi luu game: {e.Message}");
        }
    }

    // TẢI GAME
    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                gameData = JsonUtility.FromJson<GameData>(json);
                Debug.Log("<color=cyan>Game Data Loaded.</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Loi khi tai game: {e.Message}");
                gameData = new GameData();
            }
        }
        else
        {
            Debug.Log("Khong tim thay file save, tao moi du lieu.");
            gameData = new GameData();
            SaveGame();
        }
    }

    // XÓA DỮ LIỆU (Dùng khi muốn Reset game)
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            gameData = new GameData();
            Debug.Log("Save file deleted.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame(); // Tu dong luu khi thoat game
    }
}
