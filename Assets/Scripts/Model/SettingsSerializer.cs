using NoteEditor.DTO;
using System.Linq;
using UnityEngine;
using System.IO;

namespace NoteEditor.Model
{
    public class SettingsSerializer
    {
        public static void Deserialize(string json)
        {
            var dto = JsonUtility.FromJson<SettingsDTO>(json);
            Settings.NoteInputKeyCodes.Value = dto.noteInputKeyCodes
                .Select(keyCodeNum => (KeyCode)keyCodeNum)
                .ToList();

            Settings.MaxBlock = dto.maxBlock;
            // ダウンロードフォルダの取得（OSごとに処理）
            string defaultPath = GetDefaultDownloadFolder();
            Settings.WorkSpacePath.Value = string.IsNullOrEmpty(dto.workSpacePath)
                ? defaultPath//Application.persistentDataPath
                : dto.workSpacePath;
        }

        private static string GetDefaultDownloadFolder()
        {
            string downloadPath = "";

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                downloadPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            } else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                downloadPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            } else if (Application.platform == RuntimePlatform.LinuxPlayer)
            {
                downloadPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");
            } else
            {
                downloadPath = Application.persistentDataPath; // その他の環境ではデフォルトのパスを使用
            }

            return downloadPath;
        }

        public static string Serialize()
        {
            var data = new SettingsDTO();

            data.workSpacePath = Settings.WorkSpacePath.Value;
            data.maxBlock = EditData.MaxBlock.Value;
            data.noteInputKeyCodes = Settings.NoteInputKeyCodes.Value
                .Take(EditData.MaxBlock.Value)
                .Select(keyCode => (int)keyCode)
                .ToList();

            return JsonUtility.ToJson(data);
        }
    }
}
