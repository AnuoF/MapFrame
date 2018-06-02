using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using GlobleSituation.Business;
using GlobleSituation.Common;
using MapFrame.Core.Interface;
using DevExpress.XtraTreeList;

namespace GlobleSituation.UI
{
    public partial class LayersControl : XtraUserControl
    {
        /// <summary>
        /// 波束图层组
        /// </summary>
        private Dictionary<string, List<string>> beamDic = new Dictionary<string, List<string>>();
        /// <summary>
        /// 地图框架
        /// </summary>
        private ArcGlobeBusiness globeBusiness = null;
        private GMapControlBusiness mapBusiness = null;

        public LayersControl(ArcGlobeBusiness _globeBusiness, GMapControlBusiness _mapBusiness)
        {
            InitializeComponent();
            InitTree();

            this.globeBusiness = _globeBusiness;
            this.mapBusiness = _mapBusiness;
            EventPublisher.ElementAddEvent += EventPublisher_ElementAddEvent;
        }

        private void EventPublisher_ElementAddEvent(object sender, Model.ElementAddEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ElementName) || string.IsNullOrEmpty(e.LayerName)) return;

            if (e.LayerName == "卫星图层" || e.LayerName == "波束图层")
            {
                // 添加图层
                AppendLayer(e.LayerName, e.ElementName, 0);
            }
        }

        #region TreeList
        private void InitTree()
        {
            this.treeList1.BeginUnboundLoad();
            this.treeList1.AppendNode(new object[] { "卫星波束" }, -1).CheckState = CheckState.Checked;
            this.treeList1.EndUnboundLoad();
        }

        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);

            if (!e.Node.HasChildren && e.Node.Tag != null)
            {
                string nodeName = e.Node.Tag.ToString();

                if (beamDic.ContainsKey(nodeName))
                {
                    foreach (string em in beamDic[nodeName])
                    {
                        // 波束
                        ShowElement(em, e.Node.CheckState, false);
                    }
                }
                else
                    ShowElement(nodeName, e.Node.CheckState, true);    // 卫星
            }
        }

        /// <summary>
        /// 显示隐藏元素
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="check"></param>
        private void ShowElement(string tag, CheckState check, bool isSatellite)
        {
            bool visable = check == CheckState.Checked ? true : false;
            string[] tagArr = tag.Split(new char[] { ':' });
            string layerName = tagArr[0];
            string elementName = tagArr[1];

            if (!isSatellite)   // 波束
            {
                SetGMapElementVisible(layerName, elementName, visable);
                SetGlobeElementVisible(layerName, elementName, visable);
                SetGlobeElementVisible("覆盖图层", elementName + "cover", visable);
                SetGlobeElementVisible("覆盖图层", elementName + "cover_line", visable);
            }
            else    // 卫星
            {
                SetGlobeElementVisible(layerName, elementName, visable);
            }
        }

        /// <summary>
        /// 设置所有子节点的选中状态
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="check">选中状态</param>
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            try
            {
                //显示、隐藏元素
                if (node.HasChildren)
                {
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        if (node.Nodes[i].CheckState != check)
                        {
                            node.Nodes[i].CheckState = check;
                            SetCheckedChildNodes(node.Nodes[i], check);
                        }
                    }
                }
                else
                {
                    if (node.Tag != null)
                    {
                        string nodeName = node.Tag.ToString();

                        if (beamDic.ContainsKey(nodeName))
                        {
                            foreach (string em in beamDic[nodeName])
                            {
                                ShowElement(em, node.CheckState, false);
                            }
                        }
                        else
                            ShowElement(nodeName, node.CheckState, true);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log4Allen.WriteLog(typeof(LayersControl), ex);
            }
        }

        /// <summary>
        /// 设置父节点的选中状态
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="check">选中状态</param>
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            try
            {
                if (node.ParentNode != null)
                {
                    bool b = false;
                    CheckState state;

                    for (int i = 0; i < node.ParentNode.Nodes.Count; i++)
                    {
                        state = (CheckState)node.ParentNode.Nodes[i].CheckState;
                        if (!check.Equals(state))
                        {
                            b = !b;
                            break;
                        }
                    }

                    node.ParentNode.CheckState = b ? CheckState.Indeterminate : check;
                    SetCheckedParentNodes(node.ParentNode, check);
                }
            }
            catch (System.Exception ex)
            {
                Log4Allen.WriteLog(typeof(LayersControl), ex);
            }
        }

        // 添加节点
        private bool AppendLayer(string layerName, string elementName, int nodeIndex)
        {
            bool bSallte = !elementName.Contains("-");

            string sallteName = "";   // 卫星名称
            string beamName = "";     // 波束名称
            string arr = "";          // 卫星+波束号

            if (bSallte)   // 卫星
            {
                sallteName = elementName;
            }
            else           // 波束覆盖
            {
                arr = elementName.Split(new char[] { '_' })[0];
                sallteName = arr.Split(new char[] { '-' })[0];
                beamName = arr.Split(new char[] { '-' })[1];

                string ln = string.Format("{0}:{1}", layerName, elementName);
                if (beamDic.ContainsKey(arr))
                {
                    if (!beamDic[arr].Contains(ln))
                        beamDic[arr].Add(ln);
                }
                else
                {
                    List<string> em = new List<string>();
                    em.Add(ln);
                    beamDic.Add(arr, em);
                }
            }

            string groupName = nodeIndex == 0 ? "波束图层组" : "航迹图层组";

            // 检查是否已经添加相同的对象
            if (treeList1.Nodes[nodeIndex].HasChildren)
            {
                TreeListNodes nodeList = treeList1.Nodes[nodeIndex].Nodes;
                bool bNew = true;

                foreach (TreeListNode n in nodeList)
                {
                    if (!n.GetDisplayText("colLayerName").Contains(sallteName)) continue;  // 查找对应的组
                    if (!n.HasChildren) break;    // 如果没有节点，那就说明是新的节点

                    foreach (TreeListNode tn in n.Nodes)
                    {
                        if (bSallte)  // 找卫星
                        {
                            if (tn.GetDisplayText("colLayerName") == sallteName)
                            {
                                bNew = false;
                                break;
                            }
                        }
                        else    // 找波束\点、线
                        {
                            if (!tn.GetDisplayText("colLayerName").Contains(groupName)) continue;  // 找到波束图层，再找子节点图层
                            if (!tn.HasChildren)
                            {
                                bNew = true;
                                break;
                            }

                            foreach (TreeListNode ttn in tn.Nodes)
                            {
                                if (ttn.GetDisplayText("colLayerName").Contains(beamName))
                                {
                                    bNew = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (bNew)
                {
                    AddNode_Append(layerName, elementName, bSallte, nodeIndex);   // 追加节点
                }
            }
            else
            {
                AddNode_New(layerName, elementName, bSallte, nodeIndex);          // 添加新的节点
            }

            return true;
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="elementName">图元名称</param>
        private void AddNode_New(string layerName, string elementName, bool bSallte, int nodeIndex)
        {
            string sallteName = "";   // 卫星名称
            string beamName = "";     // 波束名称
            string arr = "";

            if (bSallte)
                sallteName = elementName;
            else
            {
                arr = elementName.Split(new char[] { '_' })[0];
                sallteName = arr.Split(new char[] { '-' })[0];
                beamName = arr.Split(new char[] { '-' })[1];
            }

            string groupName = nodeIndex == 0 ? "波束" : "航迹";

            if (bSallte)
            {
                string tn = elementName + "图层组";
                this.treeList1.BeginUnboundLoad();
                TreeListNode node = this.treeList1.AppendNode(new object[] { tn }, nodeIndex);
                node.CheckState = CheckState.Checked;
                treeList1.AppendNode(new object[] { sallteName }, node, string.Format("{0}:{1}", layerName, elementName)).CheckState = CheckState.Checked;   // 卫星图层
                treeList1.AppendNode(new object[] { elementName + groupName + "图层组" }, node).CheckState = CheckState.Checked;
                this.treeList1.EndUnboundLoad();
            }
            else
            {
                string tn = elementName.Split(new char[] { '-' })[0] + "图层组";
                this.treeList1.BeginUnboundLoad();
                TreeListNode node = this.treeList1.AppendNode(new object[] { tn }, nodeIndex);
                node.CheckState = CheckState.Checked;
                TreeListNode bNode = treeList1.AppendNode(new object[] { elementName.Split(new char[] { '-' })[0] + groupName + "图层组" }, node);
                bNode.CheckState = CheckState.Checked;
                treeList1.AppendNode(new object[] { beamName }, bNode, arr).CheckState = CheckState.Checked;   // 波束图层
                this.treeList1.EndUnboundLoad();
            }
        }

        private void AddNode_Append(string layerName, string elementName, bool bSallte, int nodeIndex)
        {
            string sallteName = "";   // 卫星名称
            string beamName = "";     // 波束名称
            string arr = "";

            if (bSallte)
                sallteName = elementName;
            else
            {
                arr = elementName.Split(new char[] { '_' })[0];
                sallteName = arr.Split(new char[] { '-' })[0];
                beamName = arr.Split(new char[] { '-' })[1];
            }
            string tmpName = sallteName + "图层组";
            string groupName = nodeIndex == 0 ? "波束" : "航迹";

            TreeListNodes nodeList = treeList1.Nodes[nodeIndex].Nodes;
            bool bNew1Layer = true;
            foreach (TreeListNode n in nodeList)
            {
                if (n.GetDisplayText("colLayerName").Contains(sallteName))
                {
                    bNew1Layer = false;
                    break;
                }
            }

            if (bNew1Layer)
            {
                this.treeList1.BeginUnboundLoad();
                TreeListNode node = this.treeList1.AppendNode(new object[] { tmpName }, nodeIndex);
                node.CheckState = CheckState.Checked;
                TreeListNode bNode = treeList1.AppendNode(new object[] { sallteName + groupName + "图层组" }, node);   // 波束图层
                bNode.CheckState = CheckState.Checked;
                if (bSallte)
                {
                    treeList1.AppendNode(new object[] { elementName }, node, string.Format("{0}:{1}", layerName, elementName)).CheckState = CheckState.Checked;  // 卫星图层
                }
                else
                {
                    treeList1.AppendNode(new object[] { beamName }, bNode, arr).CheckState = CheckState.Checked;   // 波束图层
                }

                if (treeList1.InvokeRequired)
                {
                    treeList1.Invoke(new Action(delegate
                    {
                        this.treeList1.EndUnboundLoad();
                    }));
                }
                else
                    this.treeList1.EndUnboundLoad();
            }
            else
            {
                foreach (TreeListNode n in nodeList)
                {
                    if (n.GetDisplayText("colLayerName").Contains(sallteName))
                    {
                        this.treeList1.BeginUnboundLoad();
                        if (bSallte)
                        {
                            treeList1.AppendNode(new object[] { elementName }, n, string.Format("{0}:{1}", layerName, elementName)).CheckState = CheckState.Checked;   // 卫星图层
                        }
                        else
                        {
                            TreeListNodes tnList = n.Nodes;
                            foreach (TreeListNode tn in tnList)
                            {
                                if (tn.GetDisplayText("colLayerName").Contains(groupName))
                                {
                                    treeList1.AppendNode(new object[] { beamName }, tn, arr).CheckState = CheckState.Checked;   // 波束图层
                                    break;
                                }
                            }
                        }
                        if (treeList1.InvokeRequired)
                        {
                            treeList1.Invoke(new Action(delegate
                            {
                                this.treeList1.EndUnboundLoad();
                            }));
                        }
                        else
                            this.treeList1.EndUnboundLoad();

                        break;
                    }
                }
            }
        }

        #endregion

        // 闪烁
        private void 闪烁ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetElementProperty(treeList1.FocusedNode, "flash");
        }

        // 设置颜色
        private void 颜色ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetElementProperty(treeList1.FocusedNode, "color");
        }

        // 卫星
        private void cbSatellite_CheckedChanged(object sender, EventArgs e)
        {
            globeBusiness.SetSatelliteLayerVisible(cbSatellite.Checked);
        }

        // 波束
        private void cbBeam_CheckedChanged(object sender, EventArgs e)
        {
            globeBusiness.SetBeamLayerVisible(cbBeam.Checked);
        }

        // 覆盖
        private void cbCover_CheckedChanged(object sender, EventArgs e)
        {
            globeBusiness.SetCoverLayerVisible(cbCover.Checked);
            mapBusiness.SetCoverLayerVisible(cbCover.Checked);
        }

        #region 图元操作

        // 设置图元属性
        private void SetElementProperty(TreeListNode node, string operate)
        {
            try
            {
                if (node == null || node.Tag == null) return;

                string layerName = "";
                string elementName = "";

                bool isSatellite = false;    // 是否是卫星

                string tag = node.Tag.ToString();
                if (!tag.Contains("-"))
                {
                    isSatellite = true;
                    string[] tagArr = tag.Split(new char[] { ':' });
                    layerName = tagArr[0];
                    elementName = tagArr[1];
                }
                else
                {
                    isSatellite = false;
                    layerName = "波束图层";
                    elementName = tag;
                }

                switch (operate)
                {
                    case "flash":
                        if (!isSatellite)
                        {
                            SetGMapElementFlash(layerName, elementName);
                            SetGlobeElementFlash(layerName, elementName);
                            SetGlobeElementFlash("覆盖图层", elementName + "cover");
                            SetGlobeElementFlash("覆盖图层", elementName + "cover_line");
                        }
                        else
                        {
                            SetGlobeElementFlash(layerName, elementName);
                        }

                        break;

                    case "color":
                        using (ColorDialogEx dlg = new ColorDialogEx())
                        {
                            if (dlg.ShowDialog() == DialogResult.OK)
                            {
                                Color color = dlg.GetColor();

                                if (!isSatellite)
                                {
                                    SetGMapElementColor(layerName, elementName, color);
                                    SetGlobeElementColor(layerName, elementName, color, isSatellite);
                                    SetGlobeElementColor("覆盖图层", elementName + "cover", color, isSatellite);
                                    SetGlobeElementColor("覆盖图层", elementName + "cover_line", color, isSatellite);
                                }
                                else
                                {
                                    SetGlobeElementColor(layerName, elementName, color, isSatellite);
                                }
                            }
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Log4Allen.WriteLog(typeof(LayersControl), ex.Message);
            }
        }

        private void SetGlobeElementFlash(string layerName, string elementName)
        {
            var layer = globeBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;

            ele.Flash(!ele.IsFlash, 500);
        }

        private void SetGMapElementFlash(string layerName, string elementName)
        {
            var layer = mapBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;

            ele.Flash(!ele.IsFlash, 500);
        }

        private void SetGlobeElementVisible(string layerName, string elementName, bool visible)
        {
            var layer = globeBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;

            ele.SetVisible(visible);
        }

        private void SetGMapElementVisible(string layerName, string elementName, bool visible)
        {
            var layer = mapBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;
            ele.SetVisible(visible);

            mapBusiness.SetTextVisible(visible, elementName);
        }

        private void SetGlobeElementColor(string layerName, string elementName, Color color, bool isSatellite)
        {
            var layer = globeBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;

            if (isSatellite)
            {
                I3DModel model = ele as I3DModel;
                if (model == null) return;

                model.SetColor(color);
            }
            else
            {
                IMFPolygon polygon = ele as IMFPolygon;
                if (polygon == null) return;
                polygon.SetFillColor(color);
            }
        }

        private void SetGMapElementColor(string layerName, string elementName, Color color)
        {
            var layer = mapBusiness.mapLogic.GetLayer(layerName);
            if (layer == null) return;
            var ele = layer.GetElement(elementName);
            if (ele == null) return;

            IMFCircle circle = ele as IMFCircle;
            if (circle == null) return;
            circle.SetFillColor(color);
        }

        #endregion

        // 鼠标点击事件
        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                treeList1.ContextMenuStrip = null;
                TreeListHitInfo hInfo = treeList1.CalcHitInfo(new Point(e.X, e.Y));
                TreeListNode node = hInfo.Node;
                if (node != null && node.HasChildren == false)
                {
                    treeList1.ContextMenuStrip = contextMenuStrip1;
                }
            }
        }




    }
}
