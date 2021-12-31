using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

public static class SaveSystem {
    
    public static PlayerData data = new PlayerData(0, 0, new List<List<int>>(), 0, 0, 0, 0, 0, 0, 0, new List<bool>(), 0, 0, 0, false);

    public static void SavePlayer() {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/numlearn.learn";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void LoadPlayer() {
        string path = Application.persistentDataPath + "/numlearn.learn";
        if(File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
        } else {
            Debug.LogError("Save file not found in " + path);
        }
    }
}
