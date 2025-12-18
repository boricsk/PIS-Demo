using System.Collections.ObjectModel;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a node in a tree structure, containing a name, a collection of child nodes, and a reference to its
    /// parent node.
    /// </summary>
    /// <remarks>TreeNodeModel can be used to build hierarchical data structures such as organizational
    /// charts, file systems, or other tree-based models. Each node may have zero or more child nodes and an optional
    /// parent node. The Children collection is initialized by default and can be modified to add or remove child nodes
    /// as needed.</remarks>
    public class TreeNodeModel
    {
        public string Name { get; set; }
        public ObservableCollection<TreeNodeModel> Children { get; set; } = new ObservableCollection<TreeNodeModel>();
        public TreeNodeModel Parent { get; set; }

        /// <summary>
        /// Returns the root node of the current tree by traversing parent nodes.
        /// </summary>
        /// <returns>The root <see cref="TreeNodeModel"/> of the tree. If the current node has no parent, returns the current
        /// node.</returns>
        public TreeNodeModel GetRoot()
        {
            var node = this;
            while (node.Parent != null)
            {
                node = node.Parent;
            }
            return node;
        }
    }
}
