using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace treeProject
{
    public enum TraverseOrder
    {
        InOrder,
        PreOrder,
        PostOrder
    }

    public class ADSTree
    {
        private ADSNode root;

        //TODO idk how to fix this value, so its here now
        private ADSNode overwritenNode;

        sealed class ADSNode
        {
            public ADSNode left;
            public ADSNode right;
            public int key;
            public int cardinality = 1;  //  Increment each time duplicates are added
            public int height = 0;  // Height of this node
        }

        public ADSTree()
        {
        }
        
        // Print the tree in a particular order
        public void printTree(TraverseOrder order)
        {
            breathPrint(root);
        }
        private void breathPrint(ADSNode n)
        {
            int size = 64;
            List<List<ADSNode>> children = new List<List<ADSNode>>();

            children.Add(new List < ADSNode >{ root});
            while (true)
            {
                children.Add(new List<ADSNode>());
                bool cont = false;
                foreach (ADSNode child in children[0])
                {
                    if (child == null)
                    {
                        Console.Write("- ".PadLeft(size / 2).PadRight(size));
                        children[1].Add(null);
                        children[1].Add(null);
                        continue;
                    }
                    cont = true;
                    Console.Write((child.key +"").PadLeft(size / 2).PadRight(size));
                    if (child.left != null)
                        children[1].Add(child.left);
                    else
                        children[1].Add(null);
                    if (child.right != null)
                        children[1].Add(child.right);
                    else
                        children[1].Add(null);
                }
                Console.WriteLine("\n");
                children.RemoveAt(0);
                size /= 2;
                if (!cont)
                    break;
            }


        }
        
        // Inserts a node into the tree and maintains it's balance
        public void insert(int value)
        {
            //Insert to root if null
            if (root == null)
                root = new ADSNode() { key = value };
            //Try to recursively insert node
            else
            {
                ADSNode n = addNode(root, value);

                //This is not neaded for root, so just discard this data
                if (n.height < 0) n.height *= -1;

                root = n;
            }

            //If you overwrite a node in your insertion, put that back in (IDK if this is most efficient, but its what im doing)
            while (overwritenNode != null)
                insert(overwritenNode.key);

        }

        //adds a node to the tree and updates the heights as you go
        private ADSNode addNode(ADSNode n, int val)
        {
            //Increment cardinality and return, they are the same so nothing changes
            if (n.key == val)
            {
                n.cardinality++;
                return n;
            }

            ADSNode side = (val > n.key) ? n.right : n.left;

            //You found your side, then inert it and you good
            if (side == null)
            {
                //If you are inserting a whole node and not just a value, then do that
                if (overwritenNode != null)
                {
                    side = overwritenNode;
                    overwritenNode = null;
                }
                else
                    side = new ADSNode() { key = val };

                //Adds node to propper size
                if (n.key > val) n.left = side;
                else n.right = side;

                return updateHeight(n);
            }

            //It wasnt ment to be inserted here, but it was inserted later. 
            //This returns the new child reference just incase the insert shifted the tree
            ADSNode child = addNode(side, val);

            //Reasign references, they could have chnaged
            if (n.key > val) n.left = child;
            else n.right = child;

            //The sign of the int is used to determine if this node should re-adjust its height values
            if (child.height < 0)
            {
                child.height *= -1;
                
                //Update the node to see if the heights need to be adjusted and return it
                return updateHeight(n);
            }
            
            return n;

        }

        //Takes a node and updates its height returns true if the height changed
        private ADSNode updateHeight (ADSNode n)
        {
            //Get the heights of the side nodes
            int l = (n.left == null) ? 0 : n.left.height + 1;
            int r = (n.right == null) ? 0 : n.right.height + 1;

            //Heavy on one side ? fix the balance
            if (Math.Abs(l - r) == 2)
                return fixBalance(n);
            
            //Save adjusted height
            int adjustedHeight =  Math.Max(l, r);

            //If it is different, change it and indicate it by making it negative
            if (adjustedHeight != n.height)
                n.height = -adjustedHeight;

            //return node with changes
            return n;
            
        }

        //fix balance on a specific node, returns the new head node reference
        private ADSNode fixBalance(ADSNode node)
        {
            //Gets the type of ballance issue;
            string type = heavy(node, false);
            
            if (type == "RR")
                return leftRotate(node);

            else if (type == "LL")
                return rightRotate(node);

            else if (type == "LR")
            {
                node.left = leftRotate(node.left);
                return rightRotate(node);
            }

            else //RL
            {
                node.right = rightRotate(node.right);
                return leftRotate(node);
            }
        }

        //Get type from pos
        private string heavy(ADSNode n, bool done = true)
        { 
            //Gets the heights at a pos
            int l = (n.left == null) ? 0 : n.left.height + 1;
            int r = (n.right == null) ? 0 : n.right.height + 1;

            //Gets which is greater
            string val = (l > r) ? "L" : "R";

            //If your done, return
            if (done)
                return val;

            //If not, add the char and continue
            return val + ((val == "L") ? heavy(n.left) : heavy(n.right));
            
        }

        //Rotate tree at node to the left
        private ADSNode leftRotate(ADSNode node)
        {
            ADSNode ret = node.right;

            //Move parent node to be under child
            overwritenNode = node.right.left;
            node.right.left = node;
            node.right = null;

            //Ret is new head node, ret.left is old head node
            //Update their heights
            quickFixHeight(ret.left);
            quickFixHeight(ret);

            return ret;
        }

        //Same as left rotation, but opposite
        private ADSNode rightRotate(ADSNode node)
        {
            ADSNode ret = node.left;

            overwritenNode = node.left.right;
            node.left.right = node;
            node.left = null;

            quickFixHeight(ret.right);
            quickFixHeight(ret);

            return ret ;
        }

        //Quickly fix the height values of the node, does not care about updating the values of parents
        //This is only called on nodes that I know need updating and dont effect the rest of the tree (ei the moved node in LR or RL balance issue)
        private void quickFixHeight(ADSNode n)
        {
            int l = (n.left == null) ? 0 : n.left.height + 1;
            int r = (n.right == null) ? 0 : n.right.height + 1;

            n.height = Math.Max(l, r);
        }

        
    }
}
