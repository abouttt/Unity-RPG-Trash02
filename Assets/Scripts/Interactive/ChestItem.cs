using UnityEngine;

public class ChestItem : FieldItem
{
    [SerializeField]
    private Animation _openAnimation;

    [SerializeField]
    private ParticleSystem _closeParticle;

    [SerializeField]
    private ParticleSystem _openParticle;

    protected override void Awake()
    {
        base.Awake();

        _openParticle.gameObject.SetActive(false);
    }

    public override void Interaction()
    {
        _openParticle.gameObject.SetActive(true);
        _openParticle.Play();
        _openAnimation.Play();

        base.Interaction();
    }

    public override void RemoveItem(ItemData itemData, int count)
    {
        base.RemoveItem(itemData, count);

        Managers.Resource.Destroy(_closeParticle.gameObject);
        Managers.Resource.Destroy(_openParticle.gameObject);
    }
}
