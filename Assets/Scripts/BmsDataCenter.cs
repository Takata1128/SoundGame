using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class BmsDataCenter : MonoBehaviour
{
    public static List<BmsData> CachedBmsDataList;
    private string[] beatmapPaths;
    public static float ScrollSpeed = 5.0f;



    // Start is called before the first frame update
    void Start()
    {
        if (CachedBmsDataList == null)
        {
            LoadBeatScores();
        }
        // BmsDataListなど保持のため
        DontDestroyOnLoad(this);
    }

    // 譜面の読み込み
    void LoadBeatScores()
    {
        var beatmapDirectory = Application.dataPath + "/../Beatmaps";
        beatmapPaths = Directory.GetFiles(beatmapDirectory, "*.bm?", SearchOption.AllDirectories);
        CachedBmsDataList = new List<BmsData>();
        for (int i = 0; i < beatmapPaths.Length; i++)
        {
            // 拡張子がbms or bmeのやつを選ぶ（2度手間だけどこうしないとダメ）
            string ext = Path.GetExtension(beatmapPaths[i]);
            if (ext == ".bms" || ext == ".bme")
            {
                try
                {
                    CachedBmsDataList.Add(BmsLoader.Load(beatmapPaths[i]));
                }
                catch (KeyNotFoundException e)
                {
                    Debug.Log(beatmapPaths[i] + "のロードに失敗しました。");
                    Debug.Log(e);
                }
            }
        }
    }
}
