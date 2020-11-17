export class BattleshipBoard {
    private readonly _boardDimension: number;
    private readonly _battleshipBoard: number[][];
    private readonly _destroyedBattleshipIds: number[];
    private _newBattleshipIdCounter = 1;

    constructor(boardDimension: number) {
        this._boardDimension = boardDimension;
        this._battleshipBoard = [];
        this._destroyedBattleshipIds = [];
        for (let x = 0; x < boardDimension; x++) {
            this._battleshipBoard.push(Array(boardDimension).fill(0));
        }
    }
    
    public GetBoardState():number[][] {
        return this._battleshipBoard.map(x => x.slice());
    }
    
    public AddBattleShip(startX:number, startY:number, endX:number, endY:number): number {
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
        let currentBattleshipId:number = this._newBattleshipIdCounter;
        for (let x = minX ; x <= maxX ; x++) {
            for (let y = minY ; y <= maxY ; y++) {
                this._battleshipBoard[x][y] = currentBattleshipId;
            }
        }

        this._newBattleshipIdCounter++;
        return currentBattleshipId;
    }
    
    IsStraightLine(startX:number, startY:number, endX:number, endY:number):boolean {
        return (startX == endX) || (startY == endY);
    }
    
    IsEmptyLine(startX:number, startY:number, endX:number, endY:number):boolean {
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

    IsPointWithinBoard(x:number, y:number):boolean {
        if (x < 0 || this._boardDimension <= x) return false;
        if (y < 0 || this._boardDimension <= y) return false;
        return true;
    }
}