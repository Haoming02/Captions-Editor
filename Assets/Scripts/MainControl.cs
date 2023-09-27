using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainControl : MonoBehaviour
{
    public static MainControl Instance { get; private set; }

    [SerializeField]
    private InputField FilePath;
    [SerializeField]
    private InputField FileExtension;

    [SerializeField]
    private GameObject pfRow;
    [SerializeField]
    private Transform Contents;
    [SerializeField]
    private ScrollRect Scroll;
    [SerializeField]
    private Toggle Fuzzy;

    private string Extension(string ext) => (ext[0] == '.') ? ext : $".{ext}";

    private List<GameObject> rows;

    private string[] captionFiles;
    private List<string>[] captions;
    private Dictionary<string, Occurance> captionMap;

    private int l;

    void Awake() { Instance = this; }

    public void Load()
    {
        StartCoroutine(LoadCaptions());
    }

    public void Save()
    {
       StartCoroutine(SaveCaptions());
    }

    public void Top()
    {
        Scroll.normalizedPosition = Vector2.up;
    }

    public void Down()
    {
        Scroll.normalizedPosition = Vector2.down;
    }

    public void DeleteTag(string tag, GameObject row)
    {
        var targets = captionMap[tag].index;

        foreach (int i in targets)
            captions[i].Remove(tag);

        rows.Remove(row);
    }

    public void Filter(string filter)
    {
        filter = filter.Trim();
        if (filter == string.Empty)
        {
            foreach (var obj in rows)
                obj.SetActive(true);
        }
        else
        {
            foreach (var obj in rows)
                obj.SetActive(obj.GetComponent<RowStruct>().Filter(filter, Fuzzy.isOn));
        }
    }

    private IEnumerator SaveCaptions()
    {
        Task[] writeTasks = new Task[l];

        for (int i = 0; i < l; i++)
            writeTasks[i] = File.WriteAllTextAsync(captionFiles[i], string.Join(", ", captions[i]));

        yield return Task.WhenAll(writeTasks);
    }

    private IEnumerator LoadCaptions()
    {
        try
        {
            captionFiles = Directory.GetFiles(FilePath.text, $"*{Extension(FileExtension.text)}", SearchOption.AllDirectories);
            l = captionFiles.Length;
        }
        catch
        {
            Debug.LogWarning("Invalid Path");
            yield break;
        }

        captions = new List<string>[l];
        Task<string>[] readTasks = new Task<string>[l];
        captionMap = new Dictionary<string, Occurance>();

        for (int i = 0; i < l; i++)
            readTasks[i] = File.ReadAllTextAsync(captionFiles[i]);

        yield return Task.WhenAll(readTasks);

        for (int i = 0; i < l; i++)
            captions[i] = readTasks[i].Result.Split(',').Select(t => t.Trim()).ToList();

        for (int i = 0; i < l; i++)
        {
            foreach (string tag in captions[i])
            {
                if (captionMap.ContainsKey(tag))
                {
                    captionMap[tag].times++;
                    captionMap[tag].index.Add(i);
                }
                else
                {
                    captionMap[tag] = new Occurance
                    {
                        times = 1,
                        index = new List<int> { i }
                    };
                }
            }
        }

        var orderedMap = captionMap.OrderByDescending(x => x.Value.times).ToDictionary(x => x.Key, x => x.Value);
        rows = new List<GameObject>();

        foreach (var pair in orderedMap.Keys)
        {
            var row = Instantiate(pfRow, Contents);
            row.GetComponent<RowStruct>().Init(pair, captionMap[pair].times, captionMap[pair].index.ToArray());
            rows.Add(row);
        }

        yield return null;

        Top();
    }

    private class Occurance
    {
        public int times;
        public List<int> index;
    }
}
