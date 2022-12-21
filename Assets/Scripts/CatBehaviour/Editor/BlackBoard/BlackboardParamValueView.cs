using CatBehaviour.Runtime;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 黑板参数值视图
    /// </summary>
    public class BlackboardParamValueView: VisualElement
    {
        /// <summary>
        /// 绘制黑板值
        /// </summary>
        public void DrawValue(BBParam bbParam)
        {
            Clear();
            bbParam.CreateGUI(contentContainer,false,null);
            
            IMGUIContainer imguiContainer = new IMGUIContainer(){};
            imguiContainer.onGUIHandler =  (() =>
            {
                bbParam.OnGUI(false,null);
            });
            Add(imguiContainer);
        }
    }
}