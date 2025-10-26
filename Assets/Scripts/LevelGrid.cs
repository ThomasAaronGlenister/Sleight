using System;
using System.Collections.Generic;
using UnityEngine;
using Deck;
using JetBrains.Annotations;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

/**
 * CLASS: LevelGrid: 2D grid representation of Level designed
 */
public class LevelGrid
{
	//Struct to hold 2d coordinate system map of level layout
	struct GridCoordinate <X, Y> : IEquatable<GridCoordinate<X, Y>>
	{
		readonly X mnX;
        readonly Y mnY;

		public GridCoordinate(X x, Y y)
        {
            this.mnX = x;
            this.mnY = y;
        }

        //Overridden Key generation
        public override int GetHashCode()
        {
            return mnX.GetHashCode() ^ mnY.GetHashCode();
        }

        //Overridden Type check
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals((GridCoordinate<X, Y>)obj);
        }

        //Overridden Key comparison
        public bool Equals(GridCoordinate<X, Y> other)
        {
            return other.mnX.Equals(mnX) && other.mnY.Equals(mnY);
        }
    }

    //Map of grid coordinates to the Chamber assigned there
	Dictionary<GridCoordinate<int,int>, GameObject> macLevelGrid = new();

    public LevelGrid()
	{

	}

    /**
     * Adds a chamber to the level grid at the specified point
     */
	public void AddChamberToGrid(int x, int y, GameObject pcChamber)
	{
        //Add the chamber to the grid
        macLevelGrid[new GridCoordinate<int, int>(x,y)] = pcChamber;
    }

    /**
     * Checks whether the level grid section contains chamber 
     */
    public bool IsGridSectionFilled(int x, int y)
    {
        return macLevelGrid.ContainsKey(new GridCoordinate<int, int>(x, y));
    }

}
