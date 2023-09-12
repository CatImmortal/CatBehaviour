using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CatBehaviour.Editor
{
    public partial class BehaviourTreeWindow
    {
        private delegate void GetUndoListDelegate(List<string> undoList, out int index);
        
        private static MethodInfo getUndoListMethod = typeof(Undo).GetMethod("GetUndoList", BindingFlags.Static | BindingFlags.NonPublic);
        private static GetUndoListDelegate getUndoList = (GetUndoListDelegate)Delegate.CreateDelegate(typeof(GetUndoListDelegate), getUndoListMethod);
        private static List<string> undoList = new List<string>();
        
        /// <summary>
        /// 是否允许记录修改
        /// </summary>
        private bool canRecord;
        
        /// <summary>
        /// 是否被修改过
        /// </summary>
        public bool IsModify
        {
            get
            {
                return titleContent.text[titleContent.text.Length - 1] == '*';
            }
            set
            {
                if (IsModify == value)
                {
                    return;
                }
                
                if (value)
                {
                    titleContent.text += '*';
                }
                else
                {
                    titleContent.text = titleContent.text.Remove(titleContent.text.Length - 1);
                }
            }
        }
        
        /// <summary>
        /// 发生Undo Redo时的监听
        /// </summary>
        private void OnUndoRedoPerformed()
        {
            //Debug.Log("OnUndoRedoPerformed");
            if (curBTSO == null)
            {
                return;
            }

            //若当前已撤销到最初的状态，则视为未修改过
            undoList.Clear();
            getUndoList(undoList, out int index);
            IsModify = index != -1;
            
            Refresh(curBTSO);
        }
        
        /// <summary>
        /// 记录修改
        /// </summary>
        public void RecordObject(string undoName)
        {
            if (IsDebugMode)
            {
                return;
            }

            if (!canRecord)
            {
                return;
            }

            Undo.RecordObject(curBTSO, undoName);

            IsModify = true;
        }

        public void BuildNodeId()
        {
            curBTSO.BuildNodeId();
        }

        /// <summary>
        /// 清理所有修改记录
        /// </summary>
        public void ClearAllRecord()
        {
            Undo.ClearAll();
            IsModify = false;
        }
    }
}