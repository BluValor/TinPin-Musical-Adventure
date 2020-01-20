using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordReader
{
    public FightData MovesData = new FightData();

    public RecordReader(string dataPath)
    {
        StreamReader reader = new StreamReader(dataPath);
        string text = reader.ReadToEnd();
        reader.Close();
        //MovesData.MovesInTime = JsonArrayHelper.FromJson<MoveData>(text);
        MovesData = JsonUtility.FromJson(text, typeof(FightData)) as FightData;
    }
}
