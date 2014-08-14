﻿using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected DataContext dataContext;
        protected GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformationMock;
        protected GamingGroupController gamingGroupController;
        protected GamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected GamingGroupCreator gamingGroupCreatorMock;
        protected GamingGroupRetriever gamingGroupRetrieverMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<DataContext>();
            gamingGroupToGamingGroupViewModelTransformationMock = MockRepository.GenerateMock<GamingGroupToGamingGroupViewModelTransformation>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<GamingGroupAccessGranter>();
            gamingGroupCreatorMock = MockRepository.GenerateMock<GamingGroupCreator>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<GamingGroupRetriever>();
            gamingGroupController = new GamingGroupController(
                dataContext, 
                gamingGroupToGamingGroupViewModelTransformationMock, 
                gamingGroupAccessGranterMock,
                gamingGroupCreatorMock,
                gamingGroupRetrieverMock);
            currentUser = new ApplicationUser()
            {
                Id = "user  id",
                CurrentGamingGroupId = 1315
            };
        }
    }
}