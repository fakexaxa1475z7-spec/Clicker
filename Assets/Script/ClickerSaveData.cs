using System;
using System.IO;
using UnityEngine;

[Serializable]
public class ClickerSaveData
{
    public float power;
    public int rebirths;
    public int baseIncome;

    public int[] storeLevels;
    public int[] clickLevels;
    public int[] baseLevels;
    public int[] rebirthLevels;

    public long lastExitTime;
}

public static class SaveSystem
{
    static string path = Application.persistentDataPath + "/save.dat";

    public static void Save(ClickerSaveData data)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(data.power);
            writer.Write(data.rebirths);
            writer.Write(data.baseIncome);
            writer.Write(data.lastExitTime);

            // Store
            writer.Write(data.storeLevels.Length);
            foreach (int level in data.storeLevels)
                writer.Write(level);

            // Click
            writer.Write(data.clickLevels.Length);
            foreach (int level in data.clickLevels)
                writer.Write(level);

            // Base
            writer.Write(data.baseLevels.Length);
            foreach (int level in data.baseLevels)
                writer.Write(level);

            //Rebirth
            writer.Write(data.rebirthLevels.Length);
            foreach (int level in data.rebirthLevels)
                writer.Write(level);
        }
    }

    public static ClickerSaveData Load()
    {
        if (!File.Exists(path))
            return null;

        ClickerSaveData data = new ClickerSaveData();

        using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            data.power = reader.ReadSingle();
            data.rebirths = reader.ReadInt32();
            data.baseIncome = reader.ReadInt32();
            data.lastExitTime = reader.ReadInt64();

            int storeLength = reader.ReadInt32();
            data.storeLevels = new int[storeLength];
            for (int i = 0; i < storeLength; i++)
                data.storeLevels[i] = reader.ReadInt32();

            int clickLength = reader.ReadInt32();
            data.clickLevels = new int[clickLength];
            for (int i = 0; i < clickLength; i++)
                data.clickLevels[i] = reader.ReadInt32();

            int baseLength = reader.ReadInt32();
            data.baseLevels = new int[baseLength];
            for (int i = 0; i < baseLength; i++)
                data.baseLevels[i] = reader.ReadInt32();

            int rebirthLength = reader.ReadInt32();
            data.rebirthLevels = new int[rebirthLength];
            for (int i = 0; i < rebirthLength; i++)
                data.rebirthLevels[i] = reader.ReadInt32();
        }

        return data;
    }
}