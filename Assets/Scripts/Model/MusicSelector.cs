using NoteEditor.Utility;
using System.Collections.Generic;
using System.IO;
using UniRx;

namespace NoteEditor.Model
{
    public class MusicSelector : SingletonMonoBehaviour<MusicSelector>
    {
        // 初期ディレクトリとして Windows のダウンロードフォルダを設定
        private static readonly string DefaultDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

        ReactiveProperty<string> directoryPath_ = new ReactiveProperty<string>(DefaultDirectory);
        ReactiveProperty<List<FileItemInfo>> filePathList_ = new ReactiveProperty<List<FileItemInfo>>(new List<FileItemInfo>());
        ReactiveProperty<string> selectedFileName_ = new ReactiveProperty<string>();

        public static ReactiveProperty<string> DirectoryPath { get { return Instance.directoryPath_; } }
        public static ReactiveProperty<List<FileItemInfo>> FilePathList { get { return Instance.filePathList_; } }
        public static ReactiveProperty<string> SelectedFileName { get { return Instance.selectedFileName_; } }

        void Awake()
        {
            // シングルトンインスタンスを正しく初期化
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // `directoryPath_` の初期値が `null` の場合、デフォルトのディレクトリを設定
            if (string.IsNullOrEmpty(directoryPath_.Value))
            {
                directoryPath_.Value = DefaultDirectory;
            }

            // ディレクトリが存在しない場合は作成する
            if (!Directory.Exists(directoryPath_.Value))
            {
                Directory.CreateDirectory(directoryPath_.Value);
            }
        }
    }
}
