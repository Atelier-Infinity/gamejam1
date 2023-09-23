using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttachmentPointManager : MonoBehaviour
{
    public List<AttachmentPoint> attachmentPoints = new();
    [Header("Limb placement search area")] public Transform center;
    [Range(0, 90)] public float maxPhi = 15;
    [Range(0, 90)] public float minPhi = 15;
    public float searchRange = 3;
    // todo add min spacing and move arms points around to make room

    public AttachmentPoint NewAttachmentPoint()
    {
        var point = new GameObject("Attachment Point").AddComponent<AttachmentPoint>();
        attachmentPoints.Add(point);
        point.transform.SetParent(transform);
        // move to some place free on surface of model
        var hit = GetLimbPoint();
        point.transform.position = hit.point;
        point.transform.rotation = Quaternion.LookRotation(hit.normal);
        return point;
    }

#if UNITY_EDITOR && true
    private void Update()
    {
        if (!Input.GetKeyUp(KeyCode.K)) return;

        var limb = FindObjectOfType<LimbAttachment>();
        if (!limb) return;
        if (limb.Attachment)
        {
            Debug.Log("Already taken");
            return;
        }
        var point = NewAttachmentPoint();
        limb.Attach(point);
    }
#endif

    private (Vector3 point, Vector3 normal) GetLimbPoint()
    {
        var rotation = center.rotation;
        var dir = rotation *
                  Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) *
                  Quaternion.AngleAxis(Random.Range(-minPhi, maxPhi), Vector3.left) *
                  Vector3.forward;

        if (Physics.Raycast(center.position + dir * searchRange, -dir, out var hit, searchRange, 1 << 14))
        {
            return (hit.point, hit.normal);
        }

        return (Vector3.zero, Vector3.zero);
    }

    public GameObject DetachRandomLimb()
    {
        if (attachmentPoints.Count == 0) return null;

        var startIdx = Random.Range(0, attachmentPoints.Count);
        var idx = startIdx;
        while (true)
        {
            var point = attachmentPoints[idx];
            if (!point.IsAttached)
            {
                idx = (idx + 1) % attachmentPoints.Count;
                if (idx == startIdx) return null; // dont go in circles
                continue; // there is no limb attached to the point, pock a different one
            }

            attachmentPoints.Remove(point);
            var limb = point.transform.GetChild(0);
            Destroy(point.gameObject); // maybe use obj pool

            limb.SendMessage(nameof(LimbAttachment.Detach));
            return limb.gameObject;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var position = center.position;
        var rotation = center.rotation;

        var maxDir = rotation * Quaternion.AngleAxis(maxPhi, Vector3.left) * Vector3.forward;
        var minDir = rotation * Quaternion.AngleAxis(-minPhi, Vector3.left) * Vector3.forward;
        for (var i = 0; i < 360; i += 10)
        {
            var spin = Quaternion.AngleAxis(i, rotation * Vector3.up);
            Gizmos.DrawLine(position, position + spin * maxDir * searchRange);
            Gizmos.DrawLine(position, position + spin * minDir * searchRange);
            Gizmos.DrawLine(position, position + spin * rotation * Vector3.forward * searchRange);
        }
    }
}