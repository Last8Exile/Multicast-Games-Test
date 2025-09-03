using Newtonsoft.Json;

using UnityEngine;

namespace Scripts.Unity.Extensions
{
    public enum PrefsAction
    {
        Get,
        Set,
    }

    public static class PlayerPrefsExtensions
    {
        public static void Prefs(string key, PrefsAction action, ref string value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                {
                    value = PlayerPrefs.GetString(key, value);
                    break;
                }
                case PrefsAction.Set:
                {
                    PlayerPrefs.SetString(key, value);
                    PlayerPrefs.Save();
                    break;
                }
            }
        }

        public static void PrefsJson<T>(string key, PrefsAction action, ref T value)
        {
            switch (action)
            {
                case PrefsAction.Get:
                {
                    var json = PlayerPrefs.GetString(key, string.Empty);
                    if (string.IsNullOrEmpty(json))
                        return;
                    value = JsonConvert.DeserializeObject<T>(json);
                    break;
                }
                case PrefsAction.Set:
                {
                    var json = JsonConvert.SerializeObject(value);
                    PlayerPrefs.SetString(key, json);
                    PlayerPrefs.Save();
                    break;
                }
            }
        }
    }
}