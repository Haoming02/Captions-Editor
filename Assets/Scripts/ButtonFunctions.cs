using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    public void Load() => StartCoroutine(LoadCaptions());
    public void Save() => StartCoroutine(SaveCaptions());

    public void Top() => Scroll.normalizedPosition = Vector2.up;
    public void Down() => Scroll.normalizedPosition = Vector2.zero;

    public void ToggleIsAnd(bool v)
    {
        isAnd.transform.GetChild(1).GetComponent<Text>().text = v ? "And" : "Or";
    }

    public void ToggleDeleteTag(string tag, ref int[] index)
    {
        if (deletionMap.ContainsKey(tag))
            deletionMap.Remove(tag);
        else
            deletionMap[tag] = index;
    }
}
