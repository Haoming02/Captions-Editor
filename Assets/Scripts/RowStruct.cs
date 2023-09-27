using UnityEngine;
using UnityEngine.UI;

public class RowStruct : MonoBehaviour
{
    private new string tag;
    private int Times;
    private int[] Index;

    public void Init(string t, int time, int[] index)
    {
        this.tag = t;
        this.Times = time;
        this.Index = index;

        transform.GetChild(0).GetComponent<Text>().text = $"{tag}: {Times}";
    }

    public void DeleteTag()
    {
        MainControl.Instance.DeleteTag(this.tag, this.gameObject);
        Destroy(this.gameObject);
    }

    public bool Filter(int index)
    {
        foreach (int i in Index)
        {
            if (i == index)
                return true;
        }

        return false;
    }

    public bool Filter(string filter, bool wild = false)
    {
        if (wild)
            return tag.Contains(filter);
        else
            return tag == filter;
    }
}
