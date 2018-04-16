using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBehaviour : MonoBehaviour {

	public float fov, force, mass, drag;
	public int points = 0;
	public int creatureIndex = -1;

	GameObject targetFood = null;

	Rigidbody rb;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody>();

		updateValues();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.instance.running)
		{
			if(targetFood == null || !targetFood.activeSelf)
			{
				FindFood();
			}

			ChaseFood();
		}
	}

	void FindFood()
	{
		GameObject [] food = GameObject.FindGameObjectsWithTag("Food");
		List<GameObject> inRange = new List<GameObject>();

		if(food.Length == 0) 
		{
			targetFood = null;
			return;
		}

		for(int i=0; i<food.Length; i++)
		{
			float dist = Vector3.Distance(food[i].transform.position, transform.position);
			if(dist < fov && food[i].activeSelf)
			{
				inRange.Add(food[i]);
			}
		}

		if(inRange.Count == 0)
		{
			targetFood = null;
		}
		else
		{
			int targetIndex = Random.Range(0, inRange.Count);
			targetFood = inRange[targetIndex];
		}
 	}

	void ChaseFood()
	{

		Vector3 f;
		if(targetFood != null)
		{
			f = targetFood.transform.position - transform.position;
			f.Normalize();
		}
		else
		{
			f = new Vector3(Random.Range(-1f, 1f),  0, Random.Range(-1f, 1f));
		}

		rb.AddForce(f * force);
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.collider.gameObject.CompareTag("Food"))
		{
			other.collider.gameObject.SetActive(false);
			points++;
		}
	}

	public void LoadDNA(DNA<float> dna, int index)
	{
		fov = dna.Genes[0];
		force = dna.Genes[1];
		mass = dna.Genes[2];
		drag = dna.Genes[3];

		creatureIndex = index;

		updateValues();
	}

	void updateValues()
	{
		rb.mass = mass;
		rb.drag = drag;
		transform.localScale = new Vector3(transform.localScale.x + mass/30, transform.localScale.x + mass/30, transform.localScale.x + mass/30);
	}
}
