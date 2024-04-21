using System.Collections.Generic;
using UnityEngine;
using System;

namespace MyClasses
{
    public abstract class MyCollider
    {
        protected double errorThreshold = 0.1;//Math.Pow((double)10, (double)-3);

        protected float coefficientOfRestitution = 0.8f;

        public abstract void detect_collision(Particle particle, out bool collided_particle);

        protected abstract bool detect_contact(Vector3 middle);

        protected abstract bool in_legal_space(Vector3 middle);

        protected void findContact(
            Vector3 start, Vector3 end, float max_time,
            out Vector3 contact_point, out float contact_time
        )
        {
            contact_point = new Vector3();
            contact_time = 0;
            int i=0;
            while(i<100)
            {
                float middle_time = 0.5f * max_time;
                Vector3 middle = Vector3.Lerp(start, end, middle_time);
                if(detect_contact(middle))
                {
                    contact_point = middle;
                    contact_time = middle_time;
                    break;
                }
                if(in_legal_space(middle))
                {
                    start = middle;
                }
                else
                {
                    end = middle;
                }
                i++;
            }
            return;
        }

        protected void updateVelocity(Particle p, Vector3 normal)
        {
            Vector3 v_n = Vector3.Dot(p.velocity, normal) * normal;
            Vector3 v_t = p.velocity - v_n;
            Vector3 v_new = - coefficientOfRestitution * v_n + v_t;
            p.velocity = - coefficientOfRestitution * v_n + v_t;
        }

        public abstract void collision_response(
            Particle particle, Particle before_collision,
            out Particle contact_particle, out float contact_time
        );
    }
    
    public class MyCubeCollider : MyCollider
    {
        List<Plane> planes;

        Plane collision_plane;
        
        public MyCubeCollider(List<Plane> planes)
        {
            this.planes = planes;
        }

        public override void detect_collision(Particle particle, out bool collision_detected)
        {
            collision_detected = false;
            for(int i=0; i<planes.Count; i++)
            {
                if(planes[i].GetDistanceToPoint(particle.position) < errorThreshold) // particle is out of bounds
                {
                    collision_detected = true;
                    collision_plane = planes[i];
                    break;
                }
            }
            return;
        }

        public override void collision_response(
            Particle particle, Particle before_collision,
            out Particle contact_particle, out float contact_time
        )
        {
            contact_particle = null;
            
            Vector3 contact_point;
            findContact(before_collision.position, particle.position, 1, out contact_point, out contact_time);

            particle.position = contact_point;
            updateVelocity(particle, collision_plane.normal);
            contact_particle = particle;
        }

        protected override bool detect_contact(Vector3 middle)
        {
            return Mathf.Abs(collision_plane.GetDistanceToPoint(middle)) < errorThreshold;
        }

        protected override bool in_legal_space(Vector3 middle)
        {
            return collision_plane.GetDistanceToPoint(middle) > errorThreshold;
        }
    }

    public class MySphereCollider : MyCollider
    {
        Vector3 center;

        float radius;

        public MySphereCollider(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public override void detect_collision(Particle particle, out bool collision_detected)
        {
            collision_detected = false;
            if(in_legal_space(particle.position))
            {
                return;
            }
            collision_detected = true;
            return;
        }

        public override void collision_response(
            Particle particle, Particle before_collision,
            out Particle contact_particle, out float contact_time
        )
        {
            contact_particle = null;
            
            Vector3 contact_point;
            findContact(before_collision.position, particle.position, 1, out contact_point, out contact_time);

            particle.position = contact_point;
            updateVelocity(particle, get_normal_normalized(contact_point));
            contact_particle = particle;
        }

        protected override bool detect_contact(Vector3 middle)
        {
            return Mathf.Abs(Vector3.Distance(middle, center) - radius) < errorThreshold;
        }

        protected override bool in_legal_space(Vector3 middle)
        {
            return Vector3.Distance(middle, center) < radius - errorThreshold;
        }

        Vector3 get_normal_normalized(Vector3 contact_point)
        {
            return (contact_point - center) / radius;
        }
    }
}
