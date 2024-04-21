using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyClasses;

namespace Scenes
{
    public class Scene_1 : MonoBehaviour
    {
        MyParticleSystem ps;

        int flag = 1;

        int created = 0;

        GameObject createGo(int i)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            Collider[] colliders = sphere.GetComponents<Collider>();
            foreach (Collider collider_ in colliders) {
                DestroyImmediate(collider_);
            }
            
            sphere.name = ""+(i);
            
            return sphere;
        }

        Particle createP()
        {
            return new Particle(1, Vector3.zero, UnityEngine.Random.insideUnitSphere.normalized * 5, Vector3.zero);
        }

        public static MyCollider createMyCubeCollider(Vector3 cube_center)
        {
            // create planes that define a cube
            Vector3[] n = {
                Vector3.left, Vector3.right, 
                Vector3.down, Vector3.up, 
                Vector3.back, Vector3.forward
            };
            List<Vector3> normals = new List<Vector3>(n);
            List<Plane> planes = new List<Plane>();
            foreach(Vector3 normal in normals)
            {
                planes.Add(new Plane(normal, cube_center + normal * -4));
            }

            // create cube collider
            return new MyCubeCollider(planes);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // create force objects
            List<Force> forces = new List<Force>();
            forces.Add(new Gravity(Vector3.down * 2));
            forces.Add(new Drag());

            // create cube collider
            MyCollider collider = createMyCubeCollider(gameObject.transform.position);

            float t = 0;

            ps = new MyParticleSystem(new List<Particle>(), forces, collider, t);

            // create particles
            /*
            while(flag < 5)
            {
                //create(flag++);
                //ps.addParticle(new Particle(1, Vector3.zero, Random.insideUnitSphere.normalized, Vector3.zero));
            }
            */

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
            if(Time.time > created * 0.5 && created < 20)
            {
                createGo(flag++);
                ps.addParticle(createP());
                created += 1;
            }
            ps.step_custom();
            for(int i=0; i<flag-1; i++)
            {
                GameObject go = GameObject.Find(""+(i+1));
                go.transform.position = ps.get_pos(i);
                //MonoBehaviour.print(ps.get_pos(i));
            }
        }
        
    }
}