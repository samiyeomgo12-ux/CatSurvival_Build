using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NicknamePanelController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private FirebaseBootstrap bootstrap;

    [Header("UI (Legacy)")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Text nameText;      // »ó´Ü Ç¥½Ã ÀÌ¸§
    [SerializeField] private Text uidText;       // "uid : xxxx"
    [SerializeField] private InputField inputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Text messageText;   // ¿¡·¯/¾È³» (¼±ÅÃ)

    private FirebaseFirestore _db;
    private string _uid;
    private string _nickname; // ÇöÀç ´Ð³×ÀÓ(¾øÀ» ¼ö ÀÖÀ½)
    private Coroutine _messageCoroutine;
    // ÇÑ±Û/¿µ¹®/¼ýÀÚ 2~12
    private readonly Regex _nickRegex = new Regex(@"^[a-zA-Z0-9°¡-ÆR]{2,12}$");

    private async void Start()
    {
        await bootstrap.InitializeAndSignInAsync();
        _db = FirebaseBootstrap.Db;
        _uid = FirebaseBootstrap.Uid;

        saveButton.onClick.AddListener(OnClickSave);
        closeButton.onClick.AddListener(() => panelRoot.SetActive(false));

        await LoadProfileAndRefreshUI();
    }

    private async Task LoadProfileAndRefreshUI()
    {
        var docRef = _db.Collection("players").Document(_uid);
        var snap = await docRef.GetSnapshotAsync();

        if (snap.Exists && snap.ContainsField("nickname"))
            _nickname = snap.GetValue<string>("nickname");
        else
            _nickname = "";

        uidText.text = "Uid : " + _uid.Substring(0, Mathf.Min(8, _uid.Length));
        nameText.text = GetDisplayName(_nickname, _uid);

        if (inputField != null) inputField.text = _nickname;
        SetMessage("");
    }

    private string NormalizeNickname(string nick)
    {
        return nick.Trim(); //°ø¹éÁ¦°Å     
    }

    private async Task<bool> IsNicknameDuplicatedAsync(string newNick)
    {
        string normalized = NormalizeNickname(newNick);

        var qs = await _db.Collection("players")
            .WhereEqualTo("nicknameLoswer", normalized)
            .Limit(1)
            .GetSnapshotAsync();

        if (qs.Count == 0) return false;

        var firstDoc = qs.Documents.First();

        string foundUid = firstDoc.Id;

        return foundUid != _uid;
    }
    private async void OnClickSave()
    {
        try
        {
            string newNick = inputField.text.Trim();

            if (string.IsNullOrEmpty(newNick))
            {
                await SaveNicknameToFirestore("");
                _nickname = "";
                RefreshNameOnly();
                SetMessage("´Ð³×ÀÓ »ý¼º Àü, UID·Î Ç¥½ÃÇÕ´Ï´Ù.");
                panelRoot.SetActive(false);
                return;
            }

            if (!_nickRegex.IsMatch(newNick))
            {
                SetMessage("´Ð³×ÀÓÀº 2~12ÀÚ, ÇÑ±Û/¿µ¹®/¼ýÀÚ¸¸ °¡´É");
                return;
            }

            // Áßº¹ °Ë»ç Ãß°¡
            bool duplicated = await IsNicknameDuplicatedAsync(newNick);
            if (duplicated)
            {
                SetMessage("ÀÌ¹Ì »ç¿ë ÁßÀÎ ¾ÆÀÌµðÀÔ´Ï´Ù. ´Ù¸¥ ¾ÆÀÌµð¸¦ ÀÔ·ÂÇØÁÖ¼¼¿ä.");
                return;
            }

            await SaveNicknameToFirestore(newNick);

            _nickname = newNick;
            RefreshNameOnly();
            SetMessage("´Ð³×ÀÓ »ý¼º ¿Ï·á");
            panelRoot.SetActive(false);
        }
        finally
        {
            saveButton.interactable = true;
        }
    }

    private async Task SaveNicknameToFirestore(string nick)
    {
        var docRef = _db.Collection("players").Document(_uid);
        var snap = await docRef.GetSnapshotAsync();

        if (!snap.Exists)
        {
            await docRef.SetAsync(new Dictionary<string, object>
            {
                { "uid", _uid },
                { "nickname", nick }, // ºó ¹®ÀÚ¿­ Çã¿ë Á¤Ã¥
                { "bestKills", 0 },
                { "createdAt", FieldValue.ServerTimestamp },
                { "updatedAt", FieldValue.ServerTimestamp }
            });
        }
        else
        {
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "nickname", nick },
                { "updatedAt", FieldValue.ServerTimestamp }
            });
        }
    }

    private void RefreshNameOnly()
    {
        nameText.text = GetDisplayName(_nickname, _uid);
        uidText.text = "Uid : " + _uid.Substring(0, Mathf.Min(8, _uid.Length));
    }

    private string GetDisplayName(string nickname, string uid)
    {
        if (!string.IsNullOrWhiteSpace(nickname)) //ºñ¾îÀÖÁö ¾ÊÀ¸¸é
            return nickname; //º°¸í
       
        string displayName = uid.Substring(0, Mathf.Min(8, uid.Length));
        // UID ÀüÃ¼ ´ë½Å ÀÏºÎ¸¸ ³ëÃâ
        return displayName; //¾Æ´Ï¸é uid
    }

    private void SetMessage(string msg)
    {
        if (messageText == null) return; 
        if (messageText != null) messageText.text = msg;

        if (_messageCoroutine != null)
            StopCoroutine(_messageCoroutine);

        _messageCoroutine = StartCoroutine(ClearMessage(5f));
    }

    private IEnumerator ClearMessage(float seconds)
    {
        yield return new WaitForSeconds(seconds); //secounds ÃÊ µÚ¿¡ ½ÇÇà 

        if (messageText != null)
            messageText.text = "";

        _messageCoroutine = null; 
    }
    // ¿ÜºÎ ¹öÆ°¿¡¼­ ÆÐ³Î ¿­ ¶§ È£Ãâ
    public void OpenPanel()
    {
        panelRoot.SetActive(true);
        inputField.text = _nickname;
        SetMessage("");
    }
}