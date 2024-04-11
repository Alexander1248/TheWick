using UnityEditor;
using UnityEngine.UIElements;

namespace SentenceGraph
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> { }

        public SplitView()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SentenceGraph/SentenceTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }
    }
}