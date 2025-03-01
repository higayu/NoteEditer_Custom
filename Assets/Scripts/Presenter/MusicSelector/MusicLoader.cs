//using NoteEditor.Model;
//using NoteEditor.Notes;
//using System.Collections;
//using System.IO;
//using UniRx;
//using UnityEngine;
//using UnityEngine.Networking;

//namespace NoteEditor.Presenter
//{
//    public class MusicLoader : MonoBehaviour
//    {
//        void Awake()
//        {
//            ResetEditor();
//        }

//        public void Load(string fileName)
//        {
//             .wav 以外のファイルは読み込まない
//            if (Path.GetExtension(fileName).ToLower() != ".wav")
//            {
//                Debug.LogWarning($"警告: {fileName} は .wav ファイルではありません。読み込みを中止します。");
//                System.Windows.Forms.MessageBox.Show("読み込むファイルが「.wav」ではありません");
//                return;
//            }
//            StartCoroutine(LoadMusic(fileName));
//        }

//        IEnumerator LoadMusic(string fileName)
//        {
//            拡張子を取得
//            string extension = Path.GetExtension(fileName).ToLower();

//             .wav 以外なら警告を出して処理を中断
//            if (extension != ".wav")
//            {
//                Debug.LogWarning($"警告: {fileName} は .wav ファイルではありません。読み込みを中止します。");
//                System.Windows.Forms.MessageBox.Show($"警告: {fileName} は .wav ファイルではありません。読み込みを中止します。");
//                yield break;
//            }

//            using (var www = new WWW("file:///" + Path.Combine(MusicSelector.DirectoryPath.Value, fileName)))
//            {
//                yield return www;

//                EditCommandManager.Clear();
//                ResetEditor();
//                Audio.Source.clip = www.GetAudioClip();

//                if (Audio.Source.clip == null)
//                {
//                    Debug.LogError($"エラー: {fileName} の読み込みに失敗しました。");
//                } else
//                {
//                    EditData.Name.Value = fileName;
//                    LoadEditData();
//                    Audio.OnLoad.OnNext(Unit.Default);

//                    **音楽が正常にロードされたらフラグを立てる * *
//                    if (Loadmanager.instance != null)
//                    {
//                        Loadmanager.instance.WAV_Lode_Flg = true;
//                        Debug.Log("WAV_Lode_Flg を true に設定しました。");
//                    } else
//                    {
//                        Debug.LogError("Loadmanager のインスタンスが見つかりません。");
//                    }
//                }
//            }
//        }



//        void LoadEditData()
//        {
//            var fileName = Path.ChangeExtension(EditData.Name.Value, "json");
//            var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");
//            var filePath = Path.Combine(directoryPath, fileName);

//            if (File.Exists(filePath))
//            {
//                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
//                EditDataSerializer.Deserialize(json);
//            }
//        }

//        public void ResetEditor()
//        {
//            Audio.TimeSamples.Value = 0;
//            Audio.SmoothedTimeSamples.Value = 0;
//            Audio.IsPlaying.Value = false;
//            Audio.Source.clip = null;
//            EditState.NoteType.Value = NoteTypes.Single;
//            EditState.LongNoteTailPosition.Value = NotePosition.None;
//            EditData.BPM.Value = 120;
//            EditData.OffsetSamples.Value = 0;
//            EditData.Name.Value = "Note Editor";
//            EditData.MaxBlock.Value = Settings.MaxBlock;
//            EditData.LPB.Value = 4;

//            foreach (var note in EditData.Notes.Values)
//            {
//                note.Dispose();
//            }

//            EditData.Notes.Clear();
//            Resources.UnloadUnusedAssets();
//        }
//    }
//}

using NoteEditor.Model;
using NoteEditor.Notes;
using System.Collections;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.Networking; // UnityWebRequest 用

namespace NoteEditor.Presenter
{
    public class MusicLoader : MonoBehaviour
    {
        void Awake()
        {
            ResetEditor();
        }

        public void Load(string fileName)
        {
            // .wav 以外のファイルは読み込まない
            if (Path.GetExtension(fileName).ToLower() != ".wav")
            {
                Debug.LogWarning($"警告: {fileName} は .wav ファイルではありません。読み込みを中止します。");
                return;
            }

            StartCoroutine(LoadMusic(fileName));
        }

        IEnumerator LoadMusic(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("エラー: LoadMusic に渡されたファイル名が null または空です。");
                yield break;
            }

            if (!File.Exists(fileName))
            {
                Debug.LogError($"エラー: 指定されたファイルが見つかりません。パス: {fileName}");
                yield break;
            }

            // Windows用のパス変換（\ を / に修正）
            string formattedPath = "file:///" + fileName.Replace("\\", "/");
            Debug.Log($"ロードするファイル: {formattedPath}");

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(formattedPath, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                // Unity 2019 のエラーチェック方法
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogError($"エラー: 音楽ファイルのロードに失敗しました。エラー: {www.error}");
                    yield break;
                }

                EditCommandManager.Clear();
                ResetEditor();
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                Audio.Source.clip = audioClip;

                if (Audio.Source.clip == null)
                {
                    Debug.LogError($"エラー: {fileName} の音楽データが正しくロードできませんでした。");
                } else
                {
                    EditData.Name.Value = Path.GetFileName(fileName);
                    LoadEditData();
                    Audio.OnLoad.OnNext(Unit.Default);
                    Debug.Log($"音楽ファイル {fileName} を正常にロードしました。");

                    // **音楽が正常にロードされたらフラグを立てる**
                    if (Loadmanager.instance != null)
                    {
                        Loadmanager.instance.WAV_Lode_Flg = true;
                        Debug.Log("WAV_Lode_Flg を true に設定しました。");
                    } else
                    {
                        Debug.LogError("Loadmanager のインスタンスが見つかりません。");
                    }
                }
            }
        }


        void LoadEditData()
        {
            var fileName = Path.ChangeExtension(EditData.Name.Value, "json");
            var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");
            var filePath = Path.Combine(directoryPath, fileName);

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
                EditDataSerializer.Deserialize(json);
            }
        }

        public void ResetEditor()
        {
            Audio.TimeSamples.Value = 0;
            Audio.SmoothedTimeSamples.Value = 0;
            Audio.IsPlaying.Value = false;
            Audio.Source.clip = null;
            EditState.NoteType.Value = NoteTypes.Single;
            EditState.LongNoteTailPosition.Value = NotePosition.None;
            EditData.BPM.Value = 120;
            EditData.OffsetSamples.Value = 0;
            EditData.Name.Value = "Note Editor";
            EditData.MaxBlock.Value = Settings.MaxBlock;
            EditData.LPB.Value = 4;

            foreach (var note in EditData.Notes.Values)
            {
                note.Dispose();
            }

            EditData.Notes.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}

