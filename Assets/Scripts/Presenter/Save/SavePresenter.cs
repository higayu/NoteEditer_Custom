﻿using NoteEditor.Model;
using NoteEditor.Utility;
using SFB;
using System;
using System.IO;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Windows.Forms;
using Button = UnityEngine.UI.Button; // MessageBox を使用するため

namespace NoteEditor.Presenter
{
    public class SavePresenter : MonoBehaviour
    {
        [SerializeField]
        Button saveButton = default;
        [SerializeField]
        Text messageText = default;
        [SerializeField]
        Color unsavedStateButtonColor = default;
        [SerializeField]
        Color savedStateButtonColor = Color.white;

        [SerializeField]
        GameObject saveDialog = default;
        [SerializeField]
        Button dialogSaveButton = default;
        [SerializeField]
        Button dialogDoNotSaveButton = default;
        [SerializeField]
        Button dialogCancelButton = default;
        [SerializeField]
        Text dialogMessageText = default;

        ReactiveProperty<bool> mustBeSaved = new ReactiveProperty<bool>();

        void Awake()
        {
            var editPresenter = EditNotesPresenter.Instance;

            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(KeyCode.Escape))
                .Subscribe(_ => UnityEngine.Application.Quit());

            var saveActionObservable = this.UpdateAsObservable()
                .Where(_ => KeyInput.CtrlPlus(KeyCode.S))
                .Merge(saveButton.OnClickAsObservable());

            mustBeSaved = Observable.Merge(
                    EditData.BPM.Select(_ => true),
                    EditData.OffsetSamples.Select(_ => true),
                    EditData.MaxBlock.Select(_ => true),
                    editPresenter.RequestForEditNote.Select(_ => true),
                    editPresenter.RequestForAddNote.Select(_ => true),
                    editPresenter.RequestForRemoveNote.Select(_ => true),
                    editPresenter.RequestForChangeNoteStatus.Select(_ => true),
                    Audio.OnLoad.Select(_ => false),
                    saveActionObservable.Select(_ => false))
                .SkipUntil(Audio.OnLoad.DelayFrame(1))
                .Do(unsaved => saveButton.GetComponent<Image>().color = unsaved ? unsavedStateButtonColor : savedStateButtonColor)
                .ToReactiveProperty();

            mustBeSaved.SubscribeToText(messageText, unsaved => unsaved ? "保存が必要な状態" : "");

            saveActionObservable.Subscribe(_ => Save());

            dialogSaveButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    mustBeSaved.Value = false;
                    saveDialog.SetActive(false);
                    Save();
                    UnityEngine.Application.Quit();
                });

            dialogDoNotSaveButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    mustBeSaved.Value = false;
                    saveDialog.SetActive(false);
                    UnityEngine.Application.Quit();
                });

            dialogCancelButton.AddListener(
                EventTriggerType.PointerClick,
                (e) =>
                {
                    saveDialog.SetActive(false);
                });

            UnityEngine.Application.wantsToQuit += ApplicationQuit;
        }

        bool ApplicationQuit()
        {
            if (mustBeSaved.Value)
            {
                dialogMessageText.text = "Do you want to save the changes you made in the note '"
                    + EditData.Name.Value + "' ?" + System.Environment.NewLine
                    + "Your changes will be lost if you don't save them.";
                saveDialog.SetActive(true);
                return false;
            }

            return true;
        }

        //public void Save()
        //{
        //    var fileName = Path.ChangeExtension(EditData.Name.Value, "json");
        //    //var directoryPath = Path.Combine(Path.GetDirectoryName(MusicSelector.DirectoryPath.Value), "Notes");
        //    string downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        //    var filePath = Path.Combine(downloadPath, fileName);

        //    if (!Directory.Exists(downloadPath))
        //    {
        //        Directory.CreateDirectory(downloadPath);
        //    }

        //    var json = EditDataSerializer.Serialize();
        //    File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
        //    messageText.text = filePath + " に保存しました";
        //}

        public void Save()
        {
            var fileName = Path.ChangeExtension(EditData.Name.Value, "json");

            // デフォルトの保存場所（Windows のダウンロードフォルダ）
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            // 保存ダイアログを開く
            string filePath = StandaloneFileBrowser.SaveFilePanel("Save File", defaultPath, fileName, "json");

            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogWarning("保存がキャンセルされました。");
                messageText.text = "保存がキャンセルされました";
                return;
            }

            try
            {
                var json = EditDataSerializer.Serialize();
                File.WriteAllText(filePath, json, System.Text.Encoding.UTF8);
                Debug.Log($"ファイルを保存しました: {filePath}");
                messageText.text = filePath + " に保存しました";

                // **保存完了メッセージを表示**
                System.Windows.Forms.MessageBox.Show("保存が完了しました！\n\n" + filePath, "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception e)
            {
                Debug.LogError($"保存中にエラーが発生しました: {e.Message}");
                messageText.text = "保存エラー: " + e.Message;
            }
        }

    }
}
