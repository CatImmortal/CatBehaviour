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
    public class BlackBoardView : PinnedElementView
    {
        private const string exposedParameterViewStyle = "USS/BlackBoardView";
        
        public BlackBoardView()
        {
            var style = Resources.Load<StyleSheet>(exposedParameterViewStyle);
            if (style != null)
                styleSheets.Add(style);
        }

        public override void Init(BehaviourTreeGraphView graphView)
        {
            base.Init(graphView);
            base.title = "黑板";
            Scrollable = true;  //可滚动
            SetPosition(new Rect(10, 30, 350, 300));
            header.Add(new Button(OnAddClicked)
            {
                text = "+"
            });

            graphView.OnBlackBoardChanged += UpdateContent;
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
                string typeName = GetBBParamTypeName(type);
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
        /// 获取黑板参数的类型名
        /// </summary>
        private string GetBBParamTypeName(Type type)
        {
            string name = type.Name;
            name = name.Replace("BBParam", "");
            return ObjectNames.NicifyVariableName(name);
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

            foreach (var pair in graphView.BT.BlackBoard.ParamDict)
            {
                Type type = pair.Value.GetType();
                string typeName = GetBBParamTypeName(type);
                var keyView = new BlackboardParamKeyView(graphView, pair.Value, pair.Key, typeName);
                var valueView = new BlackboardParamValueView();
                valueView.DrawValue(pair.Value);
                var row = new BlackboardRow(keyView,valueView);
                row.expanded = true;

                content.Add(row);
            }
        }
    }
}