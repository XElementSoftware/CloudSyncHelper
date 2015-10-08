﻿using System;
using System.Collections.Generic;
using XElement.CloudSyncHelper.DataTypes;
using XElement.CloudSyncHelper.UI.Win32.Model;

namespace XElement.CloudSyncHelper.UI.Win32.DataTypes
{
#region not unit-tested
    public class ProgramInfoViewModel
    {
        public ProgramInfoViewModel( IProgramInfo programInfo, 
                                     IConfig config, 
                                     ConfigForOsHelper cfg4OsHelper )
        {
            this._config = config;
            this._configForOsHelper = cfg4OsHelper;
            this._programInfo = programInfo;

            InitializeExecutionLogic( programInfo );
        }

        public string DisplayName
        {
            get
            {
                var displayName = String.Empty;
                if ( this._programInfo != null )
                {
                    displayName = this._programInfo.DisplayName;
                }
                return displayName;
            }
        }

        public ExecutionLogic ExecutionLogic { get; private set; }

        public bool HasSuitableConfig
        {
            get
            {
                return this.ExecutionLogic != null &&
                    this.ExecutionLogic.HasSuitableConfig();
            }
        }

        private void InitializeExecutionLogic( IProgramInfo programInfo )
        {
            var pathVariables = new PathVariablesDTO
            {
                PathToSyncFolder = this._config.PathToSyncFolder,
                UplayUserName = this._config.UplayAccountName,
                UserName = this._config.UserName
            };
            this.ExecutionLogic = new ExecutionLogic( programInfo, pathVariables, this._configForOsHelper );
        }

        public bool IsInCloud { get { return this.ExecutionLogic.IsInCloud; } }

        public bool IsLinked { get { return this.ExecutionLogic.IsLinked; } }

        public IEnumerable<IOsConfiguration> OsConfigs
        {
            get { return this._programInfo.Configuration.OsConfigs; }
        }

        public string TechnicalNameMatcher { get { return this._programInfo.TechnicalNameMatcher; } }

        private IConfig _config;
        private ConfigForOsHelper _configForOsHelper;
        private IProgramInfo _programInfo;
    }
#endregion
}
