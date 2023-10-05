using System.Collections.Generic;
using UnityEngine;

public partial class MainControl : MonoBehaviour
{
    public static MainControl Instance { get; private set; }
    void Awake() { Instance = this; }

    public static Color White => Color.white;
    public static Color Pink => new Color32(255, 200, 200, 255);
    public static Color Green => new Color32(200, 255, 200, 255);

    private string[] captionFiles;
    private Dictionary<string, List<int>> folderMap;

    private int[] filteredFiles;

    private List<int> currentFolders;
    private List<int> currentFiles;
    private List<int> currentFilters;
    private List<string> captionsToDelete;

    private List<string>[] captions;
    private Dictionary<string, List<int>> captionMap;

    private List<GameObject> captionRows;

    private int fileCount;
    private int captionCount => captionMap.Keys.Count;
}
