using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// <summary>
    /// 行为树窗口
    /// </summary>
    public partial class BehaviourTreeWindow : EditorWindow
    {
        private DropdownField dropdownField;
        private ObjectField objField;
        private Button btnSave;
        private Button btnSaveAs;
        private Button btnNew;
        
        private SplitView splitView;
        public InspectorView NodeInspector;
        public BehaviourTreeGraphView GraphView;

        private string assetPath;

        private BehaviourTreeSO curBTSO;
        private BehaviourTree curBT;
        
        
        /// <summary>
        /// 是否为调试模式
        /// </summary>
        public bool IsDebugMode { get; private set; }

        [MenuItem("CatBehaviour/打开行为树窗口")]
        public static void Open()
        {
            OpenFromAssetPath(null);
        }
        
        /// <summary>
        /// 双击行为树SO打开窗口
        /// </summary>
        [OnOpenAsset(1)]
        public static bool OnOpenAssets(int id, int line)
        {
            if (EditorUtility.InstanceIDToObject(id) is BehaviourTreeSO btSO)
            {
                string assetPath = AssetDatabase.GetAssetPath(btSO);
                OpenFromAssetPath(assetPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通过行为树SO资源路径打开窗口
        /// </summary>
        private static void OpenFromAssetPath(string assetPath)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(false);
            window.RefreshFromAssetPath(assetPath);
        }

        /// <summary>
        /// 通过调试器打开窗口
        /// </summary>
        public static void OpenFromDebugger(BehaviourTree debugBT)
        {
            BehaviourTreeWindow window = CreateWindow<BehaviourTreeWindow>("行为树窗口");
            window.Init(true);
            window.RefreshFromDebugger(debugBT);
        }


        private void OnEnable()
        {
            VisualElement root = rootVisualElement;
            
            var visualTree = Resources.Load<VisualTreeAsset>("UXML/BehaviourTreeWindow");
            visualTree.CloneTree(root);
            
            var styleSheet = Resources.Load<StyleSheet>("USS/BehaviourTreeWindow");
            root.styleSheets.Add(styleSheet);
            
            //下拉列表
            dropdownField = rootVisualElement.Q<DropdownField>("dropdownBTSO");
            dropdownField.RegisterValueChangedCallback((evt =>
            {
                if (string.IsNullOrEmpty(evt.newValue))
                {
                    return;
                }

                ClearAllRecord();
                
                if (!IsDebugMode)
                {
                    string path = evt.newValue.Replace('\\', '/');
                    RefreshFromAssetPath(path);
                }
                else
                {
                    var debugBT = BTDebugger.Get(evt.newValue);
                    RefreshFromDebugger(debugBT);
                }
            }));
            
            objField = rootVisualElement.Q<ObjectField>("ObjBTSO");
            
            btnSave = root.Q<Button>("btnSave");
            btnSave.clicked += () =>
            {
                Save(false);
            };
            
            btnSaveAs = root.Q<Button>("btnSaveAs");
            btnSaveAs.clicked += () =>
            {
                Save(true);
            };
            
            btnNew = root.Q<Button>("btnNew");
            btnNew.clicked += New;
            
            splitView = root.Q<SplitView>();
            NodeInspector = root.Q<InspectorView>("inspector");
            GraphView = rootVisualElement.Q<BehaviourTreeGraphView>("graphView");
            GraphView.Init(this);

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            
            //还原数据 防止编译后窗口数据丢失
            if (!string.IsNullOrEmpty(assetPath))
            {
                dropdownField.SetValueWithoutNotify(assetPath.Replace('/','\\'));
                objField.value = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(assetPath);
            }
            if (curBTSO != null)
            {
                Refresh(curBTSO.Deserialize());
            }
        }
        
        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            ClearAllRecord();
            
            //记录一些信息 用于编译后的窗口还原
            curBTSO.InspectorWidth = splitView.Q("left").layout.width;
            curBTSO.ViewportRect = new Rect(GraphView.viewTransform.position, GraphView.viewTransform.scale);
            curBTSO.BlackBoardRect = GraphView.BlackboardView.GetPosition();
        }
        


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init(bool isDebugMode)
        {
            IsDebugMode = isDebugMode;
            RefreshDropDown(isDebugMode);

            if (isDebugMode)
            {
                //调试状态下 删掉SO引用 保存 另存为和新建按钮
                objField.RemoveFromHierarchy();
            }
        }
        
        /// <summary>
        /// 刷新下拉菜单
        /// </summary>
        private void RefreshDropDown(bool isDebugMode)
        {
            List<string> paths;
            if (!isDebugMode)
            {
                paths = AssetDatabase.FindAssets($"t:{nameof(BehaviourTreeSO)}")
                    .Select(((s, i) => AssetDatabase.GUIDToAssetPath(s).Replace('/','\\'))).ToList();
            }
            else
            {
                paths = BTDebugger.BTInstanceDict.Keys.ToList();
            }
            dropdownField.choices = paths.ToList();
        }
        
        /// <summary>
        /// 使用行为树SO路径刷新
        /// </summary>
        private void RefreshFromAssetPath(string assetPath)
        {
            this.assetPath = assetPath;
            BehaviourTree bt = null;
            
            //加载行为树SO 反序列化出行为树来
            if (!string.IsNullOrEmpty(assetPath))
            {
                curBTSO = AssetDatabase.LoadAssetAtPath<BehaviourTreeSO>(assetPath);
                if (curBTSO != null)
                {
                    bt = curBTSO.Deserialize();
                }
                
                dropdownField.SetValueWithoutNotify(assetPath.Replace('/','\\'));
                objField.value = curBTSO;
            }


            Refresh(bt);
        }

        /// <summary>
        /// 使用调试用行为树对象刷新
        /// </summary>
        private void RefreshFromDebugger(BehaviourTree debugBT)
        {
            int index = dropdownField.choices.IndexOf(debugBT.DebugName.Replace('/','\\'));
            dropdownField.index = index;
            
            Refresh(debugBT);
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        private void Refresh(BehaviourTree bt)
        {
            //若传入的行为树为空则创建个默认的
            if (bt == null)
            {
                bt = new BehaviourTree();
                bt.RootNode = new RootNode
                {
                    Position = new Vector2(GraphView.contentContainer.layout.position.x + 500, GraphView.contentContainer.layout.position.y + 100),
                    Owner = bt
                };
                bt.AllNodes.Add(bt.RootNode);
                
                curBTSO = CreateInstance<BehaviourTreeSO>();
                curBTSO.Serialize(bt);
            }
            
            curBT = bt;
            //设置节点属性面板的宽度
            if (curBTSO.InspectorWidth != 0)
            {
                splitView.fixedPaneInitialDimension = curBTSO.InspectorWidth;
            }
            else
            {
                splitView.fixedPaneInitialDimension = 420;
            }

            //GraphView刷新期间 不记录任何修改
            canRecord = false;
            GraphView.Refresh(curBTSO,curBT);
            canRecord = true;
        }
        
        /// <summary>
        /// 节点被点击时的回调
        /// </summary>
        public void OnNodeClick(NodeView nodeView)
        {
            NodeInspector.DrawInspector(nodeView);
        }
        
        /// <summary>
        /// 保存行为树
        /// </summary>
        private void Save(bool isSaveAs)
        {
            if (string.IsNullOrEmpty(assetPath) || isSaveAs)
            {
                //是新建的 或者另存为
                string path = EditorUtility.SaveFilePanelInProject("选择保存位置", "BehaviourTree", "asset", "");
                if (string.IsNullOrEmpty(path))
                {
                    Debug.Log("取消保存");
                    return;
                }

                AssetDatabase.DeleteAsset(path);
                
                assetPath = path;
                curBTSO = CreateInstance<BehaviourTreeSO>();
                curBTSO.Serialize(curBT);
                
                AssetDatabase.CreateAsset(curBTSO,path);
                
            }
            else
            {
                if (!EditorUtility.DisplayDialog("提示","确认保存？","是","否"))
                {
                    return;
                }

                curBTSO.Serialize(curBT);
            }
            
            
            //记录属性面板宽度
            curBTSO.InspectorWidth = splitView.Q("left").layout.width;
            
            //记录节点图位置
            curBTSO.ViewportRect = new Rect(GraphView.viewTransform.position, GraphView.viewTransform.scale);

            //记录黑板位置
            curBTSO.BlackBoardRect = GraphView.BlackboardView.GetPosition();
            
            //保存SO
            EditorUtility.SetDirty(curBTSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            RefreshDropDown(false);
            RefreshFromAssetPath(assetPath);
            
            
            Debug.Log($"保存行为树成功，路径:{assetPath}");
        }

        /// <summary>
        /// 新建行为树
        /// </summary>
        private void New()
        {
            assetPath = null;
            curBTSO = null;
            curBT = null;
            dropdownField.index = -1;
            objField.value = null;
            RefreshFromAssetPath(null);
        }
        

    }
}

