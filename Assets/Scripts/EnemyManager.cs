using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * CLASS: EnemyManager
 * Maintains Data sets for all enemy game objects.
 */
public class EnemyManager : MonoBehaviour
{
    //Enemy Attributes sets
    EnemyAttributesSet mcEnemyAttributesLoader;

    //Base attack creation attributes for Enemys
    EnemyAttackAttributesSet mcEnemyAttackAttributes;

    //EnemyPrefab
    public GameObject mcEnemyPrefab;

    public void Initialize()
    {
        mcEnemyAttackAttributes = new EnemyAttackAttributesSet();

        mcEnemyAttributesLoader = new EnemyAttributesSet(mcEnemyAttackAttributes);
    }

    //Creates Enemies in the chamber
    public void PopulateWithEnemies(Chamber pcChamberToPopulate)
    {
        List<Vector2> lacSpawnPoints = pcChamberToPopulate.GetEnemySpawnPoints();

        //If any designated spawn points exist in this chamber
        while (lacSpawnPoints.Count != 0)
        {
            //Get Random Spawn point from list 
            Vector2 lcSpawnPoint = lacSpawnPoints[UnityEngine.Random.Range(0, lacSpawnPoints.Count)];

            int lnEnemyId = UnityEngine.Random.Range(0, 2);

           GameObject lcEnemy = Instantiate(mcEnemyPrefab, lcSpawnPoint, Quaternion.identity);
            lcEnemy.GetComponent<EnemyAI>().SetEnemyAttributes(mcEnemyAttributesLoader.GetBaseEnemyAttributes(lnEnemyId));
            lcEnemy.transform.SetParent(pcChamberToPopulate.transform);

            lacSpawnPoints.Remove(lcSpawnPoint);
        }
    }
}
