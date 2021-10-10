using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Linq;

namespace Unity.DebugWatch.Editor
{

    internal class WatchListView : TreeView
    {

        const int kColumnIndexName = 0;
        const int kColumnIndexValue = 1;
        const int kColumnIndexType = 2;
        const int kColumnIndexContext = 3;
        const int kColumnIndexVisualization = 4;
        static MultiColumnHeaderState CreateState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortingArrowAlignment = TextAlignment.Right,
                    canSort = false,
                    sortedAscending = false,
                    width = 128,
                    minWidth = 32,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Value"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 128,
                    minWidth = 32,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Type"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 64,
                    minWidth = 32,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Context"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 64,
                    minWidth = 32,
                    autoResize = true,
                    allowToggleVisibility = true
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Visualization"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    width = 128,
                    minWidth = 32,
                    autoResize = true,
                    allowToggleVisibility = true
                }
            };
            return new MultiColumnHeaderState(columns);
        }

        public WatchRegistry WatchRegistry;
        public WatchListView(TreeViewState state, WatchRegistry watchRegistry)
            : base(state, new MultiColumnHeader(CreateState()))
        {
            WatchRegistry = watchRegistry;
            rows = new WatchListAdapter(watchRegistry);
            getNewSelectionOverride = GetNewSelection;
            showAlternatingRowBackgrounds = true;

            Reload();
        }
        List<int> GetNewSelection(TreeViewItem item, bool selection, bool shift)
        {
            if (Event.current.control)
            {
                if (IsSelected(item.id))
                {
                    var sel = GetSelection().ToList();
                    sel.Remove(item.id);
                    return sel;
                }
                return GetSelection().Append(item.id).ToList();
            }
            return new List<int> { item.id };
        }
        private readonly WatchListAdapter rows;

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return rows;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var wi = WatchRegistry.Watches[args.item.id];

            string value = null;
            if (wi.StringAccessor == null)
            {
                wi.StringAccessor = WatchRegistry.CreateStringAccessor(wi.Watch, wi.Watch);
            }
            if (wi.StringAccessor != null)
            {
                if (!wi.StringAccessor.TryGet(out value))
                {
                    value = "<?>";
                }
            }
            else
            {
                value = $"<No string accessor for type '{wi.Watch.GetValueType().Name}'>";
            }
            for (int iVisCol = 0; iVisCol != args.GetNumVisibleColumns(); ++iVisCol)
            {
                var i = args.GetColumn(iVisCol);
                var rect = args.GetCellRect(iVisCol);
                switch (i)
                {
                    case kColumnIndexName:
                        DefaultGUI.Label(rect, wi.Watch.GetName(), args.selected, args.focused);
                        break;
                    case kColumnIndexValue:
                        DefaultGUI.Label(rect, value, args.selected, args.focused);
                        break;
                    case kColumnIndexType:
                        DefaultGUI.Label(rect, wi.Watch.GetValueType().Name, args.selected, args.focused);
                        break;
                    case kColumnIndexContext:
                        DefaultGUI.Label(rect, wi.Watch.GetContextName(), args.selected, args.focused);
                        break;
                    case kColumnIndexVisualization:
                        if (WatchRegistry.WatchTypeRegistry.TryGetTypeInfo(wi.Watch.GetValueType(), out var ti))
                        {
                            GUILayout.BeginArea(rect);
                            GUILayout.BeginHorizontal();
                            foreach (var nameVisFact in ti.Visualizers)
                            {
                                bool hasVis = wi.TryGetWatchVisualizer(nameVisFact.Key, out var vis);
                                if (GUILayout.Toggle(hasVis, new GUIContent(nameVisFact.Key, nameVisFact.Value.GetDescription()), "Button"))
                                {
                                    if (!hasVis) wi.TryAddVisualizer(nameVisFact.Value, wi.Watch, out vis);
                                }
                                else
                                {
                                    if (hasVis) wi.TryRemoveWatchVisualizer(vis);
                                }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndArea();
                        }
                        break;
                }
            }

        }

        protected override IList<int> GetAncestors(int id)
        {
            return id == 0 ? new List<int>() : new List<int>() { 0 };
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return new List<int>();
        }

        public IList<int> CurrentSelection;
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (CurrentSelection != null)
            {
                foreach (var i in CurrentSelection)
                {
                    var wi = WatchRegistry.Watches[i];
                    foreach (var v in wi.Visualizers)
                    {
                        v.Item2.SetHighlighted(false);
                    }
                }
            }
            foreach (var i in selectedIds)
            {
                var wi = WatchRegistry.Watches[i];
                foreach (var v in wi.Visualizers)
                {
                    v.Item2.SetHighlighted(true);
                }
            }
            CurrentSelection = selectedIds;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return true;
        }

        public void SelectNothing()
        {
            SetSelection(new List<int>());
        }

        public void TouchSelection()
        {
            SetSelection(
                GetSelection()
                , TreeViewSelectionOptions.RevealAndFrame);
        }

        public void FrameSelection()
        {
            var selection = GetSelection();
            if (selection.Count > 0)
            {
                FrameItem(selection[0]);
            }
        }

    }
}