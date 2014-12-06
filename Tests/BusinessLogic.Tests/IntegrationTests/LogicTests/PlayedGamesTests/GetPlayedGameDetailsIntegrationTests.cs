﻿using System;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
{
    [TestFixture]
    public class GetPlayedGameDetailsIntegrationTests : IntegrationTestBase
    {
        private PlayedGame GetTestSubjectPlayedGame(NemeStatsDataContext dataContextToTestWith)
        {
            return new PlayedGameRetriever(dataContextToTestWith)
                .GetPlayedGameDetails(testPlayedGames[0].Id);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            using (NemeStatsDataContext dbContext = new NemeStatsDataContext())
            {
                PlayedGame playedGame = GetTestSubjectPlayedGame(dbContext);
                Assert.NotNull(playedGame);
            }
        }

        [Test]
        public void ItEagerlyFetchesTheGameResults()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGame playedGame = GetTestSubjectPlayedGame(dataContext);
                    Assert.GreaterOrEqual(testPlayedGames[0].PlayerGameResults.Count, playedGame.PlayerGameResults.Count());
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesTheGameDefinition()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGame playedGame = GetTestSubjectPlayedGame(dataContext);
                    Assert.NotNull(playedGame.GameDefinition);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesThePlayers()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGame playedGame = GetTestSubjectPlayedGame(dataContext);
                    Assert.NotNull(playedGame.PlayerGameResults[0].Player);
                }
            }
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfTheIdIsInvalid()
        {
            int invalidId = -1;
            EntityDoesNotExistException expectedException = new EntityDoesNotExistException(invalidId);

            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);

                Exception actualException = Assert.Throws<EntityDoesNotExistException>(() => retriever.GetPlayedGameDetails(invalidId));

                Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
            }
        }
    }
}
