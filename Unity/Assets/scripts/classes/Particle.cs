using System.Collections.Generic;
using UnityEngine;

namespace MyClasses
{
    public class Particle
    {
        public float mass;
        
        public Vector3 position;
        
        public Vector3 velocity;
        
        public Vector3 total_force;

        public Particle(float mass, Vector3 position, Vector3 velocity, Vector3 total_force)
        {
            this.mass = mass;
            this.position = position;
            this.velocity = velocity;
            this.total_force = total_force;
        }

        public Particle(Particle other)
        {
            mass = other.mass;
            position = other.position;
            velocity = other.velocity;
            total_force = other.total_force;
        }

        public Particle copy()
        {
            return new Particle(this);
        }

        public List<Vector3> getState()
        {
            List<Vector3> x_v = new List<Vector3>();
            x_v.Add(this.position);
            x_v.Add(this.velocity);
            return x_v;
        }

        public void setState(List<Vector3> data)
        {
            this.position = data[0];
            this.velocity = data[1];
        }
    }
}
