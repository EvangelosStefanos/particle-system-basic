using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyClasses
{
    public abstract class Force
    {
        public abstract void applyForce(List<Particle> particles);
    }

    public class Gravity : Force
    {
        Vector3 gravitational_constant;
        public Gravity(Vector3 g)
        {
            gravitational_constant = g;
        }
        public override void applyForce(List<Particle> particles)
        {
            Functions.applyFunction(particles, x => x.total_force += x.mass * gravitational_constant);
        }
    }

    public class Drag : Force
    {
        float coefficient_of_drag;
        public Drag(float k = 0.1f)
        {
            coefficient_of_drag = k;
        }
        public override void applyForce(List<Particle> particles)
        {
            Functions.applyFunction(particles, x => x.total_force -= coefficient_of_drag * x.velocity);
        }
    }
    
    public class Spring : Force
    {
        Particle one_side;
        
        float spring_constant;

        float rest_length;

        float damping_constant;

        public Spring(
            Particle one_side, float spring_constant = 0.3f, 
            float rest_length = 0, float damping_constant = 0
        )
        {
            this.one_side = one_side;
            this.spring_constant = spring_constant;
            this.rest_length = rest_length;
            this.damping_constant = damping_constant;
        }

        public override void applyForce(List<Particle> particles)
        {
            foreach(Particle other_side in particles)
            {
                Vector3 x_diff = one_side.position - other_side.position;
                Vector3 x_div = x_diff / x_diff.magnitude;
                Vector3 ideal = - spring_constant * (x_diff.magnitude - rest_length) * x_div;
                Vector3 damp = - damping_constant * (Vector3.Dot(one_side.velocity - other_side.velocity, x_diff) / x_diff.magnitude) * x_div;
                Vector3 force = ideal + damp;
                one_side.total_force += force;
                other_side.total_force -= force;
            }
        }
    }

    public class GravityNt : Force
    {
        List<Particle> sources;

        String mode;

        public GravityNt(List<Particle> sources, String mode)
        {
            this.sources = sources;
            this.mode = mode;
        }

        public override void applyForce(List<Particle> particles)
        {
            foreach(Particle p_i in sources)
            {
                foreach(Particle p_j in particles)
                {
                    if(p_i == p_j)
                    {
                        continue;
                    }
                    Vector3 diff = p_i.position - p_j.position;
                    Vector3 f = 30 * (diff.normalized / (float)(Math.Pow(diff.magnitude,2)+0.1f));
                    if(mode == "attract")
                    {
                        f = f * -1;
                    }                    
                    if(f.magnitude > 10)
                    {
                        f = f.normalized * 10;
                    }
                    p_i.total_force += f;
                    p_j.total_force += -f;
                }
            }
        }
    }

    // https://processing.org/examples/flocking.html
    public class Flocking : Force
    {
        float max_force;
        
        float max_speed;

        public Flocking(float max_force=1f, float max_speed=5)
        {
            this.max_force = max_force;
            this.max_speed = max_speed;
        }

        Vector3 separate(List<Particle> particles, Particle p)
        {
            float desiredseparation = 25f;
            Vector3 steer = Vector3.zero;
            int count = 0;
            foreach(Particle other in particles)
            {
                float distance = Vector3.Distance(p.position, other.position);
                if((distance > 0) && (distance < desiredseparation))
                {
                    steer += (p.position - other.position).normalized / distance;
                    count++;
                }
            }
            if(count > 0)
            {
                steer = steer / (float) count;
            }
            if(steer.magnitude > 0)
            {
                steer = (steer.normalized * max_speed) - p.velocity;
                steer = steer.normalized * max_force;
            }
            return steer;            
        }

        Vector3 align(List<Particle> particles, Particle p)
        {
            float neighbordist = 50f;
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach(Particle other in particles)
            {
                float distance = Vector3.Distance(p.position, other.position);
                if((distance > 0) && (distance < neighbordist))
                {
                    sum += other.velocity;
                    count++;
                }
            }
            if(count > 0)
            {
                sum = sum / (float) count;
                Vector3 steer = (sum.normalized * max_speed) - p.velocity;
                steer = steer.normalized * max_force;
                return steer;
            }
            else
            {
                return Vector3.zero;
            }
        }

        Vector3 seek(Vector3 target, Particle p)
        {
            Vector3 desired = target - p.position;
            desired = desired.normalized * max_speed;
            Vector3 steer = desired - p.velocity;
            return steer.normalized * max_force;
        }

        Vector3 cohesion(List<Particle> particles, Particle p)
        {
            float neighbordist = 50f;
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach(Particle other in particles)
            {
                float distance = Vector3.Distance(p.position, other.position);
                if((distance > 0) && (distance < neighbordist))
                {
                    sum += other.position;
                    count++;
                }
            }
            if(count > 0)
            {
                sum = sum / (float) count;
                return seek(sum, p);
            }
            else
            {
                return Vector3.zero;
            }
        }

        Vector3 calculate_force(List<Particle> particles, Particle p)
        {
            Vector3 f = separate(particles, p) * 1.5f;
            f += align(particles, p) * 1f;
            f += cohesion(particles, p) * 1f;
            return f;
        }

        public override void applyForce(List<Particle> particles)
        {
            foreach(Particle p in particles)
            {
                p.total_force += calculate_force(particles, p);
            }
        }
    }
}
