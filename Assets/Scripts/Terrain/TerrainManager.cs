using System.Collections.Generic;
using UnityEngine;

namespace Confined.Terrain
{
    public class TerrainManager : MonoBehaviour
    {
        [SerializeField] private GameObject LandTerrainObject;
        [SerializeField] private GameObject WaterTerrainObject;

        [SerializeField] private float ScrollSpeed = 50.0f;

        private List<GameObject> children;

        // Change this if the list of terrain is different than 5 child objects
        private int currentTarget = 2;

        void Start()
        {
            var rot = Quaternion.Euler(90, 90, -90);

            children = new List<GameObject>()
            {
                GameObject.Instantiate(LandTerrainObject, new Vector3(-60, 80, 9.15f), rot, transform),
                GameObject.Instantiate(LandTerrainObject, new Vector3(-60, 40, 9.15f), rot, transform),
                GameObject.Instantiate(LandTerrainObject, new Vector3(-60, 0, 9.15f), rot, transform),
                GameObject.Instantiate(LandTerrainObject, new Vector3(-60, -40, 9.15f), rot, transform),
                GameObject.Instantiate(LandTerrainObject, new Vector3(-60, -80, 9.15f), rot, transform)
            };
        }

        void Update()
        {
            // If the current target moves to -20, then move the one at -100 to 60
            if (children[currentTarget].transform.position.y <= -20.0f)
            {
                var temp = children[children.Count - 1];
                children.RemoveAt(children.Count - 1);
                temp.transform.Translate(Vector3.back * 200);
                children.Insert(0, temp);
            }

            // Move all children down
            foreach (var child in children)
            {
                child.transform.Translate(Vector3.forward * Time.smoothDeltaTime * ScrollSpeed);
            }
        }
    }
}