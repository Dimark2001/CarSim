using UnityEngine;

namespace _Game.__Scripts.System.DI.Test
{
    [DefaultExecutionOrder(-10)]
    public class DITester : MonoBehaviour
    {
        private void Start()
        {
            TestDiFabric();
        }

        private void TestDiFabric()
        {
            Debug.Log("=== Testing DiFabric ===");

            var scoreManager = DiFabric.Create<ScoreManager>();
            scoreManager.AddScore(100);

            var prefab = GameResources.Prefabs.PC;
            var instance = DiFabric.InstantiatePrefab(prefab);
        }
    }
}