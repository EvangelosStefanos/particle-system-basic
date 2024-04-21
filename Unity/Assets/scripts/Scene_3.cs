using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyClasses;
using System;

namespace Scenes
{
    public class Scene_3 : MonoBehaviour
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
            return new Particle(1, UnityEngine.Random.insideUnitSphere.normalized * 2, UnityEngine.Random.insideUnitSphere.normalized * 0, Vector3.zero);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            // create force objects
            List<Force> forces = new List<Force>();
            forces.Add(new Gravity(Vector3.down * 2));
            forces.Add(new Drag());

            // create sphere collider
            MyCollider collider = new MySphereCollider(Vector3.zero, 4f);

            float t = 0;

            List<Particle> particles = new List<Particle>();

            forces.Add(new GravityNt(particles, "repell"));

            ps = new MyParticleSystem(particles, forces, collider, t);

            while(created < 8)
            {
                createGo(""+flag++);
                ps.addParticle(createP());
                created += 1;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
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