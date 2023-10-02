using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class MainControl : MonoBehaviour
{
    public void Scroll2Top() => Scroll.normalizedPosition = Vector2.up;
    public void Scroll2Butt() => Scroll.normalizedPosition = Vector2.zero;

    public async void OnLoad()
    {
        if (!(await Task.FromResult(ParseFilePath())))
            return;

        await Init();

        await ReadCaptions();

        await ConstructMapping();

        captionMap = captionMap.OrderByDescending(x => x.Value.Count).ThenBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        await GenerateCaptionRows();

        Scroll2Top();
    }

    public async void OnSave()
    {
        await DeleteCaptions();
        await WriteCaptions();

        SaveButton.GetComponent<Image>().color = Green;
    }

    public void ToggleDeleteTag(string tag)
    {
        if (captionsToDelete.Contains(tag))
            captionsToDelete.Remove(tag);
        else
            captionsToDelete.Add(tag);
    }
}
