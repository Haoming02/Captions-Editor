using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    private string Extension(string ext) => (ext[0] == '.') ? ext : $".{ext}";

    void Start()
    {
        toolsContent.SetActive(false);
    }

    private void OnFilesLoaded()
    {
        LoadButton.interactable = false;
        toolsContent.SetActive(true);
    }

    private void RestoreAll(ref List<int> list)
    {
        list.Clear();
        for (int i = 0; i < captionCount; i++)
            list.Add(i);
    }

    private void ToggleVisibility()
    {
        for (int i = 0; i < captionCount; i++)
            captionRows[i].SetActive(currentlyEditing.Contains(i) && currentlyFiltering.Contains(i));
    }

    public void FilterByFile(string filter)
    {
        if (filter.Trim() == string.Empty)
            RestoreAll(ref currentlyEditing);
        else
        {
            string[] filters = filter.Split(',').Select(t => t.Trim()).ToArray();
            List<int[]> allOccurances = new List<int[]>();

            foreach (string tag in filters)
            {
                for (int i = 0; i < captionCount; i++)
                    if (captionRows[i].GetComponent<RowObject>().Filter(tag, fleFuzzy.isOn, out int[] index))
                        allOccurances.Add(index);
            }

            if (allOccurances.Count == 0)
            {
                RestoreAll(ref currentlyEditing);
            }
            else
            {
                if (allOccurances.Count == 1)
                    filteredFiles = allOccurances[0];
                else
                    filteredFiles = (fleIsAnd.isOn ? IntersectFiles(allOccurances) : UnionFiles(allOccurances));

                for (int i = 0; i < captionCount; i++)
                {
                    if (captionRows[i].GetComponent<RowObject>().Filter(filteredFiles))
                    {
                        if (!currentlyEditing.Contains(i))
                            currentlyEditing.Add(i);
                    }
                    else
                    {
                        if (currentlyEditing.Contains(i))
                            currentlyEditing.Remove(i);
                    }
                }
            }
        }

        ToggleVisibility();
    }

    public void FilterByCaption(string filter)
    {
        if (filter.Trim() == string.Empty)
            RestoreAll(ref currentlyFiltering);
        else
        {
            string[] filters = filter.Split(',').Select(t => t.Trim()).ToArray();

            for (int i = 0; i < captionCount; i++)
            {
                if (captionRows[i].GetComponent<RowObject>().Filter(filters, cptFuzzy.isOn, cptIsAnd.isOn))
                {
                    if (!currentlyFiltering.Contains(i))
                        currentlyFiltering.Add(i);
                }
                else
                {
                    if (currentlyFiltering.Contains(i))
                        currentlyFiltering.Remove(i);
                }
            }
        }

        ToggleVisibility();
    }

    private T[] UnionFiles<T>(List<T[]> listOfLists)
    {
        return listOfLists.SelectMany(list => list).Distinct().ToArray();
    }

    private T[] IntersectFiles<T>(List<T[]> listOfLists)
    {
        var result = listOfLists[1].Intersect(listOfLists[0]);
        for (int i = 2; i < listOfLists.Count; i++)
            result = result.Intersect(listOfLists[i]);
        return result.ToArray();
    }

    // === Load ===

    private bool ParseFilePath()
    {
        try
        {
            captionFiles = Directory.GetFiles(FilePath.text, $"*{Extension(FileExtension.text)}", SearchOption.AllDirectories);

            FilePath.GetComponent<Image>().color = White;
            return true;
        }
        catch
        {
            FilePath.GetComponent<Image>().color = Pink;
            return false;
        }
    }

    private async Task Init()
    {
        await Task.Run(() =>
        {
            fileCount = captionFiles.Length;
            captions = new List<string>[fileCount];

            captionMap = new Dictionary<string, List<int>>();
            captionsToDelete = new List<string>();

            currentlyEditing = new List<int>();
            currentlyFiltering = new List<int>();

            captionRows = new List<GameObject>();
        }).ConfigureAwait(false);
    }

    private async Task ReadCaptions()
    {
        Task<string>[] readTasks = new Task<string>[fileCount];
        Task[] assignTasks = new Task[fileCount];

        for (int i = 0; i < fileCount; i++)
            readTasks[i] = File.ReadAllTextAsync(captionFiles[i]);

        await Task.WhenAll(readTasks).ConfigureAwait(false);

        for (int i = 0; i < fileCount; i++)
        {
            int index = i;
            assignTasks[i] = Task.Run(() => { captions[index] = readTasks[index].Result.Replace('\n', ',').Split(',').Select(t => t.Trim()).ToList(); });
        }

        await Task.WhenAll(assignTasks).ConfigureAwait(false);
    }

    private async Task ConstructMapping()
    {
        await Task.Run(() =>
        {
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
        }).ConfigureAwait(false);
    }

    private async Task GenerateCaptionRows()
    {
        foreach (string pair in captionMap.Keys)
        {
            GameObject row = Instantiate(pfRow, Contents);
            row.GetComponent<RowObject>().Init(pair, captionMap[pair].ToArray());
            captionRows.Add(row);
        }

        for (int i = 0; i < captionCount; i++)
        {
            currentlyEditing.Add(i);
            currentlyFiltering.Add(i);
        }

        await Task.Delay(1);
    }

    // === Load ===
    // === Save ===

    private async Task DeleteCaptions()
    {
        await Task.Run(() =>
        {
            foreach (string key in captionsToDelete)
            {
                foreach (int i in captionMap[key])
                    captions[i].Remove(key);
            }
        });
    }

    private async Task WriteCaptions()
    {
        Task[] writeTasks = new Task[fileCount];

        for (int i = 0; i < fileCount; i++)
            writeTasks[i] = File.WriteAllTextAsync(captionFiles[i], string.Join(", ", captions[i]));

        await Task.WhenAll(writeTasks);
    }

    // === Save ===
}
