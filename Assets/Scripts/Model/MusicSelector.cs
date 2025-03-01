using NoteEditor.Utility;
using System.Collections.Generic;
using System.IO;
using UniRx;

namespace NoteEditor.Model
{
    public class MusicSelector : SingletonMonoBehaviour<MusicSelector>
    {
        // �����f�B���N�g���Ƃ��� Windows �̃_�E�����[�h�t�H���_��ݒ�
        private static readonly string DefaultDirectory = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

        ReactiveProperty<string> directoryPath_ = new ReactiveProperty<string>(DefaultDirectory);
        ReactiveProperty<List<FileItemInfo>> filePathList_ = new ReactiveProperty<List<FileItemInfo>>(new List<FileItemInfo>());
        ReactiveProperty<string> selectedFileName_ = new ReactiveProperty<string>();

        public static ReactiveProperty<string> DirectoryPath { get { return Instance.directoryPath_; } }
        public static ReactiveProperty<List<FileItemInfo>> FilePathList { get { return Instance.filePathList_; } }
        public static ReactiveProperty<string> SelectedFileName { get { return Instance.selectedFileName_; } }

        void Awake()
        {
            // �V���O���g���C���X�^���X�𐳂���������
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // `directoryPath_` �̏����l�� `null` �̏ꍇ�A�f�t�H���g�̃f�B���N�g����ݒ�
            if (string.IsNullOrEmpty(directoryPath_.Value))
            {
                directoryPath_.Value = DefaultDirectory;
            }

            // �f�B���N�g�������݂��Ȃ��ꍇ�͍쐬����
            if (!Directory.Exists(directoryPath_.Value))
            {
                Directory.CreateDirectory(directoryPath_.Value);
            }
        }
    }
}
