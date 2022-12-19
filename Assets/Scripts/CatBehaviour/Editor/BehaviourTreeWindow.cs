using System;
using System.IO;
using System.Linq;
using CatBehaviour.Editor;
using CatBehaviour.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CatBehaviour.Editor
{
    public class BehaviourTreeWindow : EditorWindow
    {
        private string assetPath;
        private BehaviourTree bt;
        
        private Label labelAssetPath;
        private InspectorView inspector;
        private BehaviourTreeGraphView graphView;

        [MenuItem("CatBehaviour/打开行为树窗口")]
        public static void Open()
        {
            Open(null);
        }
        
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BehaviourTreeSO btSO)
            {
                string assetPath = AssetDatabase.GetAssetPath(btSO);
                Open(assetPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 打开行为树窗口
        /// </summary>
        private static void Open(string assetPath)
        {
            //设置序列化器
            var types = TypeCache.GetTypesDerivedFrom<IStringSerializer>();
            if (types.Count > 0)
            {
                var stringSerializer = (IStringSerializer)Activator.CreateInstance(types[0]);
                BehaviourTree.StringSerializer = stringSerializer;
            }
            
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(assetPath);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    "Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uxml");
            visualTree.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/CatBehaviour/Editor/BehaviourTreeWindow.uss");
            root.styleSheets.Add(styleSheet);

            labelAssetPath = rootVisualElement.Q<Label>("labelAssetPath");
            
            var btnSave = root.Q<Button>("btnSave");
            btnSave.clicked += Save;

            inspector = root.Q<InspectorView>("inspector");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(string assetPath)
        {
            this.assetPath = assetPath;
            
           
            if (!string.IsNullOrEmpty(assetPath))
            {
                labelAssetPath.text = $"文件名:{assetPath}";
                
                var btSO = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(assetPath);
                if (btSO != null)
                {
                    if (!string.IsNullOrEmpty(btSO.StringData))
                    {
                        bt = BehaviourTree.Deserialize(btSO.StringData);
                    }else if (btSO.BinaryData != null && btSO.BinaryData.Length > 0)
                    {
                        
                    }
                }
            }
            else
            {
                labelAssetPath.text = $"文件名:newFile";
            }

            //若反序列化得到的行为树为空则创建个默认的
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
        /// 节点被点击时的回调
        /// </summary>
        public void OnNodeClick(BehaviourTreeNode node)
        {
            inspector.DrawInspector(node);
        }
        
        /// <summary>
        /// 保存行为树
        /// </summary>
        private void Save()
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                assetPath = EditorUtility.SaveFilePanelInProject("选择保存位置", "BehaviourTree","asset", "aaa");
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.Log("取消保存");
                    return;
                }
                
                
            }
            else
            {
                if (!EditorUtility.DisplayDialog("提示","确认保存？","是","否"))
                {
                    return;
                }

                
            }
            
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
            
            string json = bt.SerializeToString();
            if (json == null)
            {
                return;
            }
            var btSO = CreateInstance<BehaviourTreeSO>();
            btSO.StringData = json;

            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.CreateAsset(btSO,assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            labelAssetPath.text = $"文件名:{assetPath}";
            
            Debug.Log($"保存行为树成功，路径:{assetPath}");
        }


    }
}

