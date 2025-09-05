using Cysharp.Threading.Tasks;

using Scripts.Unity.Extensions;
using Scripts.Unity.Systems;
using Scripts.Unity.Tools;

using System;
using System.Linq;

using UnityEngine;

namespace Scripts.Unity
{
    public class Core : MonoBehaviour
    {
        public static Core Instance { get; private set; }
        public ServiceProvider ServiceProvider { get; private set; }
        [SerializeField] private PrefabCollection _instantiateOnAwake;

        private void Awake()
        {
            Instance = this;
            ServiceProvider = new();

            SerializationExtensions.Init();
            foreach (var prefab in _instantiateOnAwake.Prefabs)
                Instantiate(prefab).RemoveCloneFromName();
        }

        private void Start()
        {
            if (Application.isMobilePlatform)
                Application.targetFrameRate = Convert.ToInt32(Screen.resolutions.Max(r => r.refreshRateRatio.value));

            Systems<SaveSystem>.Instance.Load("Player 1");

            MainMenu();
        }

        public void MainMenu()
        {
            Systems<GameplaySystem>.Instance.Hide();
            Systems<EndLevelMenuSystem>.Instance.SetVisible(false);

            Systems<MainMenuSystem>.Instance.SetVisible(true);
        }

        public void StartLevel(ILevelData levelData)
        {
            Systems<MainMenuSystem>.Instance.SetVisible(false);
            Systems<EndLevelMenuSystem>.Instance.SetVisible(false);

            Systems<GameplaySystem>.Instance.StartLevel(levelData);
        }

        public void EndLevel()
        {
            Systems<GameplaySystem>.Instance.Hide();

            Systems<EndLevelMenuSystem>.Instance.SetVisible(true);
        }

        public void Quit()
        {
            QuitAsync().Forget();
            async UniTaskVoid QuitAsync()
            {
                var result = await Systems<DialogMenuSystem>.Instance.ShowAsync(new DialogOptions
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