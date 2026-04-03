using UnityEngine;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;
public class RankingService 
{
    private readonly FirebaseFirestore _db;

    public RankingService(FirebaseFirestore db)
    {
        _db = db;
    }

    public async Task<List<PlayerData>> LoadTop100Async()
    {
        Query query = _db.Collection("players")
            .OrderByDescending("bestKills")
            .OrderBy("updatedAt")
            .Limit(100);

        QuerySnapshot qs = await query.GetSnapshotAsync();

        var result = new List<PlayerData>();

        foreach(var doc in qs.Documents)
        {
            if(doc.Exists)
            {
                result.Add(doc.ConvertTo<PlayerData>());
            }

        }

        return result;
    }
}
