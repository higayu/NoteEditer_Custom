using NoteEditor.Model;
using SFB;
using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace NoteEditor.Presenter
{
    public class SelectMusic : MonoBehaviour
    {
        [SerializeField]
        MusicLoader musicLoader = default;

        [SerializeField]
        Button loadButton = default;

        [SerializeField]
        Text selectText = default;

        void Start()
        {
            // Load ボタンを SelectAndLoadMusic() に変更
            loadButton.onClick.AddListener(SelectAndLoadMusic);
        }

        public void SelectAndLoadMusic()
        {
            // Windows のダウンロードフォルダを取得
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            string selectedFile = OpenFileDialogForWav(defaultPath);

            if (!string.IsNullOrEmpty(selectedFile))
            {
                Debug.Log($"選択されたファイル: {selectedFile}");
                musicLoader.Load(selectedFile);
                selectText.text = selectedFile;
            } else
            {
                Debug.Log("ファイル選択がキャンセルされました。");
            }
        }


        private string OpenFileDialogForWav(string initialDirectory)
        {
            // Windows のダウンロードフォルダをデフォルトパスとして設定
            string defaultPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

            // 拡張子フィルタ（.wav のみ）
            var extensions = new[]
            {
        new ExtensionFilter("WAV Files", "wav")
    };

            // ファイルダイアログを開く（初期ディレクトリを `Downloads` に設定）
            var paths = StandaloneFileBrowser.OpenFilePanel("Select WAV File", defaultPath, extensions, false);

            if (paths.Length > 0 && paths[0].Length > 0)
            {
                string filePath = paths[0];

                // 拡張子が .wav かどうかを確認（念のため）
                if (Path.GetExtension(filePath).ToLower() != ".wav")
                {
                    Debug.LogWarning($"選択されたファイルは .wav ではありません: {filePath}");
                    System.Windows.Forms.MessageBox.Show("選択されたファイルは .wav ではありません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return defaultPath; // デフォルトのダウンロードフォルダを返す
                }

                selectText.text = filePath; // 選択したファイルのパスを UI に表示
                Debug.Log("選択した WAV ファイルのパス: " + filePath);
                return filePath;
            } else
            {
                Debug.Log("ファイル選択がキャンセルされました。");
                return defaultPath; // キャンセルされた場合はデフォルトのダウンロードフォルダを返す
            }
        }

    }
}
