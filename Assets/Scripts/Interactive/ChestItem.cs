using UnityEngine;

public class ChestItem : FieldItem
{
    [SerializeField]
    private Animation _openAnimation;

    [SerializeField]
    private ParticleSystem _hasItemParticle;

    [SerializeField]
    private ParticleSystem _openItemParticle;

    protected override void Awake()
    {
        base.Awake();

        _openItemParticle.gameObject.SetActive(false);
    }

    public override void Interaction()
    {
        _openItemParticle.gameObject.SetActive(true);
        _openItemParticle.Play();
        _openAnimation.Play();

        base.Interaction();
    }

    public override void RemoveItem(ItemData itemData, int count)
    {
        base.RemoveItem(itemData, count);

        if (Items.Count == 0)
        {
            _hasItemParticle.Stop();
        }
    }
}
