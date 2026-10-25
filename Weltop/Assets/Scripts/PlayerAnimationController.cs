using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
public class PlayerAnimationController : MonoBehaviour
{

    private class BoneTransform
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }
    }

    private enum State
    {
        Walking,
        Ragdoll,
        StandingUp,
        ResettingBones
    }


    [SerializeField]
    private string _faceUpStandUpStateName;

    [SerializeField]
    private string _faceDownStandUpStateName;

    [SerializeField]
    private string _faceUpStandUpClipName;

    [SerializeField]
    private string _faceDownStandUpClipName;

    [SerializeField]
    private float _timeToResetBones;

    [SerializeField]
    private Vector3 _testRagdollHitFrom;
    [SerializeField]
    private bool _testRagdoll;

    private Rigidbody[] _ragdollRigidbodies;
    private State _currentState = State.Walking;
    private Animator _animator;
    private float _timeToWakeUp;
    private Transform _hipsBone;

    private BoneTransform[] _faceUpStandUpBoneTransforms;
    private BoneTransform[] _faceDownStandUpBoneTransforms;
    private BoneTransform[] _ragdollBoneTransforms;
    private Transform[] _bones;
    private float _elapsedResetBonesTime;
    private bool _isFacingUp;
    private PlayerController _playerController;
    private CinemachineCamera _camera;

    
    void Awake()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _playerController = transform.parent.GetComponentInChildren<PlayerController>();
        _animator = GetComponent<Animator>();
        _hipsBone = _animator.GetBoneTransform(HumanBodyBones.Hips);
        _camera = transform.parent.GetComponentInChildren<CinemachineCamera>();
        _bones = _hipsBone.GetComponentsInChildren<Transform>();
        _faceUpStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _faceDownStandUpBoneTransforms = new BoneTransform[_bones.Length];
        _ragdollBoneTransforms = new BoneTransform[_bones.Length];

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            _faceUpStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _faceDownStandUpBoneTransforms[boneIndex] = new BoneTransform();
            _ragdollBoneTransforms[boneIndex] = new BoneTransform();
        }

        PopulateAnimationStartBoneTransforms(_faceUpStandUpClipName, _faceUpStandUpBoneTransforms);
        PopulateAnimationStartBoneTransforms(_faceDownStandUpClipName, _faceDownStandUpBoneTransforms);

        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {

        if (_testRagdoll)
        {
            _testRagdoll = false;
            TriggerRagdoll(_testRagdollHitFrom,transform.position);
        }
        switch (_currentState)
        {
            case State.Walking:
                WalkingBehaviour();
                break;
            case State.Ragdoll:
                RagdollBehaviour();
                break;
            case State.StandingUp:
                StandingUpBehaviour();
                break;
            case State.ResettingBones:
                ResettingBonesBehaviour();
                break;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitPoint)
    { 
        EnableRagdoll();

        Rigidbody hitRigidbody = FindHitRigidbody(hitPoint);

        hitRigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);

        _currentState = State.Ragdoll;
        _timeToWakeUp = Random.Range(5, 10);
        RagdollResetColliders();
    } 

    private Rigidbody FindHitRigidbody(Vector3 hitPoint)
    {
        Rigidbody closestRigidbody = null;
        float closestDistance = 0;

        foreach (var rigidbody in _ragdollRigidbodies)
        {
            float distance = Vector3.Distance(rigidbody.position, hitPoint);

            if (closestRigidbody == null || distance < closestDistance)
            {
                closestDistance = distance;
                closestRigidbody = rigidbody;
            }
        }

        return closestRigidbody;
    }
      
    private void DisableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        _animator.enabled = true;
    }

    private void EnableRagdoll()
    {
        foreach (var rigidbody in _ragdollRigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }

        _animator.enabled = false;
    }

    private void WalkingBehaviour()
    {
        transform.position = _playerController.transform.position -  _playerController.GetComponent<CapsuleCollider>().height * Vector3.up * 0.5f;
        transform.rotation = Quaternion.Euler(0,_camera.transform.rotation.eulerAngles.y,0);
    }
    public void StartMoving()
    {
        _animator.SetBool("Running", true);
    }
    public void stopMoving()
    {
        _animator.SetBool("Running", false);
    }
    public void OnJump()
    {
        _animator.SetBool("Jumping", true);
    }
    public void OnLand()
    {
        _animator.SetBool("Jumping", false);
    }

    private void RagdollBehaviour()
    {
        _timeToWakeUp -= Time.deltaTime;

        if (_timeToWakeUp <= 0)
        {
            _isFacingUp = _hipsBone.up.y > 0;
            
            AlignRotationToHips();
            AlignPositionToHips();

            PopulateBoneTransforms(_ragdollBoneTransforms);

            _currentState = State.ResettingBones;
            _elapsedResetBonesTime = 0;
        }
    }

    private void StandingUpBehaviour()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(GetStandUpStateName()) == false)
        {
            _currentState = State.Walking;
        }
        _playerController.transform.position = transform.position + _playerController.GetComponent<CapsuleCollider>().height * Vector3.up * 0.5f;
    }

    private void ResettingBonesBehaviour()
    {
        _elapsedResetBonesTime += Time.deltaTime;
        float elapsedPercentage = _elapsedResetBonesTime / _timeToResetBones;

        BoneTransform[] standUpBoneTransforms = GetStandUpBoneTransforms();

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex ++)
        {
            _bones[boneIndex].localPosition = Vector3.Lerp(
                _ragdollBoneTransforms[boneIndex].Position,
                standUpBoneTransforms[boneIndex].Position,
                elapsedPercentage);

            _bones[boneIndex].localRotation = Quaternion.Lerp(
                _ragdollBoneTransforms[boneIndex].Rotation,
                standUpBoneTransforms[boneIndex].Rotation,
                elapsedPercentage);
        }

        if (elapsedPercentage >=1)
        {
            _currentState = State.StandingUp;
            DisableRagdoll();
            GetUpResetColliders();
            _animator.Play(GetStandUpStateName(), 0, 0);
        }
    }

    private void AlignRotationToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position;
        Quaternion originalHipsRotation = _hipsBone.rotation;

        Vector3 desiredDirection = -_hipsBone.forward;

        if (_isFacingUp)
        {
            desiredDirection *= -1;
        }

        desiredDirection.y = 0;
        desiredDirection.Normalize();

        Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
        transform.rotation *= fromToRotation;

        _hipsBone.position = originalHipsPosition;
        _hipsBone.rotation = originalHipsRotation;
    }

    private void AlignPositionToHips()
    {
        Vector3 originalHipsPosition = _hipsBone.position; 
        transform.position = _hipsBone.position;

        Vector3 positionOffset = GetStandUpBoneTransforms()[0].Position;
        positionOffset.y = 0;
        positionOffset = transform.rotation * positionOffset;
        transform.position -= positionOffset;

        if (Physics.Raycast(transform.position+ Vector3.up, Vector3.down, out RaycastHit hitInfo,10,~(1<<gameObject.layer)))
        {
            transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
        }
        
        _hipsBone.position = originalHipsPosition;
    }

    private void PopulateBoneTransforms(BoneTransform[] boneTransforms)
    {
        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            boneTransforms[boneIndex].Position = _bones[boneIndex].localPosition;
            boneTransforms[boneIndex].Rotation = _bones[boneIndex].localRotation;
        }
    }

    private void PopulateAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
    {
        Vector3 positionBeforeSampling = transform.position;
        Quaternion rotationBeforeSampling = transform.rotation;

        foreach (AnimationClip clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                clip.SampleAnimation(gameObject, 0);
                PopulateBoneTransforms(boneTransforms);
                break;
            }
        }

        transform.position = positionBeforeSampling;
        transform.rotation = rotationBeforeSampling;
    }

    private string GetStandUpStateName()
    {
        return _isFacingUp ? _faceUpStandUpStateName : _faceDownStandUpStateName;
    }

    private BoneTransform[] GetStandUpBoneTransforms()
    {
        return _isFacingUp ? _faceUpStandUpBoneTransforms : _faceDownStandUpBoneTransforms;
    }
    public void RagdollResetColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
        _playerController.GetComponent<Collider>().enabled = false;
        _playerController.GetComponent<Rigidbody>().useGravity = false;
        _playerController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }

    public void GetUpResetColliders()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        _playerController.GetComponent<Collider>().enabled = true;
        _playerController.GetComponent<Rigidbody>().useGravity = true;
    }

}
