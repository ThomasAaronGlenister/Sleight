using System;
using UnityEngine;
using Deck;

public class LevelGrid
{
	//Struct to hold 2d coordinate system map of level layout
	struct GridCoordinate<X , Y> : IEquatable <GridCoordinate<X , Y>>
	{
		public int X; 
		public int Y;

		public GridCoordinate(int x, int y)
		{
			X = x;	
			Y = y;
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((GridCoordinate<X, Y>)obj);
        }

        public bool Equals(GridCoordinate<X, Y> other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y);
        }
    }

	//Key Value pair sets of Level grid coordinates to Chamber game object
	Dictionary<GridCoordinate, GameObject> macLevelGrid = new();

	public LevelGrid()
	{

	}

	public void AddChamberToGrid(int x, int y, GameObject pcChamber)
	{
        GridCoordinate lsGridSection = new GridCoordinate(x, y);
        macLevelGrid[lsGridSection] = pcChamber;
    }
}
