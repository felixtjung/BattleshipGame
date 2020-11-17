import { BattleshipBoard } from "../BattleshipBoard";

describe('BattleshipBoard.AddBattleShip tests', function() {
    test('Adding battleship outside board should throw exception', function() {
        expect(() => {
            let board = new BattleshipBoard(3);
            board.AddBattleShip(1, 1, 1, 5);
        }).toThrowError("Point (1, 5) is outside board.")
    });
    test('Adding overlapping battleship is not allowed', function() {
        expect(() => {
            let board = new BattleshipBoard(3);
            board.AddBattleShip(0, 0, 0, 2);
            board.AddBattleShip(0, 1, 2, 1);
        }).toThrowError("Can not add a battleship from (0, 1) to (2, 1) because the space is occupied.")
    });
    test('Adding overlapping battleship is not allowed - Reversed input line', function() {
        expect(() => {
            let board = new BattleshipBoard(3);
            board.AddBattleShip(0, 0, 0, 2);
            board.AddBattleShip(2, 1, 0, 1);
        }).toThrowError("Can not add a battleship from (2, 1) to (0, 1) because the space is occupied.")
    });
    test('Adding diagonal battleship is not allowed', function() {
        expect(() => {
            let board = new BattleshipBoard(3);
            board.AddBattleShip(0, 0, 2, 2);
        }).toThrowError("The position of the battleship needs to be at straight line position.")
    });
    test('Successfully add battleship until the board is full', function() {
        // We don't expect this code to throw exception
        let board = new BattleshipBoard(3);
        expect(board.AddBattleShip(0, 0, 0, 2)).toEqual(1); // 1
        expect(board.AddBattleShip(1, 0, 2, 0)).toEqual(2);
        expect(board.AddBattleShip(1, 1, 2, 1)).toEqual(3);
        expect(board.AddBattleShip(1, 2, 1, 2)).toEqual(4);
        expect(board.AddBattleShip(2, 2, 2, 2)).toEqual(5);
        expect(board.GetBoardState()).toEqual([
           [1, 1, 1],
           [2, 3, 4],
           [2, 3, 5] 
        ]);
    });

});