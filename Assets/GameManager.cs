using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public GameObject creaturePrefab;
	public GameObject foodPrefab;
	List<CreatureBehaviour> creatureList;

	float [] champion = null;
	float fitnessRecord = 0;

	GeneticAlgorithm<float> ga;
	int numGenes = 4;
	System.Random random;

	public int populationSize = 100;
	public int elitism = 5;
	public float mutationRate = 0.01f;
	public int amountOfFood = 1;

	public Text timer;
	public Text generation;
	public Text fitness;
	public Text champ;
	public Text record;

	public float timeout = 2 * 60;
	float startTime = 0;
	public bool running = false;

	void Awake(){
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(this);
		}

		random = new System.Random();
		ga = new GeneticAlgorithm<float>(populationSize, numGenes, random, GetRandomGene, FitnessFunction, elitism, mutationRate);

		creatureList = new List<CreatureBehaviour>();
	}

	void loadGeneration()
	{
		GameObject [] creatures = GameObject.FindGameObjectsWithTag("Creature");
		GameObject [] food = GameObject.FindGameObjectsWithTag("Food");

		for(int c=0; c<creatures.Length; c++)
		{
			Destroy(creatures[c]);
		}
		for(int f=0; f<food.Length; f++)
		{
			Destroy(food[f]);
		}

		creatureList.Clear();

		for(int i=0; i<ga.Population.Count; i++)
		{
			GameObject newCreature = Instantiate(creaturePrefab, new Vector3(Random.Range(-40f, 40f), 1f, Random.Range(-40f, 40f)), Quaternion.identity);
			CreatureBehaviour cb = newCreature.GetComponent<CreatureBehaviour>();
			cb.LoadDNA(ga.Population[i], i);
			creatureList.Add(cb);
		}

		for(int n=0; n<amountOfFood; n++)
		{
			Instantiate(foodPrefab, new Vector3(Random.Range(-49f, 49f), 0.5f, Random.Range(-49f, 49f)), Quaternion.identity);
		}
	}

	public void startRound()
	{
		running = true;
		startTime = Time.time;
	}

	public void finishRound()
	{
		running = false;

		//If we are not done (found our ninja)
		ga.NewGeneration();

		generation.text = "gen: " + ga.Generation.ToString();
		fitness.text = "Fitness: " + ga.BestFitness.ToString();
		if(champion == null || ga.BestFitness > fitnessRecord)
		{
			champion = ga.BestGenes;
			fitnessRecord = ga.BestFitness;
			champ.text = champion[0].ToString() + ", " + champion[1].ToString() + ", " + champion[2].ToString() + ", " + champion[3].ToString();
			record.text = fitnessRecord.ToString();
		}

		loadGeneration();
		startRound();
	}

	public float timeScale = 1f;

	// Use this for initialization
	void Start () {
		Time.timeScale = timeScale;
		loadGeneration();
		startRound();
	}
	
	// Update is called once per frame
	void Update () {
		if(timeScale != Time.timeScale) Time.timeScale = timeScale;

		if(running)
		{
			float currentTimeLeft = timeout - (Time.time - startTime);

			timer.text = Mathf.Floor(currentTimeLeft).ToString();

			if(currentTimeLeft <= 0 || GameObject.FindGameObjectsWithTag("Food").Length == 0)
			{
				finishRound();
			}
		}
	}

	float GetRandomGene()
	{
		return Random.Range(0f, 80f);
	}

	float FitnessFunction(int index)
	{
		for(int i=0; i<creatureList.Count; i++)
		{
			if(creatureList[i].creatureIndex == index)
			{
				float score = creatureList[i].points;
				score = score / amountOfFood;

				score = Mathf.Pow(2, score) - 1;
				return score;
			}
		}
		return 0;
	}
}
