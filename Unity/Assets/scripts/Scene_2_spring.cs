using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyClasses;
using System;

namespace Scenes
{
    public class Scene_2_spring : MonoBehaviour
    {
        MyParticleSystem ps;

        int flag = 1;

        int created = 0;

        GameObject createGo(String name)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            Collider[] colliders = sphere.GetComponents<Collider>();
            foreach (Collider collider_ in colliders) {
                DestroyImmediate(collider_);
            }
            
            sphere.name = name;
            
            return sphere;
        }

        Particle createP()
        {
            return new Particle(1, Vector3.zero, UnityEngine.Random.insideUnitSphere.normalized * 5, Vector3.zero);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // create force objects
            List<Force> forces = new List<Force>();
            forces.Add(new Gravity(Vector3.down * 2));
            forces.Add(new Drag());

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
                planes.Add(new Plane(normal, gameObject.transform.position + normal * -4));
            }

            // create cube collider
            MyCollider collider = new MyCubeCollider(planes);

            float t = 0;

            GameObject go_1 = createGo("fixed_"+1);
            go_1.transform.position = new Vector3(-2, 0, 0);
            Scene_2_gravity.setColor(go_1, Color.red);

            forces.Add(new Spring(new Particle(1, new Vector3(-3, 3, 0), Vector3.zero, Vector3.zero)));            

            GameObject go_2 = createGo("fixed_"+2);
            go_2.transform.position = new Vector3(2, 0, 0);
            Scene_2_gravity.setColor(go_2, Color.blue);
            
            forces.Add(new Spring(new Particle(1, new Vector3(3, 3, 0), Vector3.zero, Vector3.zero)));

            ps = new MyParticleSystem(new List<Particle>(), forces, collider, t);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
            if(Time.time > created * 0.5 && created < 5)
            {
                createGo(""+flag++);
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