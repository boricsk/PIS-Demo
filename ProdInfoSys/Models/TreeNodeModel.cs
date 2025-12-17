using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Models
{
    /// <summary>
    /// Represents a node in a hierarchical tree structure.
    /// </summary>
    /// <remarks>Each node contains a name and a collection of child nodes, allowing for the creation of 
    /// nested tree structures. This class is commonly used for representing hierarchical data  such as file systems,
    /// organizational charts, or menu structures.</remarks>
    public class TreeNodeModel
    {
        public string Name {get; set; }        
        public ObservableCollection<TreeNodeModel> Children { get; set; } = new ObservableCollection<TreeNodeModel>();
        public TreeNodeModel Parent {get; set; }

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
