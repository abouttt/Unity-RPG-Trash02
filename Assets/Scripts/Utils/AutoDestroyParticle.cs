using UnityEngine;

public class AutoDestroyParticle : BaseMonoBehaviour
{
    private ParticleSystem _ps;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();
        var main = _ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        Managers.Resource.Destroy(gameObject);
    }
}
