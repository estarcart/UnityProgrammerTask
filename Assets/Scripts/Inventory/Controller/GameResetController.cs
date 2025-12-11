using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetController : MonoBehaviour
{
    [SerializeField] private InventorySaveController inventorySaveController;
    [SerializeField] private WorldItemSaveController worldItemSaveController;

    public void ResetGame()
    {
        DeleteAllSaveData();
        ReloadCurrentScene();
    }

    public void DeleteAllSaveData()
    {
        if (inventorySaveController != null)
        {
            inventorySaveController.DeleteSave();
        }

        if (worldItemSaveController != null)
        {
            worldItemSaveController.DeleteSave();
        }
        else if (WorldItemSaveController.Instance != null)
        {
            WorldItemSaveController.Instance.DeleteSave();
        }
    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
