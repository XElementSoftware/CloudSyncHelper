﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustMock;
using XElement.CloudSyncHelper.DataTypes;
using XElement.CloudSyncHelper.Serialization.DataTypes;
using XeRandom = XElement.TestUtils.Random;

namespace XElement.CloudSyncHelper.Logic
{
    [TestClass]
    public class testDefinition
    {
        [TestMethod]
        public void testDefinition_ImplementsIDefinition()
        {
            var appInfo = Mock.Create<IApplicationInfo>();
            Mock.Arrange( () => appInfo.DefinitionInfo.OsConfigs )
                .Returns( () => new List<IOsConfigurationInfo>() );
            var parametersDTO = new DefinitionParametersDTO
            {
                ApplicationInfo = appInfo
            };

            var target = new Definition( parametersDTO, new DefinitionDependenciesDTO() );

            Assert.IsInstanceOfType( target, typeof( IDefinition ) );
        }


        [TestMethod]
        public void testDefinition_BestFittingOsConfiguration_NoFulfilledConfig__RandomValues()
        {
            var appInfo = this.CreateRandomApplicationInfo( new List<int> { 3, 2 } );
            var linkFactoryMock = this.CreateLinkFactoryMock( appInfo, new List<int> { 2, 1 } );
            var expectedOsConfigInfo = appInfo.DefinitionInfo.OsConfigs.First();
            var osFilterMock = testDefinition.CreateOsFilterMock();
            IDefinition target = this.CreateInstance( appInfo, linkFactoryMock, osFilterMock );

            var actualOsConfigInfo = target.BestFittingOsConfigurationInfo;

            Assert.AreEqual( expectedOsConfigInfo, actualOsConfigInfo );
        }

        [TestMethod]
        public void testDefinition_BestFittingOsConfiguration_PreferFulfilledConfig__RandomValues()
        {
            var appInfo = this.CreateRandomApplicationInfo( new List<int> { 1, 3 } );
            var linkFactoryMock = this.CreateLinkFactoryMock( appInfo, new List<int> { 1, 2 } );
            var expectedOsConfigInfo = appInfo.DefinitionInfo.OsConfigs.First();
            var osFilterMock = testDefinition.CreateOsFilterMock();
            IDefinition target = this.CreateInstance( appInfo, linkFactoryMock, osFilterMock );

            var actualOsConfigInfo = target.BestFittingOsConfigurationInfo;

            Assert.AreEqual( expectedOsConfigInfo, actualOsConfigInfo );
        }

        [TestMethod]
        public void testDefinition_BestFittingOsConfiguration_PrefersSuitableConfig__RandomValues()
        {
            var appInfo = this.CreateRandomApplicationInfo( new List<int> { 2, 2 } );
            var osConfigs = appInfo.DefinitionInfo.OsConfigs.ToArray();
            var expectedOsCfgInfo = XeRandom.RandomTFromArrayOf( osConfigs );
            var osFilterMock = testDefinition.CreateOsFilterMock();
            Mock.Arrange( () => osFilterMock.GetFilteredOsConfigs( null ) ).IgnoreArguments()
                .Returns( new List<IOsConfigurationInfo>() { expectedOsCfgInfo } ).MustBeCalled();
            var linkFactoryMock = this.CreateLinkFactoryMock( appInfo, new List<int> { 2, 2 } );
            var target = this.CreateInstance( appInfo, linkFactoryMock, osFilterMock );

            var actualOsCfgInfo = target.BestFittingOsConfigurationInfo;

            Mock.Assert( osFilterMock );
            Assert.AreEqual( expectedOsCfgInfo, actualOsCfgInfo );
        }



        private IDefinition CreateInstance( IApplicationInfo appInfo, 
                                            ILinkFactory linkFactoryMock, 
                                            IOsFilter osFilterMock )
        {
            var parametersDTO = new DefinitionParametersDTO
            {
                ApplicationInfo = appInfo,
                OsConfigurationInfos = appInfo.DefinitionInfo.OsConfigs,
                PathVariablesDTO = null
            };
            var dependenciesDTO = new DefinitionDependenciesDTO
            {
                LinkFactory = linkFactoryMock,
                OsFilter = osFilterMock
            };
            return new Definition( parametersDTO, dependenciesDTO );
        }


        private static ILink CreateLinkedLink()
        {
            var linkedLink = Mock.Create<ILink>();
            Mock.Arrange( () => linkedLink.IsLinked ).Returns( true );
            return linkedLink;
        }


        private ILinkFactory CreateLinkFactoryMock( IApplicationInfo appInfo, 
                                                    IList<int> isLinkedCountSequence )
        {
            var factoryMock = Mock.Create<ILinkFactory>();
            for ( int i = 0 ; i < isLinkedCountSequence.Count ; ++i )
            {
                var osConfigInfo = appInfo.DefinitionInfo.OsConfigs.ElementAt(i);
                var isLinkedCount = isLinkedCountSequence[i];
                for ( int j = 0 ; j < isLinkedCount ; ++j )
                {
                    var linkInfo = osConfigInfo.Links[j];
                    Mock.Arrange( () => factoryMock.Get( appInfo,
                                                         linkInfo,
                                                         Arg.IsAny<PathVariablesDTO>() ) )
                        .Returns( testDefinition.CreateLinkedLink() );
                }
            }
            return factoryMock;
        }


        private static IOsFilter CreateOsFilterMock()
        {
            var osFilterMock = Mock.Create<IOsFilter>();
            Mock.Arrange( () => osFilterMock.GetFilteredOsConfigs( null ) ).IgnoreArguments()
                .Returns( input => input );
            return osFilterMock;
        }


        private IApplicationInfo CreateRandomApplicationInfo( IList<int> linkCountSequence )
        {
            AbstractApplicationInfo appInfo = CreateRandomEmptyApplicationInfo();
            appInfo.OsConfigs = new List<OsConfigurationInfo>();

            var osConfigCount = linkCountSequence.Count;
            for ( int i = 0 ; i < osConfigCount ; ++i )
            {
                var osConfig = new OsConfigurationInfo() { Links = new List<AbstractLinkInfo>() };
                var linkCount = linkCountSequence[i];
                for ( int j = 0 ; j < linkCount ; ++j )
                {
                    AbstractLinkInfo linkInfo = CreateRandomLinkInfo();
                    osConfig.Links.Add( linkInfo );
                }
                appInfo.OsConfigs.Add( osConfig );
            }

            return appInfo;
        }


        private static AbstractApplicationInfo CreateRandomEmptyApplicationInfo()
        {
            var shouldBeOfTypeToolInfo = XeRandom.RandomBool();
            AbstractApplicationInfo appInfo = null;
            if ( shouldBeOfTypeToolInfo )
                appInfo = new ToolInfo();
            else
                appInfo = new GameInfo();
            return appInfo;
        }


        private static AbstractLinkInfo CreateRandomLinkInfo()
        {
            AbstractLinkInfo linkInfo = null;
            var shouldCreateFolderLink = XeRandom.RandomBool();
            if ( shouldCreateFolderLink )
                linkInfo = new FolderLinkInfo();
            else
                linkInfo = new FileLinkInfo();
            return linkInfo;
        }
    }
}
