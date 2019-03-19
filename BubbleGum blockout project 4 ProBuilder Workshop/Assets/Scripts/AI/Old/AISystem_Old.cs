using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

public class AISystem_Old : MonoBehaviour {

	public enum SpawnType {RANDOM, CIRCLE}

	public static AISystem_Old Instance {get; private set;}

	[SerializeField] private SpawnType spawnType = new SpawnType();

	[Header("Random Spawn")]
	[SerializeField] private float aiSpawnCooldown = 0f;
	[SerializeField] private float distanceMinWithPlayer = 0f;

	[Header("Circle Spawn")]
	[SerializeField] private int nbCircleEnemy = 0;
	[SerializeField] private int nbCircleHold = 0;
	[SerializeField] private bool holdNext = false;
	[SerializeField] private bool holdCenter = false;
	[SerializeField] private float circleRadius = 0f;
	[SerializeField] private float spawnAngle = 0f;

	[Header("Refs")]
	[SerializeField] private AiController aiPrefab = null;
	[SerializeField] private Vector2 mapSize = new Vector2();

	[SerializeField] public PlayerCharacter Player {get; private set;}
	

	private Transform selfTransform;

	private List<AiController> spawnedEnemies;

	private void Awake() {
		if(Instance != null){
			if(Instance != this){
				Destroy(gameObject);
			}

			return;
		}

		Instance = this;

		selfTransform = GetComponent<Transform>();
		spawnedEnemies = new List<AiController>();

		Player = FindObjectOfType<PlayerCharacter>();

		if(spawnType == SpawnType.RANDOM){
			StartCoroutine(RandomCoroutine());
		}
		
	}

	private void Update() {
		if(CustomInputManager.Instance.GetClearAIsDown()){
			ClearEnemy();
		}

		if(spawnType == SpawnType.CIRCLE){
			if(CustomInputManager.Instance.GetSpawnEnemyDown()){
				SpawnEnemyCircle();
			}
		}
	}

	private IEnumerator RandomCoroutine(){
		while(true){
			yield return new WaitForSeconds(aiSpawnCooldown);
			Vector3 position = Vector3.zero;
			Vector3 selfPosition = selfTransform.position;
			do{
				position.x = Random.Range(selfPosition.x - mapSize.x, selfPosition.x + mapSize.x);
				position.y = selfPosition.y;
				position.z = Random.Range(selfPosition.z - mapSize.y, selfPosition.z + mapSize.y);
			}while(Vector3.Distance(Player.bikeCenter.position, position) < distanceMinWithPlayer);
			SpawnEnemy(position);
		}
	}

	public void ClearEnemy(){
		foreach(AiController e in spawnedEnemies.ToArray()){
			Destroy(e.gameObject);
		}

		spawnedEnemies.Clear();
	}

	public void SpawnEnemy(Vector3 position, bool wait = true){
		SpawnEnemy(position, Player.bikeCenter, wait);
	}

	public void SpawnEnemy(Vector3 position, Transform target, bool wait = true){
		AiController newEnemy = Instantiate<AiController>(aiPrefab, position, Quaternion.identity);
		newEnemy.SetTarget(target, wait);
		spawnedEnemies.Add(newEnemy);
	}

	public void SpawnEnemyCircle(){
		SpawnEnemyCircle(nbCircleEnemy, nbCircleHold, holdNext, circleRadius, spawnAngle);
	}

	public void SpawnEnemyCircle(int nbCircleEnemy, int nbCircleHold, bool holdNext, float circleRadius, float spawnAngle){
		int[] holdPlaces = new int[nbCircleHold];

		if(holdCenter){
			int begin = (nbCircleEnemy - nbCircleHold) / 2;
			for(int i = 0; i < nbCircleHold; i++){
				holdPlaces[i] = begin + i;
			}
		}
		else if(holdNext){
			int begin = Random.Range(0, nbCircleEnemy - nbCircleHold);
			for(int i = 0; i < nbCircleHold; i++){
				holdPlaces[i] = begin + i;
			}
		}
		else{
			for(int i = 0; i < nbCircleHold; i++){
				int number;
				do{
					number = Random.Range(0, nbCircleEnemy);
				}while(NumberAlreadyUsed(number, holdPlaces, i));

				holdPlaces[i] = number;
			}
		}

		for(int i = 0; i < nbCircleEnemy; i++){
			bool isHold = false;

			for(int j = 0; j < nbCircleHold; j++){
				if(holdPlaces[j] == i){
					isHold = true;
					break;
				}
			}

			if(isHold){
				continue;
			}

			float convertSpawnAngle = spawnAngle * Mathf.Deg2Rad;
			float actualRotation = Player.BikeComponents.Transform.rotation.eulerAngles.z * Mathf.Deg2Rad + (Mathf.PI / 2f);

			Vector3 position = Player.bikeCenter.position;
			float angle = ((i * convertSpawnAngle) / nbCircleEnemy);
			angle = angle + actualRotation - (convertSpawnAngle / 2f);
			position.x += Mathf.Cos(angle) * circleRadius;
			position.z += Mathf.Sin(angle) * circleRadius;

			SpawnEnemy(position);
		}
	}

	public static bool NumberAlreadyUsed(int number, int[] array, int index){
		for(int i = 0; i < index; i++){
			if(array[i] == number){
				return true;
			}
		}

		return false;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		if(Selection.activeObject != gameObject){
			return;
		}

		Transform transform = GetComponent<Transform>();
		Vector3 position = transform.position;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector3(position.x - mapSize.x, position.y, position.z - mapSize.y), new Vector3(position.x - mapSize.x, position.y, position.z + mapSize.y));
		Gizmos.DrawLine(new Vector3(position.x - mapSize.x, position.y, position.z - mapSize.y), new Vector3(position.x + mapSize.x, position.y, position.z - mapSize.y));
		Gizmos.DrawLine(new Vector3(position.x + mapSize.x, position.y, position.z + mapSize.y), new Vector3(position.x + mapSize.x, position.y, position.z - mapSize.y));
		Gizmos.DrawLine(new Vector3(position.x + mapSize.x, position.y, position.z + mapSize.y), new Vector3(position.x - mapSize.x, position.y, position.z + mapSize.y));
	}

#endif
}
