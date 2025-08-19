using NUnit.Framework;
using UnityEngine;


public class SelectionmanagerTest
{
    [Test]
    public void TestSelect()
    {
        SelectionManager s = new ();

        var tileObject = new GameObject();
        var tile = tileObject.AddComponent<Tile>();
        
        s.Select(tile);
        Assert.IsTrue(s.HasSelection);
        Assert.AreEqual(s.SelectedTile, tile);        
    }

    [Test]
    public void TestDeselect()
    {
        SelectionManager s = new ();

        var tileObject = new GameObject();
        var tile = tileObject.AddComponent<Tile>();
        
        s.Select(tile);
        s.Deselect();
        Assert.IsTrue(!s.HasSelection);
        Assert.AreEqual(null, s.SelectedTile);        
    }
}