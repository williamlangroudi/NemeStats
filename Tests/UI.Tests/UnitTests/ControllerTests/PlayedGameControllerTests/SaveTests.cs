﻿#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using Shouldly;
using UI.Mappers.Interfaces;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SaveTests : PlayedGameControllerTestBase
    {

    protected SavePlayedGameRequest Request;

        [SetUp]
        public virtual void SetUp()
        {
           

            Request = new SavePlayedGameRequest();
        }
    }

    public class When_ModelState_Is_Valid : SaveTests
    {

        [SetUp]
        public virtual void SetUp()
        {

            Request.GameDefinitionId = 1;
            Request.PlayerRanks = new List<CreatePlayerRankRequest>();

        }

    }

    public class When_EditMode_Is_False : When_ModelState_Is_Valid
    {
        private NewlyCompletedGame _expectedNewlyCompletedGame;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            Request.EditMode = false;

            var customMapper = MockRepository.GenerateMock<ICustomMapper<SavePlayedGameRequest, NewlyCompletedGame>>();
            autoMocker.Get<IMapperFactory>().Expect(mock => mock.GetMapper<SavePlayedGameRequest, NewlyCompletedGame>()).Return(customMapper);
            _expectedNewlyCompletedGame = new NewlyCompletedGame();
            customMapper.Expect(mock => mock.Map(Arg<SavePlayedGameRequest>.Is.Anything)).Return(_expectedNewlyCompletedGame);

            autoMocker.Get<ICreatePlayedGameComponent>().Expect(mock => mock.Execute(
               Arg<NewlyCompletedGame>.Is.Anything,
               Arg<ApplicationUser>.Is.Equal(currentUser)))
               .Return(new PlayedGame { Id = 1 });
        }

        [Test]
        public void Then_Returns_Success_Json()
        {
            var result = autoMocker.ClassUnderTest.Save(Request, currentUser) as JsonResult;

            Assert.IsNotNull(result);

            dynamic json = result.Data;
            Assert.IsTrue(json.success);
        }

        [Test]
        public void Then_Creates_New_Played_Game_With_Transaction_Source_Of_Web()
        {
            //--arrange


            //--act
            autoMocker.ClassUnderTest.Save(Request, currentUser);

            //--assert
            var arguments = autoMocker.Get<ICreatePlayedGameComponent>().GetArgumentsForCallsMadeOn(
                mock => mock.Execute(Arg<NewlyCompletedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            arguments.ShouldNotBeNull();
            arguments.Count.ShouldBe(1);
            var firstCall = arguments[0];
            var actualNewlyCompletedGame = firstCall[0] as NewlyCompletedGame;
            actualNewlyCompletedGame.ShouldNotBeNull();

        }

    }

    public class When_GameDefinitionId_Is_Not_Provided : SaveTests
    {
        public override void SetUp()
        {
            base.SetUp();

            Request.GameDefinitionId = null;
        }

    }

    public class When_GameDefinition_Name_Is_Not_Provided : When_GameDefinitionId_Is_Not_Provided
    {
        public override void SetUp()
        {
            base.SetUp();

            Request.GameDefinitionName = null;
        }

        [Test]
        public void Then_Return_BadRequest()
        {
            var result = autoMocker.ClassUnderTest.Save(Request, currentUser) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }
    }

    public class When_GameDefinition_Name_Is_Provided : When_EditMode_Is_False
    {
        public override void SetUp()
        {
            base.SetUp();

            Request.GameDefinitionName = "Test game";

            autoMocker.Get<IGameDefinitionSaver>()
                .Expect(
                    x =>
                        x.CreateGameDefinition(
                            Arg<CreateGameDefinitionRequest>.Matches(
                                m => m.Name == Request.GameDefinitionName), Arg<ApplicationUser>.Is.Equal(currentUser)))
                .Repeat.Once().Return(new GameDefinition());
        }

    }

    public class When_Some_PlayerId_Is_Not_Provided : When_EditMode_Is_False
    {
        public override void SetUp()
        {
            base.SetUp();

            var notExistingPlayerName = "not exists";

            Request.PlayerRanks = new List<CreatePlayerRankRequest>
            {
                new CreatePlayerRankRequest
                {
                    GameRank = 1,
                    PlayerId = 1,
                    PlayerName = "existing player"
                },
                new CreatePlayerRankRequest
                {
                    GameRank = 2,
                    PlayerName = notExistingPlayerName
                }
            };

            autoMocker.Get<IPlayerSaver>()
                .Expect(
                    x =>
                        x.CreatePlayer(
                            Arg<CreatePlayerRequest>.Matches(
                                m => m.Name == notExistingPlayerName), Arg<ApplicationUser>.Is.Equal(currentUser), Arg<bool>.Is.Anything))
                .Repeat.Once().Return(new Player {Id = 2});
        }

    }

    public class When_EditMode_Is_True : When_ModelState_Is_Valid
    {
        public override void SetUp()
        {
            base.SetUp();

            Request.PlayedGameId = 100;
            Request.EditMode = true;

            autoMocker.Get<IPlayedGameSaver>()
                .Expect(
                    x =>
                        x.UpdatePlayedGame(
                            Arg<UpdatedGame>.Matches(
                                m => m.PlayedGameId == Request.PlayedGameId),Arg<TransactionSource>.Is.Equal(TransactionSource.WebApplication),  Arg<ApplicationUser>.Is.Equal(currentUser)))
                .Repeat.Once();
        }


        [Test]
        public void Then_Returns_Success_Json()
        {
            var result = autoMocker.ClassUnderTest.Save(Request, currentUser) as JsonResult;

            Assert.IsNotNull(result);

            dynamic json = result.Data;
            Assert.IsTrue(json.success);
        }


    }
}
