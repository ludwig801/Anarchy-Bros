using UnityEngine;
using System.Collections.Generic;

namespace AnarchyBros
{
    public class NodeManager : MonoBehaviour
    {
        public static NodeManager Instance;

        void Awake()
        {
            Instance = this;
        }

        static Transform NextStep(Node current, Node objective)
        {
            // TODO : make algorithm to get next step
            return null;
        }

        static List<Transform> GetPath(Node begin, Node objective)
        {
            // TODO
            return null;
        }
    }
}