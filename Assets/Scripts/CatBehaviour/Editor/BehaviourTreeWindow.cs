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
            //设置序列化器
            var types = TypeCache.GetTypesDerivedFrom<IStringSerializer>();
            if (types.Count > 0)
            {
                var stringSerializer = (IStringSerializer)Activator.CreateInstance(types[0]);
                BehaviourTree.StringSerializer = stringSerializer;
            }
            
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/BT_Test.json");
            BehaviourTree bt = null;
            if (json)
            {
               bt = BehaviourTree.Deserialize(json.text);
            }
           
            
            Open(bt);
        }

        public static void Open(BehaviourTree bt)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(bt);
        }

        public void CreateGUI()
        {
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

            //若传入的行为树为空则创建个默认的
            if (bt == null)
            {
                bt = new BehaviourTree();
                bt.RootNode = new RootNode
                {
                    Position = new Vector2(position.position.x + 100, position.position.y + 100),
                    Owner = bt
                };
                bt.AllNodes.Add(bt.RootNode);
            }

            graphView = rootVisualElement.Q<BehaviourTreeGraphView>("graphView");
            graphView.Init(this, bt);
        }

        /// <summary>
        /// 保存行为树
        /// </summary>
        private void Save()
        {
            bt.AllNodes.Clear();
            
            foreach (Node element in graphView.nodes)
            {
                BehaviourTreeNode node = (BehaviourTreeNode) element;
                
                //添加到allNodes里
                bt.AllNodes.Add(node.RuntimeNode);
                node.RuntimeNode.Id = bt.AllNodes.Count;
                    
                //刷新位置
                node.RuntimeNode.Position = node.GetPosition().position;

                //清空父子关系
                node.RuntimeNode.ClearIdAndReference();
            }

            foreach (Node element in graphView.nodes)
            {
                //刷新父子关系
                
                BehaviourTreeNode node = (BehaviourTreeNode) element;
                if (node.inputContainer.childCount == 0)
                {
                    continue;
                }
                
                Port inputPort = (Port)node.inputContainer[0];
                Edge inputEdge = inputPort.connections.FirstOrDefault();
                if (inputEdge == null)
                {
                    continue;
                }
                
                //向父节点添加自身为其子节点
                var parent = (BehaviourTreeNode)inputEdge.output.node;
                parent.RuntimeNode.AddChild(node.RuntimeNode);
                
              
            }
            
            string json = bt.Serialize();

            File.WriteAllText("Assets/BT_Test.json", json);
            AssetDatabase.Refresh();
        }
    }
}

