using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    [TestClass]
    public class BattleshipBoardTests
    {
        [TestMethod]
        public void AddBattleShip_AddBattleshipOutsideBoard_ShouldThrowException()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var board = new BattleshipBoard(3);
                board.AddBattleShip(1, 1, 1, 5);
            }, "Point (1, 5) is outside board.");
        }
        
        [TestMethod]
        public void AddBattleShip_AddBattleshipOnOccupiedSpace_ShouldThrowException()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var board = new BattleshipBoard(3);
                board.AddBattleShip(0, 0, 0, 2);
                board.AddBattleShip(0, 1, 2, 1);
            }, "Can not add a battleship from (0, 1) to (2, 1) because the space is occupied.");
        }

        [TestMethod]
        public void AddBattleShip_AddBattleshipOnOccupiedSpaceLineReversed_ShouldThrowException()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var board = new BattleshipBoard(3);
                board.AddBattleShip(0, 0, 0, 2);
                board.AddBattleShip(2, 1, 0, 1);
            }, "Can not add a battleship from (2, 1) to (0, 1) because the space is occupied.");
        }

        [TestMethod]
        public void AddBattleShip_AddBattleshipWithDiagonalPosition_ShouldThrowException()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var board = new BattleshipBoard(3);
                board.AddBattleShip(0, 0, 2, 2);
            }, "The position of the battleship needs to be at straight line position.");
        }

        [TestMethod]
        public void AddBattleShip_AddBattleshipToBoardUntilFull_OK()
        {
            // We don't expect this code to throw exception
            var board = new BattleshipBoard(3);
            Assert.AreEqual(1, board.AddBattleShip(0, 0, 0, 2));
            Assert.AreEqual(2, board.AddBattleShip(1, 0, 2, 0));
            Assert.AreEqual(3, board.AddBattleShip(1, 1, 2, 1));
            Assert.AreEqual(4, board.AddBattleShip(1, 2, 1, 2));
            Assert.AreEqual(5, board.AddBattleShip(2, 2, 2, 2));
            CollectionAssert.AreEqual(
                new int?[][]
                {
                    new int?[] { 1, 1, 1 },
                    new int?[] { 2, 3, 4 },
                    new int?[] { 2, 3, 5 }
                }.SelectMany(x => x).ToArray(),
                board.GetBoardState().SelectMany(x => x).ToArray()
            );
        }

        [TestMethod]
        public void TryAttack_AttackingOutsideBoard_ShouldThrowException()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                var board = new BattleshipBoard(3);
                board.TryAttack(0, 3);
            }, "Point (0, 3) is outside board.");
        }

        [TestMethod]
        public void TryAttack_SuccessfullyAttackingAShip_ShouldThrowException()
        {
            var board = new BattleshipBoard(3);
            Assert.AreEqual(1, board.AddBattleShip(0, 0, 0, 2));
            Assert.AreEqual(2, board.AddBattleShip(1, 0, 2, 0));
            Assert.AreEqual(3, board.AddBattleShip(1, 1, 2, 1));
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(0, 1)); // Ship 1 is down
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(0, 1) ); // Attacking same location is a miss
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(0, 0)); // Attacking any part of Ship 1 again will result in a miss
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(2, 2)); // Attacking at empty location will produce a miss
        }

        [TestMethod]
        public void HasLostTheGame_EmptyBoard_GameIsNotLost()
        {
            var board = new BattleshipBoard(3);
            Assert.AreEqual(false, board.HasLostTheGame());
        }

        [TestMethod]
        public void HasLostTheGame_AllShipsAreStillIntact_GameIsNotLost()
        {
            var board = new BattleshipBoard(3);
            Assert.AreEqual(1, board.AddBattleShip(0, 0, 0, 2));
            Assert.AreEqual(2, board.AddBattleShip(1, 0, 2, 0));
            Assert.AreEqual(3, board.AddBattleShip(1, 1, 2, 1));
            Assert.AreEqual(false, board.HasLostTheGame());
        }

        [TestMethod]
        public void HasLostTheGame_NotAllShipsAreDestroyed_GameIsNotLost()
        {
            var board = new BattleshipBoard(3);
            Assert.AreEqual(1, board.AddBattleShip(0, 0, 0, 2));
            Assert.AreEqual(2, board.AddBattleShip(1, 0, 2, 0));
            Assert.AreEqual(3, board.AddBattleShip(1, 1, 2, 1));
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(0, 1)); // Ship 1 is down
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(0, 1)); // Attacking same location is a miss
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(0, 0)); // Attacking any part of Ship 1 again will result in a miss
            Assert.AreEqual(BattleshipBoard.AttackResult.Miss, board.TryAttack(2, 2)); // Attacking at empty location will produce a miss
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(1, 0)); // Attack ship 2
            Assert.AreEqual(false, board.HasLostTheGame()); // At this point, battleship 3 are still alive
        }

        [TestMethod]
        public void HasLostTheGame_AllShipsAreDestroyed_GameIsLost()
        {
            var board = new BattleshipBoard(3);
            Assert.AreEqual(1, board.AddBattleShip(0, 0, 0, 2));
            Assert.AreEqual(2, board.AddBattleShip(1, 0, 2, 0));
            Assert.AreEqual(3, board.AddBattleShip(1, 1, 2, 1));
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(0, 1)); // Ship 1 is down
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(1, 0)); // Ship 2 is down
            Assert.AreEqual(BattleshipBoard.AttackResult.Hit, board.TryAttack(2, 1)); // Ship 3 is down
            Assert.AreEqual(true, board.HasLostTheGame()); // At this point, all battleships should be destroyed
        }
    }
}