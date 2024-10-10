using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This class manages the basic functions of the levels
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform[] path;
    private int currency;

    /// <summary>
    /// This function manages the singleton instance, so it initializes the <c>instance</c> variable, if not set, or
    /// deletes the object otherwise
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        currency = 250;
#else
        currency = 250;
#endif
    }

    /// <summary>
    ///  This function increases the currency
    /// </summary>
    /// <param name="amount"> the amount of how much the currency is increased</param>
    public void GainCurrency(int amount)
    {
        currency += amount;
    }

    /// <summary>
    ///  This function decreases the currency
    /// </summary>
    /// <param name="amount"> the amount of how much the currency is decreased</param>
    public bool SpendCurrency(int amount)
    {
        if(amount  <= currency) {
            //buy item
            currency -= amount;
                return true; }
        else
        {
            Debug.Log("you are broke :)");
                return false;
        }
    }

    /// <summary>
    /// Gets the start point of the path 
    /// </summary>
    /// <returns>path start point</returns>
    public Transform GetStartPoint()
    {
        return startPoint;
    }

    /// <summary>
    /// Gets all points the path is consisting of
    /// </summary>
    /// <returns>path point waypoints</returns>
    public Transform[] GetPath()
    {
        return path;
    }

    /// <summary>
    /// Gets the currency of the user
    /// </summary>
    /// <returns>user's currency</returns>
    public int GetCurrency()
    {
        return currency;
    }
}
