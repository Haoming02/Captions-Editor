using UnityEngine.UI;
using UnityEngine;

public partial class MainControl : MonoBehaviour
{
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
    private GameObject toolsContent;

    [Header("File Filter")]
    [SerializeField]
    private Toggle fleIsAnd;
    [SerializeField]
    private Toggle fleFuzzy;

    [Header("Caption Filter")]
    [SerializeField]
    private Toggle cptIsAnd;
    [SerializeField]
    private Toggle cptFuzzy;
}
