using UnityEngine;
using System.Collections;
using Leap;

public class Pincher : MonoBehaviour {

  const int NUM_FINGERS = 5;
  const int NUM_JOINTS = 4;
  const int HAND_LAYER_INDEX = 11;
  const float PINCH_DISTANCE = 2.0f;
  const float SPRING_CONSTANT = 3000.0f;
  const float THUMB_TRIGGER_DISTANCE = 0.035f;

  private bool pinching_;
  private Collider grabbed_;
  private int layer_mask_;

  void Start () {
    pinching_ = false;
    grabbed_ = null;
    layer_mask_ = 1 << HAND_LAYER_INDEX;
    layer_mask_ = ~layer_mask_;
  }

  void OnPinch(Vector3 pinch_position) {
    Debug.Log("PINCH");
    pinching_ = true;

    // Check if we pinched a live human.
    GameObject human = GameObject.Find("HumanGame");
    if (human != null && (human.transform.position - pinch_position).magnitude < PINCH_DISTANCE) {
      GameObject.Find("HumanGame").GetComponent<RagdollInstantiator>().Die();
      Debug.Log("DEAD");
    }

    // Check if we pinched a movable object and grab the closest one.
    Collider[] close_things = Physics.OverlapSphere(pinch_position, PINCH_DISTANCE, layer_mask_);
    Vector3 distance = new Vector3(PINCH_DISTANCE, 0.0f, 0.0f);
    for (int j = 0; j < close_things.Length; ++j) {
      Vector3 new_distance = pinch_position - close_things[j].transform.position;
      if (close_things[j].rigidbody != null && new_distance.magnitude < distance.magnitude) {
        grabbed_ = close_things[j];
        distance = new_distance;
      }
    }
  }

  void OnRelease() {
    Debug.Log("RELEASE");
    grabbed_ = null;
    pinching_ = false;
  }

  void Update() {
    bool trigger_pinch = false;
    Hand hand = GetComponent<HandModel>().GetLeapHand();

    // Thumb tip is the pinch position.
    Vector3 thumb_tip = hand.Fingers[0].TipPosition.ToUnityScaled();

    // Check thumb tip distance to joints on all other fingers.
    // If it's close enough, start pinching.
    for (int i = 1; i < NUM_FINGERS && !trigger_pinch; ++i) {
      Finger finger = hand.Fingers[i];

      for (int j = 0; j < NUM_JOINTS && !trigger_pinch; ++j) {
        Vector3 joint_position = finger.JointPosition((Finger.FingerJoint)(j)).ToUnityScaled();
        Vector3 distance = thumb_tip - joint_position;
        if (distance.magnitude < THUMB_TRIGGER_DISTANCE)
          trigger_pinch = true;
      }
    }

    Vector3 pinch_position = transform.TransformPoint(thumb_tip);

    // Only change state if it's different.
    if (trigger_pinch && !pinching_)
      OnPinch(pinch_position);
    else if (!trigger_pinch && pinching_)
      OnRelease();

    // Accelerate what we are grabbing toward the pinch.
    if (grabbed_ != null) {
      Vector3 distance = pinch_position - grabbed_.transform.position;
      grabbed_.rigidbody.AddForce(SPRING_CONSTANT * distance);
    }
  }
}
