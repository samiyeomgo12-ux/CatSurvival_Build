using UnityEngine;
using System;
using Firebase.Firestore;

[FirestoreData]
public class PlayerData
{
    [FirestoreProperty] public string uid { get; set; }
    [FirestoreProperty] public string nickname { get; set; }
    [FirestoreProperty] public int bestKills { get; set; }
    [FirestoreProperty] public Timestamp createdAt { get; set; }
    [FirestoreProperty] public Timestamp updatedAt { get; set; }
}
