using UnityEditor;
using UnityEngine;

namespace Andre.AI.BehaviourTree.Scriptable
{
    [CreateAssetMenu(fileName = "New Tree", menuName = "AI/Behaviour Tree/SO_BehaviourTree")]
    public class SO_BehaviourTree : ScriptableObject
    {
        [SerializeField]
        public SO_Node root;

        public void Evaluate()
        {
            root.Evaluate();
        }

        public void Initialize(GameObject obj = null)
        {
            if(obj !=null)
                root.Initialize(obj);
        }

        public  void OnBTDisable()
        {
            if (root == null)
                return;
            root.OnBTDisable();
        }

        public T Create<T>(string name)
    where T : SO_Node
        {
            T node = CreateInstance<T>();
            node.name = name;
            return node;
        }

        public void AddNode(SO_Node node)
        {
            node.SetNodeStyle(node);
            root = node;
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }

        public void AddNode(SO_Node node, Vector2 pos)
        {
            node.SetNodeStyle(node);
            node.square.position = pos;
            root = node;
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
        }


        public bool ContainsName(string name)
        {
            if (root == null)
                return false;
            return root.name == name;
        }

        public void RemoveNode(string name)
        {
            if (root == null)
                return;
            root.DestroyNode(root, true);
            root = null;
        }
    }
}