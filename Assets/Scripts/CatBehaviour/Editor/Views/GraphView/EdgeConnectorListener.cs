using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CatBehaviour.Editor
{
    /// <summary>
    /// 自定义的线连接监听
    /// </summary>
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;
        private List<Edge> m_EdgesToCreate;
        private List<GraphElement> m_EdgesToDelete;

        public EdgeConnectorListener()
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = this.m_EdgesToCreate;
        }
        
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log($"OnDropOutsidePort:{position}");  //鼠标相对于窗口的坐标
            
            ShowNodeCreationMenuFromEdge(edge,position);
        }
        
        
        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            
            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                        m_EdgesToDelete.Add(connection);
                }
            }
            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge)
                        m_EdgesToDelete.Add(connection);
                }
            }
            if (m_EdgesToDelete.Count > 0)
                graphView.DeleteElements(m_EdgesToDelete);
            List<Edge> edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            foreach (Edge edge1 in edgesToCreate)
            {
                graphView.AddElement(edge1);
                edge.input.Connect(edge1);
                edge.output.Connect(edge1);
            }
        }
        
        private void ShowNodeCreationMenuFromEdge(Edge edge, Vector2 position)
        {
            var searchWindowProvider = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            var window = (BehaviourTreeWindow)EditorWindow.focusedWindow;
            searchWindowProvider.Init(window,edge);
            var screenMousePosition = position + EditorWindow.focusedWindow.position.position;
            SearchWindow.Open(new SearchWindowContext(screenMousePosition), searchWindowProvider);
        }
    }
}