using UnityEngine;

public class AttachmentPoint : MonoBehaviour
{
    public bool IsTargeted { get; set; }
    public bool IsAttached { get; set; }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, .4f, .8f);
        Gizmos.DrawSphere(transform.position, .6f);
    }
    
#if UNITY_EDITOR && false // true
    #region testing hardcoding
    public GameObject limb;
    private bool sent;
    private void Update()
    {
        if (Time.time<3 || sent) return;
        sent = true;
        limb.SendMessage(nameof(LimbAttachment.Attach), this);
        Debug.Log("connecting limb");
        
        Invoke(nameof(Detach), 2);
    }

    private void Detach()
    {
        limb.SendMessage(nameof(LimbAttachment.Detach));
        Debug.Log("detaching limb");
    }
  #endregion
#endif
}