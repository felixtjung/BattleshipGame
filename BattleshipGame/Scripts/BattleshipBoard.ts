export class BattleshipBoard {
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
    private readonly _boardDimension: number;
    private readonly _battleshipBoard: number[][];
    private readonly _destroyedBattleshipIds: number[] = [];
    private _totalNumberOfBattleships: number = 0;
    private _newBattleshipIdCounter: number = 1;

    constructor(boardDimension: number) {
        this._boardDimension = boardDimension;
        this._battleshipBoard = [];
        for (let x = 0; x < boardDimension; x++) {
            this._battleshipBoard.push(Array(boardDimension).fill(0));
        }
    }
    
    /**
     * This method will return the state of the board.
     * 
     * Keep in mind is the value returned by this method is just a copy in order to ensure 
     * that the board is not modified unexpectedly by outside entity.
     */
    public GetBoardState(): number[][] {
        return this._battleshipBoard.map(x => x.slice());
    }

    /**
     * This method will indicates whether the game has been lost or not based on 
     * the availability of the surviving battleship.
     */
    public HasLostTheGame(): boolean {
        return this._totalNumberOfBattleships > 0 
            && this._totalNumberOfBattleships == this._destroyedBattleshipIds.length;
    }

    /**
     * This method represents an attempt to attack specific location on the board.
     * 
     * If the attack is successful, then it will return AttackResult.Hit
     * Otherwise, it will return AttackResult.Miss
     * Attacking part of the battleship which has been destroyed will return AttackResult.Miss.
     */
    public TryAttack(targetX: number, targetY: number): AttackResult {
        if (!this.IsPointWithinBoard(targetX, targetY)) {
            throw new Error(`Point (${targetX}, ${targetY}) is outside board.`);
        }

        let battleshipIdOnTargetLocation: number = this._battleshipBoard[targetX][targetY];
        let battleshipIsDestroyed: boolean = 
            // If there is battle ship at target location
            battleshipIdOnTargetLocation > 0 
            // If the battleship on target location hasn't been destroyed
            && !this._destroyedBattleshipIds.includes(battleshipIdOnTargetLocation);
        if (battleshipIsDestroyed) {
            this._destroyedBattleshipIds.push(battleshipIdOnTargetLocation);
            return AttackResult.Hit;
        }

        return AttackResult.Miss;
    }

    /**
     * This method is called when a battleship is added to the board.
     * 
     * The size, location and the orientation of the battleship is defined by:
     * (startX, startY) => (endX, endY)
     * 
     * When we need to add a battleship which only consists of a single tile, then 
     * we could set (startX, startY) == (endX, endY)
     * 
     */
    public AddBattleShip(startX: number, startY: number, endX: number, endY: number): number {
        if (!this.IsPointWithinBoard(startX, startY)) {
            throw new Error(`Point (${startX}, ${startY}) is outside board.`);
        }
        else if (!this.IsPointWithinBoard(endX, endY)) {
            throw new Error(`Point (${endX}, ${endY}) is outside board.`);
        }
        else if (!this.IsStraightLine(startX, startY, endX, endY)) {
            throw new Error(`The position of the battleship needs to be at straight line position.`);
        }
        else if (!this.IsEmptyLine(startX, startY, endX, endY)) {
            throw new Error(`Can not add a battleship from (${startX}, ${startY}) to (${endX}, ${endY}) because the space is occupied.`);
        }

        let minX = Math.min(startX, endX);
        let minY = Math.min(startY, endY);
        let maxX = Math.max(startX, endX);
        let maxY = Math.max(startY, endY);
        let currentBattleshipId: number = this._newBattleshipIdCounter;
        for (let x = minX ; x <= maxX ; x++) {
            for (let y = minY ; y <= maxY ; y++) {
                this._battleshipBoard[x][y] = currentBattleshipId;
            }
        }

        this._newBattleshipIdCounter++;
        this._totalNumberOfBattleships++;
        return currentBattleshipId;
    }
    
    IsStraightLine(startX: number, startY: number, endX: number, endY: number): boolean {
        return (startX == endX) || (startY == endY);
    }
    
    IsEmptyLine(startX: number, startY: number, endX: number, endY: number): boolean {
        let minX = Math.min(startX, endX);
        let minY = Math.min(startY, endY);
        let maxX = Math.max(startX, endX);
        let maxY = Math.max(startY, endY);
        
        for (let x = minX ; x <= maxX ; x++) {
            for (let y = minY ; y <= maxY ; y++) {
                if (this._battleshipBoard[x][y] != 0) {
                    return false;
                }
            }
        }
        
        return true;
    }

    IsPointWithinBoard(x: number, y: number): boolean {
        if (x < 0 || this._boardDimension <= x) return false;
        if (y < 0 || this._boardDimension <= y) return false;
        return true;
    }
}

export enum AttackResult {
    Hit = 1,
    Miss = 2
}