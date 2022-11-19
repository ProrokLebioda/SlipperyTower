using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class XMLManager : MonoBehaviour
{
    public Leaderboard leaderboard;
    public static XMLManager instance;
    void Awake()
    {
        instance = this;
        if (!Directory.Exists(Application.persistentDataPath + "/Highscores/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Highscores/");
        }
    }

    public void SaveScores(List<HighscoreEntry> scoresToSave)
    {
        leaderboard.list = scoresToSave;
        XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
        FileStream stream = new(Application.persistentDataPath + "/Highscores/highscores.xml", FileMode.Create);
        serializer.Serialize(stream, leaderboard);
        stream.Close();
    }

    public List<HighscoreEntry> LoadScores()
    {
        if (File.Exists(Application.persistentDataPath + "/Highscores/highscores.xml"))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
            FileStream stream = new FileStream(Application.persistentDataPath + "/Highscores/highscores.xml", FileMode.Open);
            leaderboard = serializer.Deserialize(stream) as Leaderboard;
        }
        return leaderboard.list;
    }

    [System.Serializable]
    public class Leaderboard
    {
        public List<HighscoreEntry> list = new List<HighscoreEntry>();
    }
}
