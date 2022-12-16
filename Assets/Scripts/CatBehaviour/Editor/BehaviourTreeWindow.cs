using System;
using System.IO;
using System.Linq;
using CatBehaviour.Editor;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CatBehaviour.Editor
{
    public class BehaviourTreeWindow : EditorWindow
    {
        private BehaviourTree bt;
        private BehaviourTreeGraphView graphView;

        [MenuItem("CatBehaviour/打开行为树窗口")]
        public static void Open()
        {
            Open(null);
        }

        public static void Open(BehaviourTree bt)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(bt);
        }

        public void CreateGUI()
        {
            //设置序列化器
            var types = TypeCache.GetTypesDerivedFrom<IStringSerializer>();
            if (types.Count > 0)
            {
                var stringSerializer = (IStringSerializer)Activator.CreateInstance(types[0]);
                BehaviourTree.StringSerializer = stringSerializer;
            }

            VisualElement root = rootVisualElement;

            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uxml");
            visualTree.CloneTree(root);

            var btnSave = root.Q<Button>("btnSave");
            btnSave.clicked += Save;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(BehaviourTree source)
        {
            bt = source;

            //若传入的行为树为空则创建
            if (bt == null)
            {
                bt = new BehaviourTree();
                bt.RootNode.Position = new Vector2(position.position.x + 100, position.position.y + 100);
            }

            graphView = rootVisualElement.Q<BehaviourTreeGraphView>("graphView");
            graphView.Init(this, bt);
        }

        /// <summary>
        /// 保存行为树
        /// </summary>
        private void Save()
        {
            foreach (Node element in graphView.nodes)
            {
                //遍历所有节点图中的行为树节点 刷新位置信息 重建RuntimeNode间的父子关系
                if (element is BehaviourTreeNode node)
                {
                    node.RuntimeNode.Position = node.GetPosition().position;

                    node.RuntimeNode.ParentNode = null;
                    node.RuntimeNode.ClearChild();
                    Debug.Log($"当前节点：{node.RuntimeNode.GetType().Name}");

                    //向父节点的连接
                    if (node.inputContainer.childCount > 0)
                    {
                        Port inputPort = (Port)node.inputContainer[0];
                        Edge inputEdge = inputPort.connections.FirstOrDefault();
                        if (inputEdge != null)
                        {
                            var parent = (BehaviourTreeNode)inputEdge.output.node;
                            Debug.Log($"父节点：{parent.RuntimeNode.GetType().Name}");

                            parent.RuntimeNode.AddChild(node.RuntimeNode);
                        }
                    }
                }
            }
            
            string json = bt.Serialize();

            File.WriteAllText("Assets/BT_Test.json", json);
            AssetDatabase.Refresh();
        }
    }
}

