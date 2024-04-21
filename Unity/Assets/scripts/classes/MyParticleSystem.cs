using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MyClasses
{
    public class MyParticleSystem
    {
        List<Particle> particles;

        List<Force> forces;

        MyCollider collider;

        float t;

        public MyParticleSystem(List<Particle> particles, List<Force> forces, MyCollider collider, float t)
        {
            this.particles = particles;
            this.forces = forces;
            this.collider = collider;
            this.t = t;
        }

        // copy constructor
        /*
        public MyParticleSystem(MyParticleSystem other)
        {
            applyFunction(other.particles, x => this.particles.Add(new Particle(x)));
            applyFunction(other.forces, x => this.particles.Add(x.clone()));
            collider = new Collider(other.collider.clone());
            t = other.t;
        }*/

        // returns the position and velocity of each particle in a single array
        List<Vector3> getState(List<Particle> particles)
        {
            List<Vector3> xn_vn = new List<Vector3>();
            for(int i=0; i<particles.Count; i++)
            {
                xn_vn.AddRange(particles[i].getState());
            }
            return xn_vn;
        }

        // sets the position and velocity of each particle
        void setState(List<Particle> particles, List<Vector3> data)
        {
            int j = 0;
            for(int i=0; i<particles.Count; i++)
            {
                List<Vector3> xi_vi = new List<Vector3>();
                xi_vi.Add(data[j++]);
                xi_vi.Add(data[j++]);
                particles[i].setState(xi_vi);
            }
        }

        List<Vector3> getDerivative(List<Particle> particles)
        {
            this.clearForces(particles);
            this.applyForces(particles);
            List<Vector3> vn_fmn = new List<Vector3>();
            for(int i=0; i<particles.Count; i++)
            {
                vn_fmn.Add(particles[i].velocity);
                vn_fmn.Add(particles[i].total_force / particles[i].mass);
            }
            return vn_fmn;
        }

        void euler_step(List<Particle> particles, float dt)
        {
            List<Vector3> derivative = this.getDerivative(particles).Select(x => x * dt).ToList();
            List<Vector3> newState = this.getState(particles).Zip(derivative, (ai, bi) => ai+bi).ToList();
            this.setState(particles, newState);
            this.t += dt;
            return;
        }

        void step_custom(List<Particle> particles, out String msg)
        {
            msg = "";
            List<Particle> checkpoint = particles.Select(x => x.copy()).ToList();
            float dt = 0.02f;
            euler_step(particles, dt);
            if(collider == null)
            {
                msg = "no collider";
                return;
            }
            for(int i=0; i<particles.Count; i++)
            {
                bool collision_detected;
                collider.detect_collision(particles[i], out collision_detected);
                if(!collision_detected)
                {
                    continue;
                }
                Particle contact_particle;
                float contact_time;
                collider.collision_response(particles[i], checkpoint[i], out contact_particle, out contact_time);
                euler_step(particles.GetRange(i, 1), dt - dt * contact_time);
                if(particles[i].velocity.magnitude < 0.1)
                {
                    particles[i].velocity = Vector3.zero;
                }
            }
        }

        public void step_custom()
        {
            step_custom(particles, out String msg);
        }

        public Vector3 get_pos(int i)
        {
            return particles[i].position;
        }

        public static void applyFunction<T>(List<T> list, Action<T> func)
        {
            foreach(var item in list)
            {
                func(item);
            }
        }

        void clearForces(List<Particle> particles)
        {
            applyFunction(particles, x => x.total_force = Vector3.zero);
        }

        void applyForces(List<Particle> particles)
        {
            applyFunction(forces, x => x.applyForce(particles));
        }

        public void addParticle(Particle p)
        {
            particles.Add(p);
        }

        public void wraparound(float x, float y, float z)
        {
            foreach(Particle p in particles)
            {
                if(p.position.x < -x)
                {
                    p.position.x += 2 * x;
                }
                if(p.position.x > x)
                {
                    p.position.x += -2 * x;
                }
                if(p.position.y < -y)
                {
                    p.position.y += 2 * y;
                }
                if(p.position.y > y)
                {
                    p.position.y += -2 * y;
                }
                if(p.position.z < -z)
                {
                    p.position.z += 2 * z;
                }
                if(p.position.z > z)
                {
                    p.position.z += -2 * z;
                }
            }
        }
    }

}
