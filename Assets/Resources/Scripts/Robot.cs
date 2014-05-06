using UnityEngine;
using System.Collections;
using System;

public class Robot : MonoBehaviour {

  public Texture eyesOnTexture;
  public Texture eyesOffTexture;
  public int waitTime = 100;
  public bool resetOnDone = false;
  public float maxChompMovement = 2.0f;

  private int time_remaining_ = 0;
	
	void Update () {
    if (time_remaining_ <= 0)
      return;

    time_remaining_--;
    float phase = 2.0f * Mathf.PI * time_remaining_ / waitTime - Mathf.PI;
    float t = 0.5f + 0.5f * Mathf.Cos(phase);
    transform.Find("Mouth").transform.localPosition = new Vector3(0, t * maxChompMovement, 0);
    
    if (time_remaining_ == 0) {
      transform.Find("Head").renderer.material.mainTexture = eyesOffTexture;

      if (resetOnDone)
        Application.LoadLevel(0);
    }
	}

  public void Eat() {
    if (time_remaining_ <= 0) {
      time_remaining_ = waitTime;
      transform.Find("Head").renderer.material.mainTexture = eyesOnTexture;
    }
  }
}
