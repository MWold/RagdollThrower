using UnityEngine;
using System.Collections;

public class FoodScore : MonoBehaviour {

  private bool eaten = false;

	void Update () {
    if (transform.position.z > GameObject.Find("Mouth").transform.position.z) {
      if (!eaten)
        GameObject.Find("/Robot").GetComponent<Robot>().Eat();
      eaten = true;
    }
	}
}
