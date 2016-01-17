using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MultiOption : MonoBehaviour
{
    public RectTransform Content;
    public List<Text> Options;
    public int Value;
    public Color ColorSelected, ColorDefault;

    void Start()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Button option = Content.GetChild(i).GetComponent<Button>();
            int x = i;
            option.onClick.AddListener(() =>
            {
                ChangeOption(x);
            });

            Options.Add(option.GetComponent<Text>());
            Options[i].color = ColorDefault;
        }

        Value = 0;
        ChangeOption(0);
    }

    public void ChangeOption(int optionIndex)
    {
        Options[Value].color = ColorDefault;
        Options[optionIndex].color = ColorSelected;
        Value = optionIndex;
    }
}
