using UnityEngine;
using System.Collections;

public class ResourceCollection : Photon.MonoBehaviour {
	public int maxWoodCount = 500;
	public int woodCount;
	public int maxMeatCount = 500;
	public int meatCount = 20;
	public int maxStoneCount = 500;
	public int stoneCount;


	void Update()
	{
		if(Input.GetButtonDown("Inventory"))
		{
			Debug.Log("Wood supply is: " + woodCount);
			Debug.Log("Meat supply is: " + meatCount);
		}
	}


	//if player collides with wood
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag=="wood")
		{
			Debug.Log ("collected wood");
			Destroy(other.gameObject);
			woodCount+=10;
		}

		if(other.gameObject.tag=="meat")
		{
			Debug.Log ("collected meat");
			Destroy(other.gameObject);
			meatCount+=10;
		}

		if(other.gameObject.tag=="stone")
		{
			Debug.Log ("collected stone");
			Destroy(other.gameObject);
			stoneCount+=10;
		}
	}
	




	
}
