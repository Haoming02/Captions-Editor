using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    private string Extension(string ext) => (ext[0] == '.') ? ext : $".{ext}";

    public void FilterByFile(string filter)
    {
        string[] filters = filter.Split(',').Select(t => t.Trim()).ToArray();
    }

    public void FilterByCaption(string filter)
    {
        if (filter.Trim() == string.Empty)
        {
            foreach (var obj in captionRows)
                obj.SetActive(true);

            return;
        }

        string[] filters = filter.Split(',').Select(t => t.Trim()).ToArray();

        foreach (var obj in captionRows)
            obj.SetActive(obj.GetComponent<RowObject>().Filter(filters, cptFuzzy.isOn, cptIsAnd.isOn));
    }

    private bool ParseFilePath()
    {
        try
        {
            captionFiles = Directory.GetFiles(FilePath.text, $"*{Extension(FileExtension.text)}", SearchOption.AllDirectories);
            LoadButton.interactable = false;

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

            currentEditFiles = new List<int>();

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

                currentEditFiles.Add(i);
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

        await Task.Delay(1);
    }

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
}
