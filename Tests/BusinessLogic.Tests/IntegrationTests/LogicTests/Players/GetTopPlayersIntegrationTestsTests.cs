﻿using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players
{
    [TestFixture]
    public class GetTopPlayersIntegrationTests : IntegrationTestBase
    {
        private PlayerSummaryBuilder playerSummaryBuilderImpl;
        private List<TopPlayer> topPlayersResult;
        private int expectedNumberOfPlayers = 3;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using(IDataContext dataContext = new NemeStatsDataContext())
            {
                playerSummaryBuilderImpl = new PlayerSummaryBuilder(dataContext);

                topPlayersResult = playerSummaryBuilderImpl.GetTopPlayers(3);
            }
        }

        [Test]
        public void ItGetsTheSpecifiedNumberOfPlayers()
        {
            Assert.AreEqual(expectedNumberOfPlayers, topPlayersResult.Count);
        }

        [Test]
        public void ItReturnsThePlayersInDescendingOrderOfTotalNumberOfGamesPlayed()
        {
            int lastNumberofPlayedGames = int.MaxValue;

            foreach(TopPlayer topPlayer in topPlayersResult)
            {
                Assert.LessOrEqual(topPlayer.TotalNumberOfGamesPlayed, lastNumberofPlayedGames);
                lastNumberofPlayedGames = topPlayer.TotalNumberOfGamesPlayed;
            }
        }

        [Test]
        public void ItHasAllOfTheData()
        {
            TopPlayer firstResult = topPlayersResult[0];
            Assert.Greater(firstResult.TotalNumberOfGamesPlayed, 0);
            Assert.Greater(firstResult.TotalPoints, 0);
            Assert.True(!string.IsNullOrWhiteSpace(firstResult.PlayerName));
            Assert.Greater(firstResult.PlayerId, 0);
        }

        [Test]
        public void ItCalculatesTheWinPercentageCorrectly()
        {
            int totalGamesPlayed = 100;
            int totalGamesWon = 20;

            int actualWinPercentage = playerSummaryBuilderImpl.CalculateWinPercentage(totalGamesWon, totalGamesPlayed);

            Assert.AreEqual(20, actualWinPercentage);
        }
    }
}
