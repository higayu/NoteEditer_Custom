using NoteEditor.Model;
using NoteEditor.Utility;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace NoteEditor.Presenter
{
    public class LoadPresenter : MonoBehaviour
    {
        [SerializeField]
        Button loadButton = default; // ロードボタン
        [SerializeField]
        Text messageText = default; // メッセージ表示

        void Awake()
        {
            loadButton.OnClickAsObservable()
                .Subscribe(_ => OpenLoadDialog())
                .AddTo(this);
        }

        void OpenLoadDialog()
        {
            string directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");

            if (!Directory.Exists(directoryPath))
            {
                messageText.text = "譜面フォルダが見つかりません";
                return;
            }

            string[] files = Directory.GetFiles(directoryPath, "*.json");
            if (files.Length == 0)
            {
                messageText.text = "譜面データがありません";
                return;
            }

            // 仮のダイアログ処理（実際は UI で選択できるようにする）
            string filePath = files[0]; // 最初のJSONを読み込む
            Load(filePath);
        }

        void Load(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                EditDataSerializer.Deserialize(json); // JSONをパースして `EditData` に反映
                messageText.text = filePath + " を読み込みました";
            } catch (System.Exception e)
            {
                messageText.text = "ロードエラー: " + e.Message;
            }
        }
    }
}
