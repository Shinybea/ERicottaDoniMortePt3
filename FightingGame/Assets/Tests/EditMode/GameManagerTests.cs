using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTest
{
    private GameManager gameManager;

    [SetUp]
    public void Setup()
    {
        gameManager = GameManager.Instance;
    }

    [Test]
    public void GameManagerInstanceIsNotNull()
    {
        Assert.IsNotNull(gameManager);
    }

    [Test]
    public void GameManagerIsSingleton()
    {
        var gameManager2 = GameManager.Instance;
        Assert.AreEqual(gameManager, gameManager2);
    }

}