using UnityEngine;
using UnityEngine.UI;

public class ToggleAndOr : MonoBehaviour
{
    public void Toggle(bool v)
    {
        transform.GetChild(1).GetComponent<Text>().text = v ? "And" : "Or";
    }
}
