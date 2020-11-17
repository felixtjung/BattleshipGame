class BattleshipBoard {
    battleshipBoard: number[][];

    constructor(boardDimension: number) {
        this.battleshipBoard = [];
        for (let x = 0; x < boardDimension; x++) {
            this.battleshipBoard.push(new Array(boardDimension));
        }
    }

    public test(): string {
        return "Test 123";
    }
}