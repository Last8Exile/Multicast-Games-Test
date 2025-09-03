using Cysharp.Threading.Tasks;

using Scripts.Unity.Gameplay;
using Scripts.Unity.Systems;
using Scripts.Unity.Tools;

using UnityEngine;

namespace Scripts.Unity
{
    public class Core : MonoBehaviour
    {
        public static Core Instance { get; private set; }

        [SerializeField] private PrefabCollection _instantiateOnAwake;

        private void Awake()
        {
            Instance = this;

            SerializationExtensions.Init();

            foreach (var prefab in _instantiateOnAwake.Prefabs)
                Instantiate(prefab).RemoveCloneFromName();
        }

        private void Start()
        {
            SaveSystem.Instance.Load("Player 1");

            MainMenu();
        }

        public void MainMenu()
        {
            GameplaySystem.Instance.Hide();
            EndLevelMenuSystem.Instance.SetVisible(false);

            MainMenuSystem.Instance.SetVisible(true);
        }

        public void StartLevel(ILevelData levelData)
        {
            MainMenuSystem.Instance.SetVisible(false);
            EndLevelMenuSystem.Instance.SetVisible(false);

            GameplaySystem.Instance.StartLevel(levelData);
        }

        public void EndLevel()
        {
            GameplaySystem.Instance.Hide();

            EndLevelMenuSystem.Instance.SetVisible(true);
        }

        public void Quit()
        {
            ExitAsync().Forget();
            async UniTaskVoid ExitAsync()
            {
                var result = await DialogMenuSystem.Instance.ShowAsync(new DialogOptions
                {
                    Title = "Exit",
                    Message = "Are you sure you want to exit?",
                    Ok = "Yes",
                    Cancel = "No",
                });

                if (result.IsOk)
                {
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.ExitPlaymode();
#endif
                }
            }
        }
    }
}