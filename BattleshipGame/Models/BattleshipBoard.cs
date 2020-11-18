using System;
using System.Collections.Generic;
using System.Linq;

public class BattleshipBoard {
    /*
        This class has following design constraints which must be true at all times:
        1. The size of the _battleshipBoard multi-dimensional array are always _battleshipBoard[_boardDimension][_boardDimension]
        2. A tile at _battleshipBoard multi-dimensional will be equal to 0 when that tile is unoccupied.
        3. A tile at _battleshipBoard multi-dimensional will be equal to battleship-id: number when it's occupied.
        4. When adding battleship which encompass of multiple tiles, then the occupied tiles will share the same id, for example:
          [2, 2, 2],
          [0, 1, 0],
          [0, 1, 0]
        4. When a battleship is destroyed, then battleship-id will be added on _destroyedBattleshipIds
        5. The list of ids of _destroyedBattleshipIds must be unique
        6. _totalNumberOfBattleships contains the number of battleship registered in this board.
        7. The game is lost if following 2 conditions are met:
           1. There's at least one battleship registered on this board.
           2. _destroyedBattleshipIds.length must be equal to _totalNumberOfBattleships.length
    */
    private readonly int _boardDimension;
    private readonly int?[][] _battleshipBoard;
    private readonly HashSet<int> _destroyedBattleshipIds = new HashSet<int>();
    private int _totalNumberOfBattleships = 0;
    private int _newBattleshipIdCounter = 1;

    public BattleshipBoard(int boardDimension) {
        this._boardDimension = boardDimension;
        this._battleshipBoard = new int?[boardDimension][];
        for (int x = 0; x < boardDimension; x++) {
            this._battleshipBoard[x] = new int?[boardDimension];
        }
    }

    /// <summary>
    /// This method will return the state of the board.
    /// 
    /// Keep in mind is the value returned by this method is just a copy in order to ensure 
    /// that the board is not modified unexpectedly by outside entity.
    /// </summary>
    public int?[][] GetBoardState()
    {
        return this._battleshipBoard.Select(x => x.ToArray()).ToArray();
    }

    /// <summary>
    /// This method is called when a battleship is added to the board.
    ///
    /// The size, location and the orientation of the battleship is defined by:
    /// (startX, startY) => (endX, endY)
    ///
    /// When we need to add a battleship which only consists of a single tile, then
    /// we could set (startX, startY) == (endX, endY)
    /// </summary>
    public int AddBattleShip(int startX, int startY, int endX, int endY) {
        if (!this.IsPointWithinBoard(startX, startY)) {
            throw new Exception($"Point ({startX}, {startY}) is outside board.");
        }
        else if (!this.IsPointWithinBoard(endX, endY)) {
            throw new Exception($"Point ({endX}, {endY}) is outside board.");
        }
        else if (!this.IsStraightLine(startX, startY, endX, endY)) {
            throw new Exception("The position of the battleship needs to be at straight line position.");
        }
        else if (!this.IsEmptyLine(startX, startY, endX, endY)) {
            throw new Exception($"Can not add a battleship from ({startX}, {startY}) to ({endX}, {endY}) because the space is occupied.");
        }

        int minX = Math.Min(startX, endX);
        int minY = Math.Min(startY, endY);
        int maxX = Math.Max(startX, endX);
        int maxY = Math.Max(startY, endY);
        int currentBattleshipId = this._newBattleshipIdCounter;
        for (int x = minX ; x <= maxX ; x++) {
            for (int y = minY ; y <= maxY ; y++) {
                this._battleshipBoard[x][y] = currentBattleshipId;
            }
        }

        this._newBattleshipIdCounter++;
        this._totalNumberOfBattleships++;
        return currentBattleshipId;
    }

    /// <summary>
    /// This method represents an attempt to attack specific location on the board.
    /// 
    /// If the attack is successful, then it will return AttackResult.Hit
    /// Otherwise, it will return AttackResult.Miss
    /// Attacking part of the battleship which has been destroyed will return AttackResult.Miss.
    /// </summary>
    public AttackResult TryAttack(int targetX, int targetY) {
        if (!this.IsPointWithinBoard(targetX, targetY)) {
            throw new Exception($"Point ({targetX}, {targetY}) is outside board.");
        }

        int? battleshipIdOnTargetLocation = this._battleshipBoard[targetX][targetY];
        bool battleshipIsDestroyed = 
            // If there is battle ship at target location
            battleshipIdOnTargetLocation.HasValue
            // If the battleship on target location hasn't been destroyed
            && !this._destroyedBattleshipIds.Contains(battleshipIdOnTargetLocation.Value);
        if (battleshipIsDestroyed) {
            this._destroyedBattleshipIds.Add(battleshipIdOnTargetLocation.Value);
            return AttackResult.Hit;
        }

        return AttackResult.Miss;
    }

    /// <summary>
    /// This method will indicates whether the game has been lost or not based on
    /// the availability of the surviving battleship.
    /// </summary>
    public bool HasLostTheGame() {
        return this._totalNumberOfBattleships > 0
            && this._totalNumberOfBattleships == this._destroyedBattleshipIds.Count;
    }

    private bool IsStraightLine(int startX, int startY, int endX, int endY) {
        return (startX == endX) || (startY == endY);
    }
    
    private bool IsEmptyLine(int startX, int startY, int endX, int endY) {
        int minX = Math.Min(startX, endX);
        int minY = Math.Min(startY, endY);
        int maxX = Math.Min(startX, endX);
        int maxY = Math.Min(startY, endY);
        
        for (int x = minX ; x <= maxX ; x++) {
            for (int y = minY ; y <= maxY ; y++) {
                if (this._battleshipBoard[x][y].HasValue) {
                    return false;
                }
            }
        }
        
        return true;
    }

    private bool IsPointWithinBoard(int x, int y) {
        if (x < 0 || this._boardDimension <= x) return false;
        if (y < 0 || this._boardDimension <= y) return false;
        return true;
    }
    
    public enum AttackResult {
        Hit = 1,
        Miss = 2
    }
}
