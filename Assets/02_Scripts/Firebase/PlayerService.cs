using UnityEngine;
using System.Collections.Generic;
using Firebase.Firestore;
using System.Threading.Tasks;

public class PlayerService : MonoBehaviour
{
    private readonly FirebaseFirestore _db;

    public PlayerService(FirebaseFirestore db)
    {
        _db = db;
    }

    private DocumentReference PlayerDoc(string uid) => _db.Collection("players").Document(uid);

    public async Task CreateOrUpdateNicknameAsync(string uid, string nickname)
    {
        var docRef = PlayerDoc(uid);
        var snap = await docRef.GetSnapshotAsync();

        if(!snap.Exists)
        {
            var createData = new Dictionary<string, object>
            {
                { "uid", uid},
                { "nickname", nickname },
                {  "bestKills", 0},
                { "createdAt", FieldValue.ServerTimestamp},
                { "updatedAt", FieldValue.ServerTimestamp}

            };

            await docRef.SetAsync(createData);
        }
        else
        {
            var updateData = new Dictionary<string, object>
            {
                { "nickname", nickname },
                {  "updatedAt", FieldValue.ServerTimestamp}

            };
            await docRef.UpdateAsync(updateData);
        }
    }

    public async Task UpdateBestKillsIfHigherAsync(string uid, int currentGameKills)
    {
        var docRef = PlayerDoc(uid);
        await _db.RunTransactionAsync(async transaction =>
        {
            var snap = await transaction.GetSnapshotAsync(docRef);

            if (!snap.Exists)
            {
                var newData = new Dictionary<string, object>
                {
                    { "uid", uid },
                    { "nickname", "NoName" },
                    { "bestKills", currentGameKills },
                    { "createdAt", FieldValue.ServerTimestamp },
                    { "updatedAt", FieldValue.ServerTimestamp }
                };

                transaction.Set(docRef, newData);
                return;
            }
            int prevBest = snap.ContainsField("bestKills") ? snap.GetValue<int>("bestKills") : 0;

            if (currentGameKills > prevBest)
            {
                transaction.Update(docRef, new Dictionary<string, object>
                {
                    { "bestKills", currentGameKills },
                    { "updatedAt", FieldValue.ServerTimestamp }
                });
            }

        });
    }
}
