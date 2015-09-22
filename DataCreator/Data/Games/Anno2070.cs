﻿using System;
using System.Collections.Generic;
using System.IO;
using XElement.CloudSyncHelper.Serialization.DataTypes;
using XElement.DotNet.System.Environment;

namespace XElement.CloudSyncHelper.DataCreator.Data
{
    internal static partial class Games
    {
        private static GameInfo Anno2070()
        {
            return new GameInfo
            {
                DisplayName = "ANNO 2070",
                FolderName = "Anno 2011 [ANNO 2070]",
                OsConfigs = new List<OsConfiguration>
                {
                    new OsConfiguration
                    {
                        Links = new List<AbstractLinkInfo>
                        {
                            new FolderLinkInfo
                            {
                                DestinationRoot = Environment.SpecialFolder.MyDocuments,
                                DestinationSubFolderPath = Path.Combine("ANNO 2070", "Accounts", "%UPLAY_ACCOUNT_NAME%"),
                                DestinationTargetName = "Savegames",
                                SourceId = "Savegames"
                            }
                        },
                        OsId = OsId.Win81
                    }
                },
                TechnicalNameMatcher = "ANNO 2070"  // TODO: check matcher
            };
        }
    }
}