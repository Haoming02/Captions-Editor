using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    private string Extension(string ext) => (ext[0] == '.') ? ext : $".{ext}";

    private IEnumerator LoadCaptions()
    {
        try
        {
            captionFiles = Directory.GetFiles(FilePath.text, $"*{Extension(FileExtension.text)}", SearchOption.AllDirectories);
            fileCount = captionFiles.Length;
            FilePath.GetComponent<Image>().color = White;
            LoadButton.enabled = false;
        }
        catch
        {
            FilePath.GetComponent<Image>().color = Pink;
            yield break;
        }

        captions = new List<string>[fileCount];
        captionMap = new Dictionary<string, List<int>>();
        deletionMap = new Dictionary<string, int[]>();

        Task<string>[] readTasks = new Task<string>[fileCount];

        for (int i = 0; i < fileCount; i++)
            readTasks[i] = File.ReadAllTextAsync(captionFiles[i]);

        yield return Task.WhenAll(readTasks);

        for (int i = 0; i < fileCount; i++)
            captions[i] = readTasks[i].Result.Replace('\n', ',').Split(',').Select(t => t.Trim()).ToList();

        for (int i = 0; i < fileCount; i++)
        {
            foreach (string tag in captions[i])
            {
                if (captionMap.ContainsKey(tag))
                    captionMap[tag].Add(i);
                else
                    captionMap[tag] = new List<int> { i };
            }
        }

        var orderedMap = captionMap.OrderByDescending(x => x.Value.Count).ToDictionary(x => x.Key, x => x.Value);
        captionRows = new List<GameObject>();

        foreach (string pair in orderedMap.Keys)
        {
            GameObject row = Instantiate(pfRow, Contents);
            row.GetComponent<RowStruct>().Init(pair, captionMap[pair].ToArray());
            captionRows.Add(row);
        }

        yield return null;

        Top();
    }

    public void Filter(string filter)
    {
        if (filter.Trim() == string.Empty)
        {
            foreach (var obj in captionRows)
                obj.SetActive(true);

            return;
        }

        string[] filters = filter.Split(',').Select(t => t.Trim()).ToArray();

        foreach (var obj in captionRows)
            obj.SetActive(obj.GetComponent<RowStruct>().Filter(filters, Fuzzy.isOn, isAnd.isOn));
    }

    private IEnumerator SaveCaptions()
    {
        foreach (string key in deletionMap.Keys)
        {
            foreach (int i in deletionMap[key])
                captions[i].Remove(key);
        }

        Task[] writeTasks = new Task[fileCount];

        for (int i = 0; i < fileCount; i++)
            writeTasks[i] = File.WriteAllTextAsync(captionFiles[i], string.Join(", ", captions[i]));

        yield return Task.WhenAll(writeTasks);

        SaveButton.GetComponent<Image>().color = Green;
    }
}
