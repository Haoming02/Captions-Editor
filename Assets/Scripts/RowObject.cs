using UnityEngine;
using UnityEngine.UI;

public class RowObject : MonoBehaviour
{
    private string Caption;
    private int[] Index;

    private int cachedIndex;

    public void Init(string tag, int[] index)
    {
        Caption = tag;
        Index = index;
        cachedIndex = -1;

        transform.GetChild(0).GetComponent<Text>().text = $"{Caption}: {Index.Length}";
    }

    public void DeleteTag()
    {
        MainControl.Instance.ToggleDeleteTag(Caption);

        if (cachedIndex == -1)
        {
            cachedIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();
            transform.GetComponent<Image>().color = MainControl.Pink;
        }
        else
        {
            transform.SetSiblingIndex(cachedIndex);
            cachedIndex = -1;
            transform.GetComponent<Image>().color = MainControl.White;
        }
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

    public int[] Filter(string filter, bool fuzzy)
    {
        if (fuzzy ? Caption.Contains(filter) : Caption == filter)
            return Index;
        else
            return new int[] { };
    }

    public bool Filter(string[] filters, bool fuzzy, bool isAnd)
    {
        bool flag = isAnd;

        foreach (string filter in filters)
        {
            if (isAnd)
            {
                if (!(fuzzy ? Caption.Contains(filter) : Caption == filter))
                {
                    flag = false;
                    break;
                }
            }
            else
            {
                if (fuzzy ? Caption.Contains(filter) : Caption == filter)
                {
                    flag = true;
                    break;
                }
            }
        }

        return flag;
    }
}
