using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Effect Prefabs")]
    [SerializeField] private GameObject explosionEffect;
    
    private static EffectManager instance;
    
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    public static EffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EffectManager>();
                
                if (instance == null)
                {
                    GameObject go = new GameObject("EffectManager");
                    instance = go.AddComponent<EffectManager>();
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void AddExplosion(Vector3 pos)
    {
        if (explosionEffect == null)
        {
            Debug.LogWarning("ExplosionEffect Prefab이 설정되지 않았습니다!");
            return;
        }
        
        _ = Instantiate(explosionEffect, pos, Quaternion.identity);
    }
} 