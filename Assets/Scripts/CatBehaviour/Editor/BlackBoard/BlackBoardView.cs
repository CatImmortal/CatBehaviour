using System;
using System.Collections.Generic;
using System.Linq;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 黑板界面
    /// </summary>
    public class BlackBoardView : PinnedElementView
    {
        private const string exposedParameterViewStyle = "USS/BlackBoardView";
        
        public BlackBoardView()
        {
            var style = Resources.Load<StyleSheet>(exposedParameterViewStyle);
            if (style != null)
                styleSheets.Add(style);
        }

        public override void Init(BehaviourTreeWindow window)
        {
            base.Init(window);
            
            base.title = "黑板";
            Scrollable = true;  //可滚动
            SetPosition(new Rect(10, 30, 250, 250));
            header.Add(new Button(OnAddClicked)
            {
                text = "+"
            });
            graphView.OnBlackBoardChanged += UpdateContent;
        }
        
        public override void Refresh()
        {
            if (graphView.BT.BlackBoard.Position != default)
            {
                SetPosition(graphView.BT.BlackBoard.Position);
            }
            UpdateContent();
        }

        /// <summary>
        /// 点击了黑板上的加号
        /// </summary>
        private void OnAddClicked()
        {
            var paramType = new GenericMenu();

            foreach (Type type in GetBlackBoardParamTypes())
            {
                string typeName = BBParam.GetBBParamTypeName(type);
                paramType.AddItem(new GUIContent(typeName),false,() =>
                {
                    string uniqueKey = $"New {typeName}";
                    uniqueKey = GetUniqueBlackBoardKey(uniqueKey);
                    graphView.AddBlackBoardParam(uniqueKey,type);
                });
            }
            
            paramType.ShowAsContext();
        }
        
        
        /// <summary>
        /// 获取黑板参数Type
        /// </summary>
        private List<Type> GetBlackBoardParamTypes()
        {
            List<Type> types = new List<Type>();

            foreach (Type type in TypeCache.GetTypesDerivedFrom<BBParam>())
            {
                if (type.IsGenericType || type.IsAbstract)
                {
                    continue;
                }
                types.Add(type);
            }
            
            return types;
        }
        
        
        /// <summary>
        /// 获取唯一的黑板Key
        /// </summary>
        private string GetUniqueBlackBoardKey(string key)
        {
            string uniqueKey = key;
            int i = 0;
            var keys = graphView.BT.BlackBoard.ParamDict.Keys;
            while (keys.Any(e => e == key))
            {
                key = uniqueKey + " " + i++;
            }
            return key;
        }

        
        /// <summary>
        /// 更新黑板内容
        /// </summary>
        private void UpdateContent()
        {
            content.Clear();

            var keyList = graphView.BT.BlackBoard.ParamDict.Keys.ToList();
            keyList.Sort();

            foreach (string key in keyList)
            {
                BBParam value = graphView.BT.BlackBoard.ParamDict[key];
                Type type = value.GetType();
                string typeName = BBParam.GetBBParamTypeName(type);
                var keyView = new BlackboardParamKeyView(graphView, value, key, typeName);
                var valueView = new BlackboardParamValueView();
                valueView.DrawValue(value);
                var row = new BlackboardRow(keyView,valueView);
                row.expanded = true;

                content.Add(row);
            }
            
        }
        
        /// <summary>
        /// 设置位置并记录
        /// </summary>
        public void SetPosAndRecord(Rect newPos)
        {
            window.RecordObject($"SetPosition BlackBoard");
            
            SetPosition(newPos);
            
            graphView.BT.BlackBoard.Position = newPos;
            window.ClonedBTSO.BlackBoardRect = newPos;
        }
    }
}