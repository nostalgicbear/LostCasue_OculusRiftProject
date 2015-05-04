using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * This script is palced on hte sun, and causes the sun to rotate around, creating a day night cycle.
 */ 

public class DayNightCycle : MonoBehaviour {
	
	public Gradient nightDayColor;

	/*
	 * These are all customizable settings that alter different aspects of the light
	 */ 
	public float maxIntensity = 3f;
	public float minIntensity = 0f;
	public float minPoint = -0.2f;
	
	public float maxAmbient = 1f;
	public float minAmbient = 0f;
	public float minAmbientPoint = -0.2f;
	
	
	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale = 1f;
	
	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;
	
	public Vector3 dayRotateSpeed;
	public Vector3 nightRotateSpeed;
	public Vector3 fastNightRotateSpeed;

	public bool isNightTime = false;
	private bool isDayTime = false;

	private GameObject player;
	private GameObject player2;
	private bool p1_inside = false;
	private bool p2_inside = false;
	private Transform closestHut;

	public List<Transform> huts = new List<Transform>();
	
	private float skySpeed = 1;
	private float nightSpeed = 30.0f; //not currently used
	public bool increaseNightSpeed = false;
	
	
	Light mainLight;
	Skybox sky;
	Material skyMat;
	
	void Start () 
	{
		
		mainLight = GetComponent<Light>();
		skyMat = RenderSettings.skybox;
		
	}
	
	void Update () 
	{

		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
		
		mainLight.intensity = i;
		
		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		i = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = i;
		
		mainLight.color = nightDayColor.Evaluate(dot);
		RenderSettings.ambientLight = mainLight.color;
		
		RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;
		
		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat ("_AtmosphereThickness", i);

		/**
		 * Its daytime
		 */ 
		if (dot > 0) {
			transform.Rotate (dayRotateSpeed * Time.deltaTime * skySpeed);
			isNightTime = false;
			/**
			 * Its night time
			 */ 
		} else {
			isNightTime = true;
			/**
			 * This is only true if both players are inside a hut. If players are inside the hut, then the nught cycle
			 * will go at a faster rate. 
			 */ 
				if(increaseNightSpeed){
				//	Debug.Log("Increasing night speed");
					transform.Rotate (fastNightRotateSpeed * Time.deltaTime * skySpeed);
				} else {
				//	Debug.Log("Night time : less than 2 in hut");
					transform.Rotate (nightRotateSpeed * Time.deltaTime * skySpeed);
				}
		}

	}
}
