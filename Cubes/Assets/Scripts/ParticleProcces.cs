using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WhichParticle
{
    stars,
}

public class ParticleProcces : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particles_l;

    public void PlayParticle(WhichParticle whichParticle, Vector3 pos)
    {
        int id = (int)whichParticle;

        _particles_l[id].transform.SetPositionAndRotation(pos, Quaternion.identity);
        _particles_l[id].Play();
    }

}
