
using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockyBar : MonoBehaviour
{
    [SerializeField] private Sprite sprite;

    private int _max;
    private int _prev;
    private Image[] _images;

    public void Initialize(int max, Vector2 anchor, Vector2Int blockSize, Vector2Int blockMargin, int inRowCount, float angle = 0)
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
            var x = (int)((i % inRowCount + .5f) * block.x * (1 - anchor.x * 2));
            var y = (int)((i / inRowCount + .5f) * block.y * (1 - anchor.y * 2));
            t.anchoredPosition = new Vector2(x, y);
            t.rotation = Quaternion.Euler(0, 0, angle);
            _images[i] = obj.AddComponent<Image>();
            _images[i].sprite = sprite;
        }

        _prev = _max;
    }

    public void Set(int value)
    {
        for (var i = _prev; i < value; i++)
            _images[i].enabled = true;
        
        for (var i = value; i < _prev; i++)
            _images[i].enabled = false;

        _prev = value;
    }
}
