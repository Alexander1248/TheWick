
using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockyBar : MonoBehaviour
{
    [SerializeField] private Sprite sprite;

    private int _max;
    private Image[] _images;

    public void Initialize(int max, Vector2 anchor, Vector2Int blockSize, Vector2Int blockMargin, int inRowCount)
    {
        _max = max;
        _images = new Image[max];
        Vector2Int block = blockSize + blockMargin;
        for (int i = 0; i < max; i++)
        {
            GameObject obj = new GameObject("D" + i);
            var t = obj.AddComponent<RectTransform>();
            t.SetParent(transform);
            t.sizeDelta = new Vector2(blockSize.x, blockSize.y);
            t.anchorMax = t.anchorMin = anchor;
            var x = (int) ((i % inRowCount + .5f) * block.x * (1 - anchor.x * 2));
            var y = (int) ((i / inRowCount + .5f) * block.y * (1 - anchor.y * 2));
            t.anchoredPosition = new Vector2(x, y);
            _images[i] = obj.AddComponent<Image>();
            _images[i].sprite = sprite;
        }
    }

    public void Set(int value)
    {
        for (int i = 0; i < value; i++)
            _images[i].enabled = true;
        
        for (int i = value; i < _max; i++)
            _images[i].enabled = false;
    }
}
