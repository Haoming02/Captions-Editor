using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    public static MainControl Instance { get; private set; }
    void Awake() { Instance = this; }

    public static Color White => Color.white;
    public static Color Pink => new Color32(255, 200, 200, 255);
    public static Color Green => new Color32(200, 255, 200, 255);

    [Header("Fields Panel")]
    [SerializeField]
    private InputField FilePath;
    [SerializeField]
    private InputField FileExtension;
    [SerializeField]
    private Button LoadButton;
    [SerializeField]
    private Button SaveButton;

    [Header("Captions Panel")]
    [SerializeField]
    private GameObject pfRow;
    [SerializeField]
    private Transform Contents;
    [SerializeField]
    private ScrollRect Scroll;

    [Header("Tools Panel")]
    [SerializeField]
    private Toggle Fuzzy;
    [SerializeField]
    private Toggle isAnd;

    private List<GameObject> captionRows;

    private string[] captionFiles;

    private List<string>[] captions;
    private Dictionary<string, List<int>> captionMap;
    private Dictionary<string, int[]> deletionMap;

    private int fileCount;
}
